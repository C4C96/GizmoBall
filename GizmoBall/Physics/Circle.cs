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
				List<Vector2> lines = new List<Vector2>();
				
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
