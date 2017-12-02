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
	/// SceneUC.xaml 的交互逻辑
	/// </summary>
	public partial class SceneUC : UserControl
	{
		private Scene scene;
		private bool[,] hasObject;
		private bool isPlaying = false;

		public bool IsPlaying => isPlaying;

		public Scene Scene
		{
			get => scene;
			set
			{
				if (scene != null)
				{
					List<UIElement> tmp = new List<UIElement>();
					foreach (var ui in MainCanvas.Children)
						if (ui is RigidbodyUC)
							tmp.Add(ui as UIElement);
					foreach (var ui in tmp)
						MainCanvas.Children.Remove(ui);
				}
				scene = value;
				DrawAxis((int)scene.Size.x, (int)scene.Size.y);
				hasObject = new bool[(int)scene.Size.x, (int)scene.Size.y];
				AddRigidbody(scene.Ball);
				AddRigidbody(scene.Flipper);
				foreach (var d in scene.Destroyers)
					AddRigidbody(d);
				foreach (var o in scene.Obstacles)
					AddRigidbody(o);
			}
		}

		public double BlockHeight
		{
			get => ActualHeight / Scene.Size.y;
		}

		public double BlockWidth
		{
			get => ActualWidth / Scene.Size.x;
		}

		public SceneUC()
		{
			InitializeComponent();
			// 运行时阻止拖动
			PreviewMouseLeftButtonDown += (o, e) =>
			{
				if (isPlaying)
					e.Handled = true;
			};
		}

		private void DrawAxis(int x, int y)
		{
			AxisGrid.Children.Clear();
			AxisGrid.ColumnDefinitions.Clear();
			AxisGrid.RowDefinitions.Clear();
			for (int i = 0; i < x; i++)
				AxisGrid.ColumnDefinitions.Add(new ColumnDefinition());
			for (int i = 0; i < y; i++)
				AxisGrid.RowDefinitions.Add(new RowDefinition());
			for (int i = 0; i < x; i++)
				for (int j = 0; j < y; j++)
				{
					var border = new Border()
					{
						Margin = new Thickness(0),
						BorderBrush = new SolidColorBrush(Colors.White),
						BorderThickness = new Thickness(0.5),
					};
					Grid.SetColumn(border, i);
					Grid.SetRow(border, j);
					AxisGrid.Children.Add(border);
				}
		}

		/// <summary>
		/// 根据已知的rigidbody，生成对应的UC，并加入SceneUC中，不修改Scene
		/// </summary>
		/// <param name="rigidbody"></param>
		public void AddRigidbody(Rigidbody rigidbody)
		{
			if (rigidbody == null) return;

			int x = (int)rigidbody.Position.x,
				y = (int)rigidbody.Position.y,
				width = (int)rigidbody.Size.x,
				height = (int)rigidbody.Size.y;
			RigidbodyUC uc = new RigidbodyUC()
			{
				Rigidbody = rigidbody,
				Width = BlockWidth * width,
				Height = BlockHeight * height,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				Margin = new Thickness(x * BlockWidth, y * BlockHeight, 0, 0)
			};
			MainWindow.Instance.Register(uc);
			uc.PropertyChanged += RigidbodyUC_PropertyChanged;
			MainCanvas.Children.Add(uc);
			SetHasObject(x, y, width, height, true);
		}

		/// <summary>
		/// 将一个RigidbodyUC加入SceneUC中，并更新Scene
		/// </summary>
		/// <param name="rigidbodyUC"></param>
		/// <returns>若位置非法，则返回null；若位置已有其他RigidbodyUC则返回false；成功返回true</returns>
		public bool? AddRigidbodyUC(RigidbodyUC rigidbodyUC)
		{
			int width = (int)rigidbodyUC.Rigidbody.Size.x;
			int height = (int)rigidbodyUC.Rigidbody.Size.y;
			int x = (int)rigidbodyUC.Rigidbody.Position.x;
			int y = (int)rigidbodyUC.Rigidbody.Position.y;
			if (x < 0 || x + width > scene.Size.x || y < 0 || y + height > scene.Size.y)
				return null;

			if (!IsValible(x, y, width, height))
				return false;

			switch (rigidbodyUC.Type)
			{
				case RigidbodyUC.RigidbodyType.Ball:
					scene.Ball = rigidbodyUC.Rigidbody as Ball;
					break;
				case RigidbodyUC.RigidbodyType.Destroyer:
					scene.Destroyers.Add(rigidbodyUC.Rigidbody as Destroyer);
					break;
				case RigidbodyUC.RigidbodyType.Flipper:
					scene.Flipper = rigidbodyUC.Rigidbody as Flipper;
					break;
				default:
					scene.Obstacles.Add(rigidbodyUC.Rigidbody);
					break;
			}

			SetHasObject(x, y, width, height, true);
			MainCanvas.Children.Add(rigidbodyUC);
			rigidbodyUC.PropertyChanged += RigidbodyUC_PropertyChanged;
			rigidbodyUC.Margin = new Thickness(x * BlockWidth, y * BlockHeight, 0, 0);
			return true;
		}

		public void RemoveRigidbodyUC(RigidbodyUC rigidbodyUC)
		{
			if (rigidbodyUC.Parent != MainCanvas)
				return;

			int x = (int)rigidbodyUC.Rigidbody.Position.x;
			int y = (int)rigidbodyUC.Rigidbody.Position.y;
			int width = (int)rigidbodyUC.Rigidbody.Size.x;
			int height = (int)rigidbodyUC.Rigidbody.Size.y;
			SetHasObject(x, y, width, height, false);
			rigidbodyUC.PropertyChanged -= RigidbodyUC_PropertyChanged;
			MainCanvas.Children.Remove(rigidbodyUC);

			switch (rigidbodyUC.Type)
			{
				case RigidbodyUC.RigidbodyType.Ball:
					scene.Ball = null;
					break;
				case RigidbodyUC.RigidbodyType.Flipper:
					scene.Flipper = null;
					break;
				case RigidbodyUC.RigidbodyType.Destroyer:
					scene.Destroyers.Remove(rigidbodyUC.Rigidbody as Destroyer);
					break;
				default:
					scene.Obstacles.Remove(rigidbodyUC.Rigidbody);
					break;
			}
		}

		private void RigidbodyUC_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			RigidbodyUC rigidbodyUC = sender as RigidbodyUC;
			if (rigidbodyUC == null) return;

			switch (e.PropertyName)
			{
				case "Position":
					rigidbodyUC.Margin = new Thickness(BlockWidth * rigidbodyUC.Rigidbody.Position.x,
														BlockHeight * rigidbodyUC.Rigidbody.Position.y,
														0, 0);
					break;
			}
		}

		private Scene backUp; // 备份
		public void Play()
		{
			isPlaying = true;
			backUp = scene.Clone() as Scene;
			scene.BallDestroied += OnBallDestroied;
			CompositionTarget.Rendering += Update;
		}

		private void Update(object sender, EventArgs e)
		{
			if (scene != null)
				scene.NextFrame();
		}

		private void OnBallDestroied(object o, EventArgs e)
		{
			CompositionTarget.Rendering -= Update;
		}

		public void Stop()
		{
			isPlaying = false;
			scene.BallDestroied -= OnBallDestroied;
			Scene = backUp;
			CompositionTarget.Rendering -= Update;
		}

		private bool IsValible(int x, int y, int width, int height)
		{
			for (int i = x; i < x + width; i++)
				for (int j = y; j < y + height; j++)
					if (hasObject[i, j])
						return false;
			return true;
		}

		private void SetHasObject(int x, int y, int width, int height, bool value)
		{
			for (int i = x; i < x + width; i++)
				for (int j = y; j < y + height; j++)
					hasObject[i, j] = value;
		}
	}
}
