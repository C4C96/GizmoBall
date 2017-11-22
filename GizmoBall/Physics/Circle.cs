using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizmoBall.Physics
{
    class Circle : Rigidbody
    {
        public override void Rotate()
        {
            
        }

        public Circle(Vector2 position, Vector2 size, Vector2 speed, float density)
        {
            Position = position;
            Size = size;
            Speed = speed;
            Density = density;
        }
    }
}
