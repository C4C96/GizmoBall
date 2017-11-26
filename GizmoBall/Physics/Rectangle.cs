using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizmoBall.Physics
{
    public class Rectangle : Rigidbody
    {
		public override List<Vector2> Lines
		{
			get
			{
				List<Vector2> lines = new List<Vector2>
				{
					position,
					position + Vector2.Right * size.x,
					position + size,
					position + Vector2.Down * size.y,
				};
				return lines;
			}
		}
		
		public override void Rotate()
        {

        }

		public override object Clone()
		{
			Rectangle ret = new Rectangle()
			{
				position = this.position,
				size = this.size,
				speed = this.speed,
				density = this.density,
			};
			return ret;
		}
    }
}
