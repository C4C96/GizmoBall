using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizmoBall.Physics
{
    public class Destroyer : Rectangle
    {
		public override object Clone()
		{
			Destroyer ret = new Destroyer()
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
