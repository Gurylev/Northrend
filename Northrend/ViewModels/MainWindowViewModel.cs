using Microsoft.Extensions.DependencyInjection;
using Northrend.Alodi.Classes;
using Northrend.Alodi.Interfaces;
using Northrend.Alodi.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Northrend.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider mServiceProvider;
        public IMap? CurrentMap { get; private set; }

        public List<CellViewModel> Cells { get; private set; } = [];
        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            mServiceProvider = serviceProvider;
            var importDataService = mServiceProvider.GetRequiredService<ImportDataService>();
            CurrentMap = importDataService.LoadIntegralVelocities(@"Data\IntegrVelocity.xlsx");
            var nodes = importDataService.LoadNodes(@"Data\ГрафДанные.xlsx");
            var info = importDataService.LoadRequestsAndIcebreakers(@"Data\Расписание движения судов.xlsx");

            var routesCreatorService = mServiceProvider.GetRequiredService<RoutesCreatorService>();

            var (routes, isSuccess) = routesCreatorService.CreateAllRoutes(nodes, "окно в европу", "остров Врангеля");

            PrepareCellsViewModels();
        }

        private void PrepareCellsViewModels()
        {
            for (int i = 0; i < 217; i++)
            {
                for (int j = 0; j < 269; j++)
                {
                    Cells.Add(new CellViewModel(mServiceProvider, CurrentMap.Cells[i, j], i, j));
                }
            }
        }
    }
}
