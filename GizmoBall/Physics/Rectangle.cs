using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizmoBall.Physics
{
    class Rectangle : Rigidbody
    {
        //放置时候的旋转，调用一次顺时针旋转90度
        public override void Rotate()
        {
            //把size长宽换一下
            Vector2 tmp = new Vector2(Size.y,Size.x);
            Size = tmp;
        }

        public Rectangle(Vector2 position, Vector2 size, Vector2 speed, float density)
        {
            Position = position;
            Size = size;
            Speed = speed;
            Density = density;
        }
    }
}
