using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizmoBall.Physics
{
    class Triangle : Rigidbody
    {
        //0-3分别为左上角，右上角，右下角，左下角
        private int state;

        public int State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        //调用一次顺时针转90度
        public override void Rotate()
        {
            //把state往后调
            if(state>=0 && state <=2)
            {
                state++;
            }
            else
            {
                state = 0;
            }

            //size的边换一下
            Vector2 tmp = new Vector2(Size.y, Size.x);
            Size = tmp;
        }

        public Triangle(Vector2 position, Vector2 size, Vector2 speed, float density, int state)
        {
            Position = position;
            Size = size;
            Speed = speed;
            Density = density;
            State = state;
        }
    }
}
