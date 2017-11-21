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
        readonly float gravity;

        //被撞后是否移动,true 不受碰撞影响
        readonly bool isKinematic;

        private List<Rigidbody> obstacles = new List<Rigidbody>();

        private Destroyer destroyer;

        private Ball ball;

        private Flipper flipper;

        public Vector2 Size
        {
            get
            {
                return size;
            }
        }

        public float Gravity
        {
            get
            {
                return gravity;
            }
        }

        public bool IsKinematic
        {
            get
            {
                return isKinematic;
            }
        }

        //下一帧的处理，修正速度，修正位置，判断碰撞，碰撞后处理
        public void NextFrame()
        {

        }

        private bool Hit(Rigidbody rigidbody1 , Rigidbody rigidbody2)
        {
            return false;
        }
    }
}
