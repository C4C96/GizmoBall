using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GizmoBall.Physics
{
    public abstract class Rigidbody
    {
        //position是图形包围盒的左上角的位置
        private Vector2 position;

        //size确定包围盒大小，x长度，y宽度
        private Vector2 size;

        //速度分xy轴
        private Vector2 speed;

        //密度用来计算碰撞动量
        private float density;

        //表示图形的各条边，y=ax+b ，a和b分别为xy
        protected List<Vector2> lines = new List<Vector2>();


        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public Vector2 Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        public Vector2 Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        public float Density
        {
            get
            {
                return density;
            }
            set
            {
                density = value;
            }
        }

        //放置时候的旋转
        public virtual void Rotate()
        {
            
        }

        public Rigidbody()
        {
            position = new Vector2(0,0);
            size = new Vector2(0, 0);
            speed = new Vector2(0, 0);
            density = 0;
        }
	}
}
