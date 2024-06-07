using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using Northrend.Alodi.Interfaces;
using ReactiveUI;

namespace Northrend.ViewModels
{
	public class CellViewModel : ViewModelBase
	{
		private readonly IServiceProvider mServiceProvider;
		public ICell AssociatedCell { get; set; }

		public IBrush CellColor { get; set; }

		public string Point => $"{X/5}.{Y/5}";
        public string Coordinates => $"{AssociatedCell.Latitude}.{AssociatedCell.Longitude}";

        public decimal CurrentIntegralVelocity { get; set; } 

        public int X { get;set; }
		public int Y { get;set; }

		public CellViewModel(IServiceProvider serviceProvider, ICell associatedCell, int x, int y)
		{
			mServiceProvider = serviceProvider;
			AssociatedCell = associatedCell;

			X = x * 5;
			Y = y * 5;

			SetColor();
		}

		private void SetColor()
		{
            CurrentIntegralVelocity = AssociatedCell.IntegralVelocities.First().Value;

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

			if(X == 0 && Y == 0)
                CellColor = new SolidColorBrush(Colors.Yellow);

            if (X / 5 == 216 && Y / 5 == 268)
                CellColor = new SolidColorBrush(Colors.Green);

            if (X == 0 && Y / 5 == 268)
                CellColor = new SolidColorBrush(Colors.Yellow);

            if (X / 5 == 216 && Y == 0)
                CellColor = new SolidColorBrush(Colors.Green);
        }
	}
}