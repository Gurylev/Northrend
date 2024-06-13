using Microsoft.Win32;
using Northrend.Alodi.Classes;
using Northrend.Alodi.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        /// <returns></returns>
        public (IEnumerable<IRouteNode> Routes, bool IsSuccess) CreateAllRoutes(INodesMap? nodes, string startNodeName, string endNodeName, CancellationToken? cancellationToken = null)
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
    }
}
