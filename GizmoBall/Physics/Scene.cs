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

			foreach(var obstacle1 in Obstacles)
            {
                foreach(var obstacle2 in Obstacles)
                {
                    if (obstacle1 == obstacle2) continue;

                    if(Hit(obstacle1,obstacle2)) //这俩会撞
                    {

                    }
                }
            }
        }

        // 判断两个障碍物是否相撞
        private bool Hit(Rigidbody rigidbody1 , Rigidbody rigidbody2)
        {
            DateTime now = DateTime.Now;
            int deltaTime = (now - lastTime.Value).Milliseconds; // 与上一帧的时间差（毫秒）

            for(int i=0;i<rigidbody1.Lines.Count;i++)
            {
                Vector2 r1point1 = rigidbody1.Lines[i];  //第一个点
                Vector2 r1point2 = new Vector2();  //第二个点
                if(i!=rigidbody1.Lines.Count-1)
                {
                    r1point2 = rigidbody1.Lines[i + 1];
                }
                else
                {
                    r1point2 = rigidbody1.Lines[0];
                }

                if (r1point1.x == r1point2.x) //该直线是平行于y轴
                {
                    float next_x1 = r1point1.x + rigidbody1.Speed.x * deltaTime; //r1本帧应该到达的位置
                    foreach (var r2point in rigidbody2.Lines)
                    {
                        float next_x2 = r2point.x + rigidbody2.Speed.x * deltaTime; //r2本帧应该到达的位置
                        if ((r2point.x - r1point1.x) * (next_x2 - next_x1) <= 0)      //此时会相撞
                        {
                            return true;
                        }
                    }
                }
                else                      //y=ax+b
                {
                    float a = (r1point1.y - r1point2.y) / (r1point1.x - r1point2.x);
                    float b = r1point1.y - a * r1point1.x;
                    float next_b = b + rigidbody1.Speed.y * deltaTime;  //本帧应该变成的b
                    foreach (var r2point in rigidbody2.Lines)
                    {
                        float next_x2 = r2point.x + rigidbody2.Speed.x * deltaTime; //r2本帧应该到达的位置
                        float next_y2 = r2point.y + rigidbody2.Speed.y * deltaTime;
                        if ((a * r2point.x - r2point.y) * (a * next_x2 - next_y2) <= 0) //此时会相撞
                        {
                            return true;
                        }
                    }
                }

            }

            
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
