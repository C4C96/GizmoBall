using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizmoBall.Physics
{
    public class Triangle : Rigidbody
    {
		private TriangleState state;

		public TriangleState State
		{
			get => state;
			set => state = value;
		}

		public override List<Vector2> Lines
		{
			get
			{
				List<Vector2> lines = new List<Vector2>();
				if (state != TriangleState.RightDown)
					lines.Add(position);
				if (state != TriangleState.LeftDown)
					lines.Add(position + Vector2.Right * size.x);
				if (state != TriangleState.LeftUp)
					lines.Add(position + size);
				if (state != TriangleState.RightUp)
					lines.Add(position + Vector2.Down * size.y);
				return lines;
			}
		}
        public List<Vector2> Hypotenuse
        {
            get
            {
                List<Vector2> ret = new List<Vector2>();
                switch (state)
                {
                    case TriangleState.RightUp:
                    case TriangleState.LeftDown:
                        ret.Add(position);
                        ret.Add(position + size);
                        break;
                    case TriangleState.RightDown:
                    case TriangleState.LeftUp:
                        ret.Add(position + Vector2.Right * size.x);
                        ret.Add(position + Vector2.Down * size.y);
                        break;
                }
                return ret;
            }
        }


        public override void Rotate()
        {
			state = (TriangleState)(((int)state + 1) % 4);
			OnPropertyChanged("TriangleState");
        }

		public override object Clone()
		{
			Triangle ret = new Triangle()
			{
				position = this.position,
				size = this.size,
				speed = this.speed,
				density = this.density,
				state = this.state,
			};
			return ret;
		}

		/// <summary>
		/// 对于三角形在包围盒内位置的枚举
		/// </summary>
		public enum TriangleState : int
		{			
			LeftDown = 0,
			LeftUp,
			RightUp,
			RightDown,
		}
    }
}
