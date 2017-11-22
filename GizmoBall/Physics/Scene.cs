using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizmoBall.Physics
{
    class Scene
    {
        //地图的尺寸
        readonly Vector2 size;

        //设置好重力加速度
        private float gravity;

        //被撞后是否移动,true 不受碰撞影响
        private bool isKinematic;

        private List<Rigidbody> obstacles = new List<Rigidbody>();

        private Destroyer destroyer;

        private Ball ball;

        private Flipper flipper;

        public float Gravity
        {
            get
            {
                return gravity;
            }
            set
            {
                gravity = value;
            }
        }

        public bool IsKinematic
        {
            get
            {
                return isKinematic;
            }
            set
            {
                isKinematic = value;
            }
        }

        public List<Rigidbody> Obstacles
        {
            get
            {
                return obstacles;
            }
        }

        public Destroyer Destroyer
        {
            get
            {
                return destroyer;
            }
            set
            {
                destroyer = value;
            }
        }

        public Ball Ball
        {
            get
            {
                return ball;
            }
            set
            {
                ball = value;
            }
        }

        public Flipper Flipper
        {
            get
            {
                return flipper;
            }
            set
            {
                flipper = value;
            }
        }

        //用来记录上一帧的时间
        DateTime lastTime = new DateTime();

        public Vector2 Size
        {
            get
            {
                return size;
            }
        }
        

        //调用后进行下一帧的处理，修正速度，修正位置，判断碰撞，碰撞后处理
        public void NextFrame()
        {

        }

        //判断两个障碍物是否相撞
        private bool Hit(Rigidbody rigidbody1 , Rigidbody rigidbody2)
        {
            return false;
        }

        //用来获取本帧和上一帧的时间差，并设置上一帧时间为本帧
        private long TimeDiff()
        {
            long timeDiff = 0;
            DateTime nowTime = DateTime.Now;
            TimeSpan diff = nowTime - lastTime;
            timeDiff = diff.Milliseconds;
            lastTime = nowTime;
            return timeDiff;
        }

        //深度拷贝
        public void ICloneable()
        {

        }

        public Scene()
        {
            size.x = 20;
            size.y = 20;
        }
    }
}
