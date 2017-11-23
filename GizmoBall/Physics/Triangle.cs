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
		
		public override void Rotate()
        {
			Size = new Vector2(Size.y, Size.x);
			state = (TriangleState)(((int)state + 1) % 4);
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
		private enum TriangleState : int
		{
			LeftUp = 0,
			RightUp,
			RightDown,
			LeftDown,
		}
    }
}
