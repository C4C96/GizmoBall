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
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			//SizeChanged += (o, e) => Width = (Height - 60) / 3 * 4;
			SceneUC.Scene = new Scene(20, 20);
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

		//private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
		//{
		//	double coefficient = 0.05;
		//	double newHeight = Height + e.Delta * coefficient;
		//	if (newHeight >= MinHeight && newHeight <= MaxHeight)
		//		Height = newHeight;
		//}

		private Vector2? InputSize
		{
			get
			{
				if (int.TryParse(SizeXText.Text, out int x) &&
					int.TryParse(SizeYText.Text, out int y))
					return new Vector2(x, y);
				else
					return null;
			}
		}

		private RigidbodyUC draged = null;	// 当前正在拖动的图片
		private Point lastPosition;			// 拖动前的位置（相对SceneUC）
		private Point mouseOffset;			// 鼠标指针与被拖图片位置间的偏差值

		private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			Image shape = sender as Image;
			if (shape == null) return;
			if (InputSize == null)
			{
				MessageBox.Show("尺寸参数不合法");
				return;
			}
			Vector2 inputSize = InputSize.Value;

			// 在原图的位置生成一个障碍UC
			var position = shape.TranslatePoint(new Point(0, 0), MainGrid);
			RigidbodyUC obstacle = new RigidbodyUC();
			switch (shape.Name)
			{
				case "BallImage":
					if (SceneUC.Scene.Ball != null)
						return;
					obstacle.Rigidbody = new Ball()
					{
						Size = new Vector2(1, 1),
					};
					break;
				case "DestroyerImage":
					obstacle.Rigidbody = new Destroyer()
					{
						Size = inputSize,
					};
					break;
				case "RectangleImage":
					obstacle.Rigidbody = new Physics.Rectangle()
					{
						Size = inputSize,
					};
					break;
				case "TriangleImage":
					obstacle.Rigidbody = new Triangle()
					{
						Size = inputSize,
					};
					break;
				case "CircleImage":
					obstacle.Rigidbody = new Circle()
					{
						Size = inputSize,
					};
					break;
				default:
					return;
			}
			obstacle.Height = SceneUC.BlockHeight * obstacle.Rigidbody.Size.y;
			obstacle.Width = SceneUC.BlockWidth * obstacle.Rigidbody.Size.x;
			obstacle.HorizontalAlignment = HorizontalAlignment.Left;
			obstacle.VerticalAlignment = VerticalAlignment.Top;
			obstacle.Margin = new Thickness(position.X, position.Y, 0, 0);
			MainGrid.Children.Add(obstacle);

			// 实现拖动功能
			// 拖动时父对象为MainGrid
			// 拖动后若坐标合法则父对象为SceneUC.MainCanvas
			// 不合法则删除
			draged = obstacle;
			lastPosition = shape.TranslatePoint(new Point(0, 0), SceneUC);
			mouseOffset = e.GetPosition(shape);
			obstacle.MouseLeftButtonDown += (o1, e1) =>
			{
				e1.Handled = true;
				draged = (o1 as RigidbodyUC);
				mouseOffset = e1.GetPosition(draged);
				lastPosition = draged.TranslatePoint(new Point(0, 0), SceneUC);
				if (draged.Parent == SceneUC.MainCanvas)
				{
					SceneUC.RemoveRigidbody(draged);
					MainGrid.Children.Add(draged);
				}
				draged.SetValue(Panel.ZIndexProperty, 5);	
			};
			obstacle.MouseRightButtonDown += (o1, e1) => (o1 as RigidbodyUC).Rigidbody.Rotate();
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
				
				var pos = draged.TranslatePoint(new Point(0, 0), SceneUC);
				MainGrid.Children.Remove(draged);
				draged.Rigidbody.Position = new Vector2((int)(pos.X / SceneUC.BlockWidth + 0.5), 
														(int)(pos.Y / SceneUC.BlockHeight + 0.5));
				if (SceneUC.AddRigidbody(draged) == false)
				{
					draged.Rigidbody.Position = new Vector2((int)(lastPosition.X / SceneUC.BlockWidth + 0.5),
															(int)(lastPosition.Y / SceneUC.BlockHeight + 0.5));
					SceneUC.AddRigidbody(draged);
				}
				draged = null;
			};
		}

		private void SizeText_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			if (textbox == null) return;

			TextChange[] change = e.Changes.ToArray();
			if (change[0].AddedLength > 0)
			{
				bool isInt = int.TryParse(textbox.Text, out int input);
				if (!isInt || input < 1)
				{
					textbox.Text = textbox.Text.Remove(change[0].Offset, change[0].AddedLength);
					textbox.Select(change[0].Offset, 0);
				}
			}
		}

		private void SizeText_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			var textbox = sender as TextBox;
			if (textbox == null) return;

			Keyboard.Focus(textbox);
			e.Handled = true;
		}

		private void SizeText_GotFocus(object sender, RoutedEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			if (textbox == null) return;

			textbox.SelectAll();
			e.Handled = true;
		}
	}
}
