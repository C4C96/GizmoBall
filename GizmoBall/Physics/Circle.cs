using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizmoBall.Physics
{
    public class Circle : Rigidbody
    {
		public override List<Vector2> Lines
		{
			get
			{
				// 简化成八边形
				float x = size.x, y = size.y;
				float c = (float)Math.Sqrt(3 * x * x + 2 * x * y + 3 * y * y) / 2 - (x + y) / 2;
				float a = (x - c) / 2;
				float b = (y - c) / 2;

				List<Vector2> lines = new List<Vector2>
				{
					position + Vector2.Right * a,
					position + Vector2.Right * (a + c),
					position + Vector2.Right * size.x + Vector2.Down * b,
					position + Vector2.Right * size.x + Vector2.Down * (b + c),
					position + Vector2.Down * size.y + Vector2.Right * (a + c),
					position + Vector2.Down * size.y + Vector2.Right * a,
					position + Vector2.Down * (b + c),
					position + Vector2.Down * b,
				};
				return lines;
			}
		}

		public override object Clone()
		{
			Circle ret = new Circle()
			{
				position = this.position,
				size = this.size,
				speed = this.speed,
				density = this.density,
			};
			return ret;
		}

		public override void Rotate()
		{

		}
	}
}
