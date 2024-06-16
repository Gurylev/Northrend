using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using Avalonia.Threading;
using Northrend.Alodi.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Northrend.ViewModels
{
	public class CellViewModel : ViewModelBase
	{
		private readonly IServiceProvider mServiceProvider;
        [Reactive]
		public ICell AssociatedCell { get; set; }

        [Reactive]
        public string SelectedDate { get; set; }

        [Reactive]
		public IBrush CellColor { get; set; }

		public string Point => $"{X/5}.{Y/5}";
        
        public string Coordinates => $"{AssociatedCell.Latitude}.{AssociatedCell.Longitude}";
        [Reactive]
        public decimal CurrentIntegralVelocity { get; set; }

        [Reactive]
        public bool IsPort { get;set; }

        [Reactive]
        public bool IsRoutePoint { get; set; }

        [Reactive]
        public int zIndex { get; set; } = -1;

        [Reactive]
        public string PortName { get; set; }
        [Reactive]
        public int X { get;set; }
        [Reactive]
		public int Y { get;set; }

        public CellViewModel(IServiceProvider serviceProvider, ICell associatedCell, int x, int y, bool isPort = false, bool isRoutePoint = false, string portName = "")
		{
			mServiceProvider = serviceProvider;
			AssociatedCell = associatedCell;
            IsPort = isPort;
            IsRoutePoint = isRoutePoint;

            PortName = portName;

            SelectedDate = AssociatedCell.IntegralVelocities.First().Key;
            CurrentIntegralVelocity = AssociatedCell.IntegralVelocities.First().Value;

            X = x * 5;
			Y = y * 5;

			SetColor();

            this.WhenAnyValue(x => x.SelectedDate)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    CurrentIntegralVelocity = AssociatedCell.IntegralVelocities[SelectedDate];
                    SetColor();
                });
		}

		public void SetColor()
		{
            Dispatcher.UIThread.Invoke(() =>
            {
                if (IsPort)
                {
                    CellColor = new SolidColorBrush(Colors.Yellow);
                    zIndex = 2;
                    return;
                }

                if (IsRoutePoint)
                {
                    CellColor = new SolidColorBrush(Colors.Red);
                    zIndex = 5;
                    return;
                }

                switch (CurrentIntegralVelocity)
                {
                    case decimal n when (n < 0):
                        CellColor = new SolidColorBrush(Colors.Brown);
                        break;
                    case decimal n when (n == 0):
                        CellColor = new SolidColorBrush(Colors.DarkBlue);

                        break;
                    case decimal n when (n > 0 && n < 15):
                        CellColor = new SolidColorBrush(Colors.Blue);

                        break;
                    case decimal n when (n > 14 && n < 20):
                        CellColor = new SolidColorBrush(Colors.LightBlue);
                        break;
                    case decimal n when (n > 19):
                        CellColor = new SolidColorBrush(Colors.AliceBlue);
                        break;
                }

                if (X == 0 && Y == 0)
                    CellColor = new SolidColorBrush(Colors.Yellow);

                if (X / 5 == 216 && Y / 5 == 268)
                    CellColor = new SolidColorBrush(Colors.Green);

                if (X == 0 && Y / 5 == 268)
                    CellColor = new SolidColorBrush(Colors.Yellow);

                if (X / 5 == 216 && Y == 0)
                    CellColor = new SolidColorBrush(Colors.Green);
            });
            
        }
	}
}