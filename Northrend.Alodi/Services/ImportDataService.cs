using Northrend.Alodi.Classes;
using Northrend.Alodi.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Northrend.Alodi.Services
{
    public class ImportDataService
    {
        INodesMap? nodesMap;



        public ImportDataService(IServiceProvider serviceProvider) 
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public IMap? LoadIntegralVelocities(string path)
        {
            FileInfo existingFile = new(path);
            if (!existingFile.Exists)
                return null;

            IMap? map = null;

            using (ExcelPackage package = new(existingFile))
            {
                if(!package.Workbook.Worksheets[0].Name.Equals("lon")
                    && !package.Workbook.Worksheets[1].Name.Equals("lat"))
                {
                    //вызываем диалоговое окно с ошибкой.
                }

                var worksheetWithLongitude = package.Workbook.Worksheets[0];
                var worksheetWithLatitude = package.Workbook.Worksheets[1];

                int colCount = package.Workbook.Worksheets[0].Dimension.End.Column;
                int rowCount = package.Workbook.Worksheets[0].Dimension.End.Row;

                map = Factory.CreateMap(colCount, rowCount);
                for (int col = 1; col <= colCount; col++)
                {
                    for (int row = 1; row <= rowCount; row++)
                    {
                        var longitudeString = worksheetWithLongitude.Cells[row, col].Value?.ToString();
                        var latitudeString = worksheetWithLatitude.Cells[row, col].Value?.ToString();

                        var longitudeResult = decimal.TryParse(longitudeString, CultureInfo.CurrentCulture, out var longitude);
                        var latitudeResult = decimal.TryParse(latitudeString, CultureInfo.CurrentCulture, out var latitude);
                        if(!longitudeResult || !latitudeResult)
                        {
                            //вызываем ошибку
                        }

                        var cell = Factory.CreateCell(longitude, latitude);

                        for (int i = 2; i < package.Workbook.Worksheets.Count; i++)
                        {
                            var integralVelocityString = package.Workbook.Worksheets[i].Cells[row, col].Value?.ToString();

                            decimal integralVelocity = 0;
                            if(!decimal.TryParse(integralVelocityString, out integralVelocity))
                            {
                                //вызываем ошибку
                            }

                            cell.AddIntegralVelocity(package.Workbook.Worksheets[i].Name, integralVelocity);
                        }

                        map.AddCell(col - 1, row - 1, cell);
                    }
                }
            }
            return map;
        }

        public INodesMap? LoadNodes(string path)
        {
            FileInfo existingFile = new(path);
            if (!existingFile.Exists)
                return null;


            using (ExcelPackage package = new(existingFile))
            {
                if (!package.Workbook.Worksheets[0].Name.Equals("points")
                    && !package.Workbook.Worksheets[1].Name.Equals("edges"))
                {
                    //вызываем диалоговое окно с ошибкой.
                }

                var worksheetWithPoints = package.Workbook.Worksheets[0];

                //int colCount = package.Workbook.Worksheets[0].Dimension.End.Column;
                int rowCount = worksheetWithPoints.Dimension.End.Row;

                nodesMap = Factory.CreateNodeMap();
                for (int row = 2; row <= rowCount; row++)
                {
                    var idString = worksheetWithPoints.Cells[row, 1].Value?.ToString();
                    var latitudeString = worksheetWithPoints.Cells[row, 2].Value?.ToString();
                    var longitudeString = worksheetWithPoints.Cells[row, 3].Value?.ToString();
                    var name = worksheetWithPoints.Cells[row, 4].Value?.ToString() ?? string.Empty;




                    var idResult = ushort.TryParse(idString, CultureInfo.CurrentCulture, out var id);
                    var longitudeResult = decimal.TryParse(longitudeString, CultureInfo.CurrentCulture, out var longitude);
                    var latitudeResult = decimal.TryParse(latitudeString, CultureInfo.CurrentCulture, out var latitude);
                    if (!idResult && !longitudeResult && !latitudeResult)
                    {
                        //вызываем ошибку
                    }

                    var node = Factory.CreateNode(id, name, longitude, latitude);

                    nodesMap.Add(node);
                }


                var worksheetWithEdges = package.Workbook.Worksheets[1];

                rowCount = worksheetWithEdges.Dimension.End.Row;
                for (int row = 2; row <= rowCount; row++)
                {
                    var idString = worksheetWithEdges.Cells[row, 1].Value?.ToString();
                    var startPointString = worksheetWithEdges.Cells[row, 2].Value?.ToString();
                    var endPointString = worksheetWithEdges.Cells[row, 3].Value?.ToString();
                    var distanceString = worksheetWithEdges.Cells[row, 4].Value?.ToString() ?? string.Empty;
                    var statusString = worksheetWithEdges.Cells[row, 6].Value?.ToString();


                    var idResult = ushort.TryParse(idString, CultureInfo.CurrentCulture, out var id);
                    var startPointResult = ushort.TryParse(startPointString, CultureInfo.CurrentCulture, out var startPoint);
                    var endPointResult = ushort.TryParse(endPointString, CultureInfo.CurrentCulture, out var endPoint);
                    var distanceResult = decimal.TryParse(distanceString, CultureInfo.CurrentCulture, out var distance);
                    var statusResult = Enum.TryParse<NodeStatus>(statusString, out var status);
                    if (!idResult || !startPointResult || !endPointResult || !distanceResult || !statusResult)
                    {
                        //вызываем ошибку
                    }

                    INode? startNode = nodesMap.Collection.FirstOrDefault(x => x.Id == startPoint);
                    if(startNode is null)
                    {
                        //error
                        return null;
                    }
                    

                    INode? endNode = nodesMap.Collection.FirstOrDefault(x => x.Id == endPoint);
                    if (endNode is null)
                    {
                        //error
                        return null;
                    }
                    startNode.AddNextNode(endNode.Name, distance, status);

                    endNode = nodesMap.Collection.FirstOrDefault(x => x.Id == endPoint);
                    if (endNode is null)
                    {
                        //error
                        return null;
                    }
                    startNode = nodesMap.Collection.FirstOrDefault(x => x.Id == startPoint);
                    if (startNode is null)
                    {
                        //error
                        return null;
                    }
                    endNode.AddNextNode(startNode.Name, distance, status);
                }



            }
            return nodesMap;
        }

        public (IEnumerable<IRequest>? Requests, IEnumerable<IIcebreakerCard>? Icebreakers) LoadRequestsAndIcebreakers(string path)
        {
            if(nodesMap is null)
                return (null, null);

            FileInfo existingFile = new(path);
            if (!existingFile.Exists)
                return (null, null);

            List<IRequest> requests = [];
            List<IIcebreakerCard> icebreakerCards = [];

            using (ExcelPackage package = new(existingFile))
            {
                if (!package.Workbook.Worksheets[0].Name.Equals("requests")
                    && !package.Workbook.Worksheets[1].Name.Equals("icebreakers"))
                {
                    //вызываем диалоговое окно с ошибкой.
                }

                var worksheetWithRequests = package.Workbook.Worksheets[0];

                //int colCount = package.Workbook.Worksheets[0].Dimension.End.Column;
                int rowCount = worksheetWithRequests.Dimension.End.Row;

                for (int row = 2; row <= rowCount; row++)
                {
                    var nameString = worksheetWithRequests.Cells[row, 1].Value?.ToString();
                    var iceClassString = worksheetWithRequests.Cells[row, 2].Value?.ToString()?.Replace(" ", "");
                    var speedInKnotsString = worksheetWithRequests.Cells[row, 3].Value?.ToString();
                    var nameStartPointNameString = worksheetWithRequests.Cells[row, 4].Value?.ToString() ?? string.Empty;
                    var nameEndPointNameString = worksheetWithRequests.Cells[row, 5].Value?.ToString() ?? string.Empty;
                    var dateAtPointString = worksheetWithRequests.Cells[row, 6].Value?.ToString() ?? string.Empty;

                    var iceClassResult = Enum.TryParse<IceClass>(iceClassString, true, out var iceClass);
                    var speedInKnotsResult = decimal.TryParse(speedInKnotsString, CultureInfo.CurrentCulture, out var speedInKnots);
                    var startPoint = nodesMap.GetNodeByName(nameStartPointNameString);
                    var endPoint = nodesMap.GetNodeByName(nameEndPointNameString);

                    var datetimeConvertToDoubleResult = double.TryParse(dateAtPointString, CultureInfo.CurrentCulture, out var dateAtPointInDouble);
                    DateTime dateAtPoint;
                    if (datetimeConvertToDoubleResult)
                        dateAtPoint = DateTime.FromOADate(dateAtPointInDouble);
                    else
                    {
                        if (!DateTime.TryParse(dateAtPointString, CultureInfo.CurrentCulture, out dateAtPoint))
                        {
                            //вызываем ошибку
                        }
                    }

                    if (!iceClassResult || !speedInKnotsResult || startPoint is null || endPoint is null || !datetimeConvertToDoubleResult)
                    {
                        string t = "";
                        //вызываем ошибку
                    }

                    var request = Factory.CreateRequest(nameString, iceClass, speedInKnots, startPoint, endPoint, dateAtPoint);

                    requests.Add(request);
                }


                var worksheetWithIcebreakers = package.Workbook.Worksheets[1];

                rowCount = worksheetWithIcebreakers.Dimension.End.Row;

                for (int row = 2; row <= rowCount; row++)
                {
                    var nameString = worksheetWithIcebreakers.Cells[row, 1].Value?.ToString();
                    var speedInKnotsString = worksheetWithIcebreakers.Cells[row, 2].Value?.ToString();
                    var iceClassString = worksheetWithIcebreakers.Cells[row, 3].Value?.ToString()?.Replace(" ", "");
                    var nameStartPointNameString = worksheetWithIcebreakers.Cells[row, 4].Value?.ToString() ?? string.Empty;
                    var dateAtPointString = worksheetWithIcebreakers.Cells[row, 5].Value?.ToString() ?? string.Empty;

                    var speedInKnotsResult = decimal.TryParse(speedInKnotsString, CultureInfo.CurrentCulture, out var speedInKnots);
                    var iceClassResult = Enum.TryParse<IceClass>(iceClassString, true, out var iceClass);
                    var startPoint = nodesMap.GetNodeByName(nameStartPointNameString);

                    var datetimeConvertToDoubleResult = double.TryParse(dateAtPointString, CultureInfo.CurrentCulture, out var dateAtPointInDouble);
                    DateTime dateAtPoint;
                    if (datetimeConvertToDoubleResult)
                        dateAtPoint = DateTime.FromOADate(dateAtPointInDouble);
                    else
                    {
                        if(!DateTime.TryParse(dateAtPointString, CultureInfo.CurrentCulture, out dateAtPoint))
                        {
                            //вызываем ошибку
                        }
                    }

                    if (!iceClassResult || !speedInKnotsResult || startPoint is null || !datetimeConvertToDoubleResult)
                    {
                        string t = "";
                        //вызываем ошибку
                    }

                    var icebreakerCard = Factory.CreateIcebreakerCard(nameString, iceClass, speedInKnots, startPoint, dateAtPoint);

                    icebreakerCards.Add(icebreakerCard);
                }
            }
            return (requests, icebreakerCards);
        }
    }
}
