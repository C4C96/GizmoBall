using GizmoBall.Physics;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
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
using System.Xml.Serialization;

namespace GizmoBall
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		private const float FLIPPER_SPEED = 2.0f;

		private static MainWindow instance;
		public static MainWindow Instance => instance;

		public MainWindow()
		{
			InitializeComponent();
			instance = this;
			//SizeChanged += (o, e) => Width = (Height - 60) / 3 * 4;
			SceneUC.Scene = new Scene(20, 20);
			SpeedBoardUC.MaxSpeed = 5;
			DataContext = this;
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

		public float? InputGravity
		{
			get
			{
				if (float.TryParse(GravityText.Text, out float g))
					return g;
				else
					return null;
			}
		}

		public double BallStartSpeed_X
		{
			get; set;
		}

		public double BallStartSpeed_Y
		{
			get; set;
		}

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
				case "FlipperImage":
					if (SceneUC.Scene.Flipper != null)
						return;
					obstacle.Rigidbody = new Flipper()
					{
						Size = inputSize,
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
			lastPosition = obstacle.TranslatePoint(new Point(0, 0), SceneUC);
			mouseOffset = new Point(obstacle.Width / 2, obstacle.Height / 2);
			Register(obstacle);
		}

		private RigidbodyUC draged = null;  // 当前正在拖动的图片
		private Point lastPosition;         // 拖动前的位置（相对SceneUC）
		private Point mouseOffset;          // 鼠标指针与被拖图片位置间的偏差值

		/// <summary>
		/// 对一个RigidbodyUC进行注册，使之能够被拖动、右键旋转
		/// </summary>
		/// <param name="rigidbodyUC"></param>
		public void Register(RigidbodyUC rigidbodyUC)
		{
			rigidbodyUC.MouseLeftButtonDown += (o1, e1) =>
			{
				e1.Handled = true;
				draged = (o1 as RigidbodyUC);
				mouseOffset = e1.GetPosition(draged);
				lastPosition = draged.TranslatePoint(new Point(0, 0), SceneUC);
				if (draged.Parent == SceneUC.MainCanvas)
				{
					SceneUC.RemoveRigidbodyUC(draged);
					MainGrid.Children.Add(draged);
				}
				draged.SetValue(Panel.ZIndexProperty, 5);
			};
			rigidbodyUC.MouseRightButtonDown += (o1, e1) => (o1 as RigidbodyUC).Rigidbody.Rotate();
		}

		private void PlayStopButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// Stop
			if (SceneUC.IsPlaying)
			{
				PlayStopButton.Source = new BitmapImage(new Uri(@"./Images/PlayButton.png", UriKind.Relative));
				SceneUC.Stop();
			}
			// Play
			else
			{
				if (InputGravity == null)
				{
					MessageBox.Show("重力加速度参数不合法");
					return;
				}
				if (SceneUC.Scene.Ball == null)
				{
					MessageBox.Show("未放置小球");
					return;
				}
				if (SceneUC.Scene.Flipper == null)
				{
					MessageBox.Show("未放置挡板");
					return;
				}
				PlayStopButton.Source = new BitmapImage(new Uri(@"./Images/StopButton.png", UriKind.Relative));
				SceneUC.Scene.Gravity = InputGravity.Value;
				SceneUC.Scene.Ball.Speed = SpeedBoardUC.Speed;
				if (SceneUC.Scene.Flipper != null) SceneUC.Scene.Flipper.Speed = Vector2.Zero; 
				SceneUC.Play();
			}
		}

		private void SizeBox_TextChanged(object sender, TextChangedEventArgs e)
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

		private void GravityBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			if (textbox == null) return;

			TextChange[] change = e.Changes.ToArray();
			if (change[0].AddedLength > 0)
			{
				bool isFloat = float.TryParse(textbox.Text, out float input);
				if (!isFloat || input < 0)
				{
					textbox.Text = textbox.Text.Remove(change[0].Offset, change[0].AddedLength);
					textbox.Select(change[0].Offset, 0);
				}
			}
		}

		private void TextBox_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			var textbox = sender as TextBox;
			if (textbox == null) return;

			Keyboard.Focus(textbox);
			e.Handled = true;
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			if (textbox == null) return;

			textbox.SelectAll();
			e.Handled = true;
		}

		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
			if (draged == null) return;
			var mousePosition = e.GetPosition(MainGrid);
			draged.Margin = new Thickness(mousePosition.X - mouseOffset.X, mousePosition.Y - mouseOffset.Y, 0, 0);
		}

		private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (draged == null) return;
			draged.SetValue(Panel.ZIndexProperty, 0);

			var pos = draged.TranslatePoint(new Point(0, 0), SceneUC);
			MainGrid.Children.Remove(draged);
			draged.Rigidbody.Position = new Vector2((int)(pos.X / SceneUC.BlockWidth + 0.5),
													(int)(pos.Y / SceneUC.BlockHeight + 0.5));
			if (SceneUC.AddRigidbodyUC(draged) == false)
			{
				draged.Rigidbody.Position = new Vector2((int)(lastPosition.X / SceneUC.BlockWidth + 0.5),
														(int)(lastPosition.Y / SceneUC.BlockHeight + 0.5));
				SceneUC.AddRigidbodyUC(draged);
			}
			draged = null;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				DragMove();
			}
			catch
			{
			}
		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					Close();
					break;
				case Key.Left:
					if (SceneUC.IsPlaying && SceneUC.Scene.Flipper != null)
						SceneUC.Scene.Flipper.Speed = Vector2.Left * FLIPPER_SPEED;
					break;
				case Key.Right:
					if (SceneUC.IsPlaying && SceneUC.Scene.Flipper != null)
						SceneUC.Scene.Flipper.Speed = Vector2.Right * FLIPPER_SPEED;
					break;
			}
		}

		private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Left:
				case Key.Right:
					if (SceneUC.IsPlaying && SceneUC.Scene.Flipper != null)
						SceneUC.Scene.Flipper.Speed = Vector2.Zero;
					break;
			}
		}

		private void SaveButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog()
			{
				Filter = "GizmoBall场景文件(*.ooad)|*.ooad",
				CheckPathExists = true,
				RestoreDirectory = true,
			};
			if (sfd.ShowDialog() == true)
			{
				FileStream fs = new FileStream(sfd.FileName, FileMode.Create);
				XmlSerializer xs = new XmlSerializer(typeof(Scene));
				xs.Serialize(fs, SceneUC.Scene);
				fs.Close();
			}
		}

		private void LoadButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog()
			{
				Filter = "GizmoBall场景文件(*.ooad)|*.ooad|所有文件|*.*",
				CheckFileExists = true,
				RestoreDirectory = true,
			};
			if (ofd.ShowDialog() == true)
			{
				FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
				XmlSerializer xs = new XmlSerializer(typeof(Scene));
				SceneUC.Scene = xs.Deserialize(fs) as Scene;
				fs.Close();
			}
		}
	}
}
