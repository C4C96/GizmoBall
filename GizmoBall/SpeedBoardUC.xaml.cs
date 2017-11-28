using GizmoBall.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GizmoBall
{
	/// <summary>
	/// SpeedBoardUC.xaml 的交互逻辑
	/// </summary>
	public partial class SpeedBoardUC : UserControl
	{
		private Vector2 speed;

		public Vector2 Speed => speed;

		public double MaxSpeed
		{
			get => SpeedXSlider.Maximum;
			set
			{
				SpeedXSlider.Maximum = SpeedYSlider.Maximum = value;
				SpeedXSlider.Minimum = SpeedYSlider.Minimum = -value;
			}
		}

		public double Speed_X
		{
			get => speed.x;
			set
			{
				speed.x = (float)value;
				DrawArrow(speed.x, speed.y);
			}
		}

		public double Speed_Y
		{
			get => speed.y;
			set
			{
				speed.y = (float)value;
				DrawArrow(speed.x, speed.y);
			}
		}
		public SpeedBoardUC()
		{
			InitializeComponent();
			DataContext = this;
		}

		private void DrawArrow(double x, double y)
		{
			Line.X1 = Canvas.ActualWidth / 2;
			Line.Y1 = Canvas.ActualHeight / 2;
			Arrow_L.X1 = Arrow_R.X1 = Line.X2 = Line.X1 + (x / MaxSpeed) * Canvas.ActualWidth / 2;
			Arrow_L.Y1 = Arrow_R.Y1 = Line.Y2 = Line.Y1 + (y / MaxSpeed) * Canvas.ActualHeight / 2;
			double degree = Math.Atan(y / x);
			if (x < 0)
				degree += Math.PI;
			Arrow_L.X2 = Line.X2 - Math.Cos(degree + Math.PI / 6) * 10;
			Arrow_L.Y2 = Line.Y2 - Math.Sin(degree + Math.PI / 6) * 10;
			Arrow_R.X2 = Line.X2 - Math.Cos(degree - Math.PI / 6) * 10;
			Arrow_R.Y2 = Line.Y2 - Math.Sin(degree - Math.PI / 6) * 10;
		}

		private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			var position = e.GetPosition(Canvas);
			Speed_X = (position.X / Canvas.ActualWidth - 0.5) * MaxSpeed * 2;
			SpeedXSlider.Value = Speed_X;
			Speed_Y = (position.Y / Canvas.ActualHeight - 0.5) * MaxSpeed * 2;
			SpeedYSlider.Value = Speed_Y;
		}
	}
}
