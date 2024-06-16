using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using Northrend.Alodi.Classes;
using Northrend.Alodi.Interfaces;
using Northrend.Alodi.Services;
using Northrend.Views;
using OfficeOpenXml.LoadFunctions.Params;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static OfficeOpenXml.ExcelErrorValue;

namespace Northrend.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider mServiceProvider;
        [Reactive]
        public IMap? CurrentMap { get; private set; }

        [Reactive]
        public ObservableCollection<CellViewModel> Cells { get; private set; } = [];

        [Reactive]
        public ObservableCollection<string> Dates { get; set; } = [];

        private int set { get; set; } = 0;

        [Reactive]
        public string SelectedDate { get; set; }

        [Reactive]
        public ObservableCollection<string> AllPoints { get; set; } = [];

        [Reactive]
        public string SelectedFirstPoint { get; set; } = "окно в европу";

        [Reactive]
        public string SelectedLastPoint { get; set; } = "остров Врангеля";

        [Reactive]
        public INodesMap Nodes { get; set; }       

        [Reactive]
        public string PathToIceVelocities { get; set; } = @"Data\IntegrVelocity.xlsx";
        [Reactive]
        public string PathToGraph { get; set; } = @"Data\ГрафДанные.xlsx";

        [Reactive]
        public string PathToRequests { get; set; } = @"Data\Расписание движения судов.xlsx";

        public ReactiveCommand<Unit, Unit> PrepareRouteCommand { get; }

        public ReactiveCommand<Unit, Unit> LoadIceCommand { get; }

        public ReactiveCommand<Unit, Unit> LoadGraphCommand { get; }

        public ReactiveCommand<Unit, Unit> LoadRequestsCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            mServiceProvider = serviceProvider;

            //LoadData();
           
            PrepareRouteCommand = ReactiveCommand.Create(PrepareRoute);
            LoadIceCommand = ReactiveCommand.CreateFromTask(SetPathToIce);
            LoadGraphCommand = ReactiveCommand.CreateFromTask(SetPathToGraph);
            LoadRequestsCommand = ReactiveCommand.CreateFromTask(SetPathToRequests);
            LoadDataCommand = ReactiveCommand.CreateFromTask(LoadData);
            //var routesCreatorService = mServiceProvider.GetRequiredService<RoutesCreatorService>();

            //var (routes, isSuccess) = routesCreatorService.CreateRoutesByNodes(Nodes, "окно в европу", "остров Врангеля");


            //var result = routesCreatorService.FindCellsForRouteNodes(CurrentMap, routes.First(), 0m);


            //PrepareRoutes(result.UpdatedRoute);

            this.WhenAnyValue(x => x.SelectedDate)
                .WhereNotNull()
                .Subscribe(x =>
                {                   
                    UpdateCellsIntegralVelocity();
                });
        }

        public async Task LoadData()
        {
            var importDataService = mServiceProvider.GetRequiredService<ImportDataService>();
            CurrentMap = importDataService.LoadIntegralVelocities(PathToIceVelocities);
            Nodes = importDataService.LoadNodes(PathToGraph);
            var info = importDataService.LoadRequestsAndIcebreakers(PathToRequests);

            PrepareDates(CurrentMap.Cells[0, 0]);
            PreparePoints(Nodes);

            Cells.Clear();

            PrepareCells();
            PreparePorts(Nodes);

            mServiceProvider.GetRequiredService<MainWindow>().InvalidateVisual();
        }

        public void PrepareRoute()
        {
            var routesCreatorService = mServiceProvider.GetRequiredService<RoutesCreatorService>();

            var (routes, isSuccess) = routesCreatorService.CreateRoutesByNodes(Nodes, SelectedFirstPoint, SelectedLastPoint);

            var result = routesCreatorService.FindCellsForRouteNodes(CurrentMap, routes.First(), 0m);

            PrepareRoutes(result.UpdatedRoute);
        }
        public async Task SetPathToIce()
        {
            var mainWindow = mServiceProvider.GetRequiredService<MainWindow>();
            var topLevel = TopLevel.GetTopLevel(mainWindow);

            // Start async operation to open the dialog.
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {               
                AllowMultiple = false
            });
            try
            {
                PathToIceVelocities = files.FirstOrDefault().TryGetLocalPath();
            }
            catch (Exception)
            {

            }           
            
        }
        public async Task SetPathToGraph()
        {
            var mainWindow = mServiceProvider.GetRequiredService<MainWindow>();
            var topLevel = TopLevel.GetTopLevel(mainWindow);

            // Start async operation to open the dialog.
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false
            });

            try
            {
                PathToGraph = files.FirstOrDefault().TryGetLocalPath();
            }
            catch (Exception)
            {

            }
        }

        public async Task SetPathToRequests()
        {
            var mainWindow = mServiceProvider.GetRequiredService<MainWindow>();
            var topLevel = TopLevel.GetTopLevel(mainWindow);

            // Start async operation to open the dialog.
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false
            });
            try
            {
                PathToRequests = files.FirstOrDefault().TryGetLocalPath();
            }
            catch (Exception)
            {

            }
        }

        private void UpdateCellsIntegralVelocity()
        {
            foreach (var cell in Cells)
                cell.SelectedDate = SelectedDate;
        }

        private void PrepareDates(ICell cell)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                Dates.Clear();
                foreach (var date in cell.IntegralVelocities.Keys)
                {
                    Dates.Add(date);
                }
                SelectedDate = Dates.First();
            });
            
        }

        private void PreparePoints(INodesMap nodes)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                AllPoints.Clear();
                AllPoints.AddRange(nodes.Collection.Select(x => x.Name));                
            });

        }

        private void PrepareCells()
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                for (int i = 0; i < 217; i++)
                {
                    for (int j = 0; j < 269; j++)
                    {
                        Cells.Add(new CellViewModel(mServiceProvider, CurrentMap.Cells[i, j], i, j));
                    }
                }
            });           
        }

        private void PreparePorts(INodesMap nodes)
        {
            Dispatcher.UIThread.Invoke(() =>
            {                
                foreach (var port in nodes.Collection)
                {
                    var cell = Cells.FirstOrDefault(x => Math.Abs(x.AssociatedCell.Latitude - port.Latitude) < (decimal)1 & Math.Abs(x.AssociatedCell.Longitude - port.Longitude) < (decimal)1);

                    if (cell is null)
                        continue;

                    cell.IsPort = true;
                    cell.PortName = port.Name;
                    cell.SetColor();

                    //if (cell != null)
                    //    Ports.Add(new CellViewModel(mServiceProvider, cell.AssociatedCell, cell.AssociatedCell.PositionX, cell.AssociatedCell.PositionY, true, portName: port.Name));
                }
            });                   
        }

        private void PrepareRoutes(IRouteByNodes route)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                var lastRoute = Cells.Where(x => x.IsRoutePoint).ToList();
                for (int i = 0; i < lastRoute.Count(); i++)
                {
                    lastRoute[i].IsRoutePoint = false;
                    lastRoute[i].SetColor();
                }

                foreach (var routePoint in route.CellsPositionsOnMap)
                {
                    var cell = Cells.FirstOrDefault(x => x.X / 5 == routePoint.i && x.Y / 5 == routePoint.j);

                    if (cell is null)
                        continue;

                    cell.IsRoutePoint = true;
                    cell.SetColor();

                    //if (cell != null)
                    //    Route.Add(new CellViewModel(mServiceProvider, cell.AssociatedCell, cell.AssociatedCell.PositionX, cell.AssociatedCell.PositionY, isRoutePoint: true));
                }
            });
           
        }       
    }
}
