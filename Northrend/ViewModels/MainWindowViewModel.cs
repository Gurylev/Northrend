using Microsoft.Extensions.DependencyInjection;
using Northrend.Alodi.Classes;
using Northrend.Alodi.Interfaces;
using Northrend.Alodi.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Runtime.Serialization;
using static OfficeOpenXml.ExcelErrorValue;

namespace Northrend.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider mServiceProvider;
        public IMap? CurrentMap { get; private set; }

        public List<CellViewModel> Cells { get; private set; } = [];

        public List<CellViewModel> Ports { get; private set; } = [];

        public ObservableCollection<CellViewModel> Route { get; private set; } = [];

        public ObservableCollection<string> Dates { get; set; } = [];

        private int set { get; set; } = 0;

        [Reactive]
        public string SelectedDate { get; set; }

        public INodesMap Nodes { get; set; }

        [Reactive]
        public string FirstPoint { get; set; } = "окно в европу";
        [Reactive]
        public string LastPoint { get; set; } = "остров Врангеля";

        public ReactiveCommand<Unit, Unit> PrepareRouteCommand { get; }

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            mServiceProvider = serviceProvider;
            var importDataService = mServiceProvider.GetRequiredService<ImportDataService>();
            CurrentMap = importDataService.LoadIntegralVelocities(@"Data\IntegrVelocity.xlsx");
            Nodes = importDataService.LoadNodes(@"Data\ГрафДанные.xlsx");
            var info = importDataService.LoadRequestsAndIcebreakers(@"Data\Расписание движения судов.xlsx");
            PrepareRouteCommand = ReactiveCommand.Create(PrepareRoute);
            //var routesCreatorService = mServiceProvider.GetRequiredService<RoutesCreatorService>();

            //var (routes, isSuccess) = routesCreatorService.CreateRoutesByNodes(Nodes, "окно в европу", "остров Врангеля");


            //var result = routesCreatorService.FindCellsForRouteNodes(CurrentMap, routes.First(), 0m);

            PrepareDates(CurrentMap.Cells[0, 0]);

            PrepareCells();
            PreparePorts(Nodes);
            //PrepareRoutes(result.UpdatedRoute);

            this.WhenAnyValue(x => x.SelectedDate)
                .WhereNotNull()
                .Subscribe(x =>
                {                   
                    UpdateCellsIntegralVelocity();
                });
        }

        public void PrepareRoute()
        {
            var routesCreatorService = mServiceProvider.GetRequiredService<RoutesCreatorService>();

            var (routes, isSuccess) = routesCreatorService.CreateRoutesByNodes(Nodes, FirstPoint, LastPoint);

            var result = routesCreatorService.FindCellsForRouteNodes(CurrentMap, routes.First(), 0m);

            PrepareRoutes(result.UpdatedRoute);
        }

        private void UpdateCellsIntegralVelocity()
        {
            foreach (var cell in Cells)
                cell.SelectedDate = SelectedDate;
        }

        private void PrepareDates(ICell cell)
        {
            foreach(var date in cell.IntegralVelocities.Keys)
            {
                Dates.Add(date);
            }
            SelectedDate = Dates.First();
        }

        private void PrepareCells()
        {
            for (int i = 0; i < 217; i++)
            {
                for (int j = 0; j < 269; j++)
                {
                    Cells.Add(new CellViewModel(mServiceProvider, CurrentMap.Cells[i, j], i, j));
                }
            }
        }

        private void PreparePorts(INodesMap nodes)
        {
            foreach (var port in nodes.Collection)
            {
                var cell = Cells.FirstOrDefault(x => Math.Abs(x.AssociatedCell.Latitude - port.Latitude) < (decimal)1 & Math.Abs(x.AssociatedCell.Longitude - port.Longitude) < (decimal)1);
               
                if (cell != null)
                    Ports.Add(new CellViewModel(mServiceProvider, cell.AssociatedCell, cell.AssociatedCell.PositionX, cell.AssociatedCell.PositionY, true));
            }           
        }

        private void PrepareRoutes(IRouteByNodes route)
        {
            Route.Clear();

            foreach (var routePoint in route.CellsPositionsOnMap)
            {
                var cell = Cells.FirstOrDefault(x => x.X/5 == routePoint.i && x.Y/5 == routePoint.j);

                if (cell != null)
                    Route.Add(new CellViewModel(mServiceProvider, cell.AssociatedCell, cell.AssociatedCell.PositionX, cell.AssociatedCell.PositionY, isRoutePoint: true));
            }
        }       
    }
}
