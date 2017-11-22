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
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			SizeChanged += (o, e) =>
			{
				Width = (Height - 60) / 3 * 4;
			};
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					Close();
					break;
			}
		}

		private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			double coefficient = 0.05;
			double newHeight = Height + e.Delta * coefficient;
			if (newHeight >= MinHeight && newHeight <= MaxHeight)
				Height = newHeight;
		}

		private Image draged = null;	// 当前正在拖动的图片
		private Point mouseOffset;		// 鼠标指针与被拖图片位置间的偏差值

		private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			Image shape = sender as Image;
			if (shape == null) return;

			// 在原图的位置复制一份一样的图片
			var position = shape.TranslatePoint(new Point(0, 0), MainGrid);
			Image image = new Image()
			{
				Height = shape.Height,
				Width = shape.Width,
				Source = shape.Source,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				Margin = new Thickness(position.X, position.Y, 0, 0),
			};
			MainGrid.Children.Add(image);

			// 实现拖动功能
			draged = image;
			mouseOffset = e.GetPosition(shape);
			image.MouseLeftButtonDown += (o1, e1) =>
			{
				e1.Handled = true;
				draged = (o1 as Image);
				mouseOffset = e1.GetPosition(draged);
				draged.SetValue(Panel.ZIndexProperty, 1);
			};
			this.MouseMove += (o1, e1) =>
			{
				if (draged == null) return;
				var mousePosition = e1.GetPosition(MainGrid);
				draged.Margin = new Thickness(mousePosition.X - mouseOffset.X, mousePosition.Y - mouseOffset.Y, 0, 0);
			};
			this.MouseLeftButtonUp += (o1, e1) =>
			{
				if (draged == null) return;
				draged.SetValue(Panel.ZIndexProperty, 0);
				draged = null;
			};
		}
	}
}
