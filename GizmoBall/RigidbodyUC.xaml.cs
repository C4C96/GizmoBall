using GizmoBall.Physics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// RigidbodyUC.xaml 的交互逻辑
	/// </summary>
	public partial class RigidbodyUC : UserControl
	{
		private Rigidbody rigidbody;
		private RigidbodyType type;

		public Rigidbody Rigidbody
		{
			get => rigidbody;
			set
			{
				if (rigidbody != null)
					rigidbody.PropertyChanged -= Rigidbody_PropertyChanged;
				rigidbody = value;
				if (rigidbody != null)
				{
					rigidbody.PropertyChanged += Rigidbody_PropertyChanged;
					if (rigidbody is Ball)
					{
						type = RigidbodyType.Ball;
						Image.Source = new BitmapImage(new Uri(@"./Images/Ball.png", UriKind.Relative));
					}
					else if (rigidbody is Destroyer)
					{
						type = RigidbodyType.Destroyer;
						Image.Source = new BitmapImage(new Uri(@"./Images/Destroyer.png", UriKind.Relative));
					}
					else if (rigidbody is Physics.Rectangle)
					{
						type = RigidbodyType.Rectangle;
						Image.Source = new BitmapImage(new Uri(@"./Images/Rectangle.png", UriKind.Relative));
					}
					else if (rigidbody is Triangle)
					{
						type = RigidbodyType.Triangle;
						OnTriangleStateChanged();
						Image.Source = new BitmapImage(new Uri(@"./Images/Triangle.png", UriKind.Relative));
					}
					else if (rigidbody is Circle)
					{
						type = RigidbodyType.Circle;
						Image.Source = new BitmapImage(new Uri(@"./Images/Circle.png", UriKind.Relative));
					}
				}
			}
		}

		public RigidbodyType Type => type;

		private void Rigidbody_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "TriangleState":
					OnTriangleStateChanged();
					break;
			}
		}

		private void OnTriangleStateChanged()
		{
			Triangle triangle = rigidbody as Triangle;
			if (triangle == null) return;
			ScaleTransform scaleTransform = new ScaleTransform();
			switch (triangle.State)
			{
				case Triangle.TriangleState.LeftDown:
					scaleTransform.ScaleX = scaleTransform.ScaleY = 1;
					break;
				case Triangle.TriangleState.LeftUp:
					scaleTransform.ScaleX = 1;
					scaleTransform.ScaleY = -1;
					break;
				case Triangle.TriangleState.RightDown:
					scaleTransform.ScaleX = -1;
					scaleTransform.ScaleY = 1;
					break;
				case Triangle.TriangleState.RightUp:
					scaleTransform.ScaleX = scaleTransform.ScaleY = -1;
					break;
			}
			Image.RenderTransform = scaleTransform;
		}

		public RigidbodyUC()
		{
			InitializeComponent();
		}

		public enum RigidbodyType
		{
			Ball,
			Destroyer,
			Rectangle,
			Triangle,
			Circle,
		}
	}
}
