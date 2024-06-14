using Northrend.Alodi.Classes;
using Northrend.Alodi.Interfaces;
using System.Collections.Concurrent;
using System.Data;

namespace Northrend.Alodi.Services
{
    public class RoutesCreatorService
    {
        readonly ConcurrentBag<IRouteNode> mCreatedRoutes = [];
        readonly ConcurrentBag<Task> mTasks = [];

        static readonly byte mMaxDegreeOfParallelism = 32;
        static readonly byte mMaxDeepNodesInRouteRelativelyMinNodesInRoute = 2;
        static int? mMinDeepNodesInMinNodesInRoute = null;

        readonly CancellationToken cancellationToken;
        static readonly byte mTimeWatingToAbortAnalyzeInSeconds = 5;

        public RoutesCreatorService(IServiceProvider serviceProvider) { }

        INode? mStartPoint;
        INode? mEndPoint;
        INodesMap? mNodes;

        /// <summary>
        /// Позволяет проанализировать маршрут от начальной точки до конечной точки с учетом представленного списка нодов.
        /// </summary>
        /// <param name="nodes">Список нодов среди которых проводится анализ.</param>
        /// <param name="startNodeName">Начальная точка.</param>
        /// <param name="endNodeName">Конечная точка.</param>
        /// <param name="cancellationToken">Токен отмены анализа в случае, если анализ будет длиться продолжительное время.</param>
        /// <returns>Возвращает кортеж из списка маршрутов, отсортированных по дистанции, а также флаг успешности выполнения поиска маршуртов.
        /// Возвращает true, если успешно выполнен поиск, иначе возвращает false.</returns>
        public (IEnumerable<IRouteNode> Routes, bool IsSuccess) CreateRoutesByNodes(INodesMap? nodes, string startNodeName, string endNodeName, CancellationToken? cancellationToken = null)
        {
            if (nodes is null)
                return ([], false);

            mNodes = nodes;

            mStartPoint = mNodes.GetNodeByName(startNodeName);
            mEndPoint = mNodes.GetNodeByName(endNodeName);

            if (mStartPoint is null || mEndPoint is null)
                return ([], false);

            mCreatedRoutes.Clear();

            if (cancellationToken is null)
                cancellationToken = new CancellationTokenSource(new TimeSpan(0, mTimeWatingToAbortAnalyzeInSeconds, 0)).Token;

            ConcurrentBag<IRouteNode> routes = [];
            foreach (var nodeLine in mStartPoint.NextNodes)
            {
                var node = mNodes.GetNodeByName(nodeLine.Key);
                if (node is null)
                    return ([], false);

                var route = new RouteNode();
                route.Add(mStartPoint);
                route.Add(node);
                routes.Add(route);
            }

            var isSuccess = AnalyzeNodesToCreateRoutes(routes);
            return (mCreatedRoutes.OrderBy(x => x.Distance), isSuccess);
        }


        bool AnalyzeNodesToCreateRoutes(IReadOnlyCollection<IRouteNode> routes) 
        {
            if (mNodes is null)
                return false;

            if (mStartPoint is null || mEndPoint is null)
                return false;

            ConcurrentBag<IRouteNode> tempRoutes = [];

            Task[] tasks = new Task[routes.Count];
            int taskPosition = 0;
            foreach (var route in routes)
            {
                var task = Task.Factory.StartNew(() =>
                {
                    var lastNode = route.Nodes.Last();
                    if (lastNode.Name.Equals(mEndPoint.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!mCreatedRoutes.Contains(route))
                        {
                            mCreatedRoutes.Add(route);
                            if (mMinDeepNodesInMinNodesInRoute is null)
                                mMinDeepNodesInMinNodesInRoute = route.Nodes.Count;
                        }
                    }
                    else
                    {
                        ParallelOptions parallelOptions = new()
                        {
                            MaxDegreeOfParallelism = mMaxDegreeOfParallelism,
                            CancellationToken = cancellationToken,
                        };
                        Parallel.ForEach(lastNode.NextNodes, parallelOptions, node =>
                        {
                            var findNode = mNodes.GetNodeByName(node.Key);
                            if (findNode is null)
                                return;//необходимо передавать ошибку

                            if (!route.IsExistNodeByName(node.Key))
                            {
                                if (mMinDeepNodesInMinNodesInRoute + mMaxDeepNodesInRouteRelativelyMinNodesInRoute <= route.Nodes.Count)
                                    return;

                                var routeClone = route.Clone();
                                routeClone.Add(findNode);
                                tempRoutes.Add(routeClone);
                            }
                        });
                    }
                }, cancellationToken);
                tasks[taskPosition] = task;
                taskPosition++;
            }
            try
            {
                Task.WaitAll(tasks);
            }
            catch
            {
                return false;
            }

            if (tempRoutes.IsEmpty)
                return true;
            return AnalyzeNodesToCreateRoutes(tempRoutes);
        }


        public (IRouteNode? UpdatedRoute, bool IsSuccess) FindCellsForRouteNodes(IMap? map, IRouteNode? route, decimal shift)
        {
            if(map is null ||  route is null) 
                return (null, false);

            for (int i = 0; i < map.Cells.GetLength(0) - 1; i++)
                for (int j = 0; j < map.Cells.GetLength(1) - 1; j++)
                {
                    decimal deltaX = (map.Cells[i, j].Longitude - map.Cells[i + 1, j].Longitude) / 2;
                    decimal deltaY = (map.Cells[i, j].Latitude - map.Cells[i, j + 1].Latitude) / 2;

                    RectangleM rectangle = new(
                        map.Cells[i, j].Longitude - deltaX, 
                        map.Cells[i, j].Latitude - deltaY, 
                        deltaX * 2, 
                        deltaY * 2);
                    
                    foreach (var node in route.Nodes)
                        if (rectangle.Contains(node.Longitude, node.Latitude, shift))
                            node.AddCell(map.Cells[i, j]);
                }

            if (route.Nodes.Any(x => x.Cell is null))
                return FindCellsForRouteNodes(map, route, shift + 0.1m);

            return (route, !route.Nodes.Any(x => x.Cell is null));
        }
    }
}
