using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizmoBall.Physics
{
    public class Scene : ICloneable
    {
        // 地图的尺寸
        private readonly Vector2 size;

        // 设置好重力加速度
        private float gravity;

        // 被撞后是否移动,true 不受碰撞影响
        private bool isKinematic;

        private List<Rigidbody> obstacles = new List<Rigidbody>();

        private Destroyer destroyer;

        private Ball ball;

        private Flipper flipper;

        public float Gravity
        {
            get => gravity;
			set => gravity = value;
        }

        public bool IsKinematic
        {
            get => isKinematic;
            set => isKinematic = value;
        }

        public List<Rigidbody> Obstacles
        {
            get => obstacles;
        }

        public Destroyer Destroyer
        {
            get => destroyer;
            set => destroyer = value;
        }

        public Ball Ball
        {
            get =>  ball;
            set => ball = value;
        }

        public Flipper Flipper
        {
            get => flipper;
            set => flipper = value;
        }
		
        public Vector2 Size
        {
            get => size;
        }

		// 用来记录上一帧的时间
		private DateTime? lastTime;

		// 调用后进行下一帧的处理，修正速度，修正位置，判断碰撞，碰撞后处理
		public void NextFrame()
        {
			if (lastTime == null)
			{
				lastTime = DateTime.Now;
				return;
			}
			DateTime now = DateTime.Now;
			int deltaTime = (now - lastTime.Value).Milliseconds; // 与上一帧的时间差（毫秒）
			lastTime = now;

			// 请开始你的表演
        }

        // 判断两个障碍物是否相撞
        private bool Hit(Rigidbody rigidbody1 , Rigidbody rigidbody2)
        {
            return false;
        }

		public object Clone()
		{
			Scene ret = new Scene((int)size.x, (int)size.y)
			{
				ball = this.ball.Clone() as Ball,
				destroyer = this.destroyer.Clone() as Destroyer,
				flipper = this.flipper.Clone() as Flipper,
				gravity = this.gravity,
				isKinematic = this.isKinematic
			};
			foreach (var rb in obstacles)
				ret.obstacles.Add(rb.Clone() as Rigidbody);
			return ret;
		}

		public Scene(int x, int y)
        {
			size = new Vector2(x, y);
        }
    }
}
