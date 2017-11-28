using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizmoBall.Physics
{
	public class Ball : Circle
	{
		public override object Clone()
		{
			Ball ret = new Ball()
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
