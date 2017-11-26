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

		public Scene Scene
		{
			get => scene;
			set
			{
				if (scene != null)
					; // 解绑
				scene = value;
				// 绑定
				DrawAxis((int)scene.Size.x, (int)scene.Size.y);
				hasObject = new bool[(int)scene.Size.x, (int)scene.Size.y];
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
		/// 将一个RigidbodyUC加入SceneUC中
		/// </summary>
		/// <param name="rigidbodyUC"></param>
		/// <returns>若位置非法，则返回null；若位置已有其他RigidbodyUC则返回false；成功返回true</returns>
		public bool? AddRigidbody(RigidbodyUC rigidbodyUC)
		{
			int width = (int)rigidbodyUC.Rigidbody.Size.x;
			int height = (int)rigidbodyUC.Rigidbody.Size.y;
			int x = (int)rigidbodyUC.Rigidbody.Position.x;
			int y = (int)rigidbodyUC.Rigidbody.Position.y;
			if (x < 0 || x + width > scene.Size.x || y < 0 || y + height > scene.Size.y)
				return null;
			
			for (int i = x; i < x + width; i++)
				for (int j = y; j < y + height; j++)
					if (hasObject[i, j])
						return false;

			switch (rigidbodyUC.Type)
			{
				case RigidbodyUC.RigidbodyType.Ball:
					scene.Ball = rigidbodyUC.Rigidbody as Ball;
					break;
				case RigidbodyUC.RigidbodyType.Destroyer:
					scene.Destroyers.Add(rigidbodyUC.Rigidbody as Destroyer);
					break;
				default:
					scene.Obstacles.Add(rigidbodyUC.Rigidbody);
					break;
			}

			for (int i = x; i < x + width; i++)
				for (int j = y; j < y + height; j++)
					hasObject[i, j] = true;
			MainCanvas.Children.Add(rigidbodyUC);
			rigidbodyUC.Margin = new Thickness(x * BlockWidth, y * BlockHeight, 0, 0);
			return true;
		}

		public void RemoveRigidbody(RigidbodyUC rigidbodyUC)
		{
			if (rigidbodyUC.Parent != MainCanvas)
				return;

			int x = (int)rigidbodyUC.Rigidbody.Position.x;
			int y = (int)rigidbodyUC.Rigidbody.Position.y;
			int width = (int)rigidbodyUC.Rigidbody.Size.x;
			int height = (int)rigidbodyUC.Rigidbody.Size.y;
			for (int i = x; i < x + width; i++)
				for (int j = y; j < y + height; j++)
					hasObject[i, j] = false;
			MainCanvas.Children.Remove(rigidbodyUC);

			switch (rigidbodyUC.Type)
			{
				case RigidbodyUC.RigidbodyType.Ball:
					scene.Ball = null;
					break;
				case RigidbodyUC.RigidbodyType.Destroyer:
					scene.Destroyers.Remove(rigidbodyUC.Rigidbody as Destroyer);
					break;
				default:
					scene.Obstacles.Remove(rigidbodyUC.Rigidbody);
					break;
			}
		}

	}
}
