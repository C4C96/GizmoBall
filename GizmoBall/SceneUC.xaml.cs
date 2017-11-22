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
        public SceneUC()
        {
            InitializeComponent();
			DrawAxis(20, 20);
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
	}
}
