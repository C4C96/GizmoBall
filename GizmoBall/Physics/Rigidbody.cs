using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GizmoBall.Physics
{
    public abstract class Rigidbody : ICloneable, INotifyPropertyChanged
    {
        // position是图形包围盒的左上角的位置
        protected Vector2 position;

        // size确定包围盒大小，x长度，y宽度
        protected Vector2 size;

        // 速度分xy轴
        protected Vector2 speed;

        // 密度用来计算碰撞动量
        protected float density;

		public event PropertyChangedEventHandler PropertyChanged;

		// 表示图形的各条边
		public abstract List<Vector2> Lines
		{
			get;
		}

        public Vector2 Position
		{
			get => position;
			set
			{
				position = value;
				OnPropertyChanged("Position");
			}
		}

        public Vector2 Size
		{
			get => size;
			set
			{
				size = value;
				OnPropertyChanged("Size");
			}
		}

        public Vector2 Speed
		{
			get => speed;
			set
			{
				speed = value;
				OnPropertyChanged("Speed");
			}
		}

        public float Density
		{
			get => density;
			set
			{
				density = value;
				OnPropertyChanged("Density");
			}
		}

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
        public Vector2 Center
        {
            get
            {
                return position + size / 2;
            }
        }

        // 放置时候的旋转
        public abstract void Rotate();

		// 深度拷贝
		public abstract object Clone();
	}
}
