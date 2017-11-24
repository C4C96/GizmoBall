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

            if (Hit(ball, destroyer)) //碰到吸收器，游戏结束
            {
                return;
            }
            //游戏继续
            ball.Speed = new Vector2(ball.Speed.x,(ball.Speed.y + deltaTime * gravity)); //重力加速度


            if (isKinematic==false)  //障碍物刚体
            {
                foreach (var obstacle in Obstacles)  //小球撞到障碍物
                {
                    if(Hit(ball,obstacle))   
                    {
                        foreach(var ballpoint in ball.Lines)  //找到是哪个点和那条边撞了
                        {
                            for(int i=0;i<obstacle.Lines.Count;i++)
                            {
                                Vector2 obstaclepoint1 = obstacle.Lines[i];  //第一个点
                                Vector2 obstaclepoint2 = new Vector2();   //第二个点
                                if(i!=obstacle.Lines.Count-1)
                                {
                                    obstaclepoint2 = obstacle.Lines[i + 1];
                                }
                                else
                                {
                                    obstaclepoint2 = obstacle.Lines[0];
                                }

                                if (obstaclepoint1.x == obstaclepoint2.x) //该直线是平行于y轴
                                {
                                    foreach (var r2point in ball.Lines)
                                    {
                                        float next_x2 = r2point.x + ball.Speed.x * deltaTime; //小球本帧应该到达的位置
                                        if ((r2point.x - obstaclepoint1.x) * (next_x2 - obstaclepoint1.x) <= 0)      //此时会相撞
                                        {
                                            if (r2point.x == obstaclepoint1.x)//小球刚开始在线上
                                            {
                                                ball.Position = new Vector2(ball.Position.x + ball.Speed.x * deltaTime, ball.Position.y + ball.Speed.y * deltaTime);
                                                break;
                                            }

                                            if(next_x2 == obstaclepoint1.x)  //小球本帧会落在线上
                                            {
                                                ball.Position = new Vector2(obstaclepoint1.x, ball.Position.y + ball.Speed.y * deltaTime);
                                                ball.Speed = new Vector2(-ball.Speed.x, ball.Speed.y);
                                                break;
                                            }

                                            float move_distance = ball.Speed.x * deltaTime;
                                            ball.Speed = new Vector2(-ball.Speed.x,ball.Speed.y);
                                            float diff1= r2point.x - obstaclepoint1.x;      //间距
                                            ball.Position = new Vector2(obstaclepoint1.x + move_distance - diff1, ball.Position.y + ball.Speed.y * deltaTime);
                                            break;
                                        }
                                        
                                    }
                                }
                                else                      //y=ax+b
                                {
                                    float a = (obstaclepoint1.y - obstaclepoint2.y) / (obstaclepoint1.x - obstaclepoint2.x);
                                    float b = obstaclepoint1.y - a * obstaclepoint1.x;
                                    foreach (var r2point in ball.Lines)
                                    {
                                        float next_x2 = r2point.x + ball.Speed.x * deltaTime; //r2本帧应该到达的位置
                                        float next_y2 = r2point.y + ball.Speed.y * deltaTime;
                                        if ((a * r2point.x - r2point.y) * (a * next_x2 - next_y2) <= 0) //此时会相撞
                                        {
                                            if(a * ball.Position.x + b - ball.Position.y == 0)  //小球开始在线上
                                            {
                                                ball.Position = new Vector2(ball.Position.x + ball.Speed.x * deltaTime, ball.Position.y + ball.Speed.y * deltaTime);
                                                break;
                                            }

                                            //现在我们要玩夹角公式了,k1k2k3为初始速度斜率，法线斜率，弹回来的斜率
                                            float k1 = ball.Speed.y / ball.Speed.x;
                                            float k2 = -(1 / a);
                                            float tmp = (k2 - k1) / (1 + k1 * k2);
                                            float k3 = (k2 + tmp) / (1 - tmp * k2);
                                            
                                            
                                            if (a * next_x2 + b - next_y2 == 0)   //小球会落在线上
                                            {
                                                ball.Position = new Vector2(next_x2,next_y2);
                                                //再给他一段时间看看小球会在哪里
                                                float distance = ball.Speed.Magnitude *( deltaTime + deltaTime);  //前进距离
                                                float diff_distance = (float)Math.Sqrt((next_x2 - ball.Position.x) * (next_x2 - ball.Position.x)
                                                                                      + (next_y2 - ball.Position.y) * (next_y2 - ball.Position.y));  //前进到碰撞位置的距离
                                                float after_distance = distance - diff_distance;
                                                //判断反弹向量的xy正负
                                                float considerx = (float)Math.Sqrt((after_distance * after_distance) / (1 + k3 * k3));
                                                float considerx1 = next_x2 + considerx;   //假设他的实际落点在这里
                                                float considery1 = next_y2 + considerx * k3;
                                                float newk = (considery1 - next_y2) / (considerx1 - next_x2); //假设他的实际速度是这个
                                                float speedx = (float)Math.Sqrt((ball.Speed.Magnitude * ball.Speed.Magnitude) / (1 + newk * newk));
                                                float speedy = speedx * newk;
                                                if ((a * considerx1 - considery1) * (a * ball.Position.x - ball.Position.y) > 0)
                                                {
                                                    ball.Speed = new Vector2(speedx, speedy);
                                                    break;
                                                }
                                                else
                                                {
                                                    ball.Speed = new Vector2(-speedx, -speedy);
                                                    break;
                                                }
                                            }
                                            else     //小球没有落在线上
                                            {
                                                //现在计算碰到的那个点
                                                float hitpointx = (ball.Speed.y - ball.Speed.x * k1 - b) / (a - k1);
                                                float hitpointy = (a * hitpointx + b);
                                                //现在看看小球最终会待在哪里
                                                float distance = ball.Speed.Magnitude * deltaTime;  //前进距离
                                                float diff_distance = (float)Math.Sqrt((hitpointx - ball.Position.x) * (hitpointx - ball.Position.x)
                                                                                      + (hitpointy - ball.Position.y) * (hitpointy - ball.Position.y));  //前进到碰撞位置的距离
                                                float after_distance = distance - diff_distance;

                                                //判断反弹向量的xy正负
                                                float considerx = (float)Math.Sqrt((after_distance * after_distance) / (1 + k3 * k3));
                                                float considerx1 = hitpointx + considerx;   //假设他的实际落点在这里
                                                float considery1 = hitpointy + considerx * k3;
                                                float newk = (considery1 - hitpointy) / (considerx1 - hitpointx); //假设他的实际速度是这个
                                                float speedx = (float)Math.Sqrt((ball.Speed.Magnitude * ball.Speed.Magnitude) / (1 + newk * newk));
                                                float speedy = speedx * newk;
                                                if ((a * considerx1 - considery1) * (a * ball.Position.x - ball.Position.y) > 0)
                                                {
                                                    ball.Position = new Vector2(considerx1, considery1);
                                                    ball.Speed = new Vector2(speedx, speedy);
                                                    break;
                                                }
                                                else
                                                {
                                                    ball.Position = new Vector2(hitpointx - considerx, hitpointy - considerx * k3);
                                                    ball.Speed = new Vector2(-speedx, -speedy);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }


            
        }

        //判断两个障碍物是否相撞
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
