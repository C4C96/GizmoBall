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

        private List<Destroyer> destroyers = new List<Destroyer>();

        private Ball ball;

        private Flipper flipper;

		public event EventHandler BallDestroied; 

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

        public List<Destroyer> Destroyers
        {
            get => destroyers;
        }

        public Ball Ball
        {
            get => ball;
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
            foreach (var destroyer in destroyers)
            {
				if (HitPolygon(ball, destroyer, deltaTime) == true)
				{
					if (BallDestroied != null)
						BallDestroied.Invoke(null, null);
					return;//游戏结束
				}
            }
            if(HitEdge(ball,deltaTime))
            {
                return;
            }

            //游戏继续
            ball.Speed = new Vector2(ball.Speed.x, (ball.Speed.y + deltaTime * gravity)); //重力加速度

            FlipperHit(deltaTime);

            HitPolygon(ball,flipper,deltaTime);

            foreach (var obstacle in obstacles)
            {
                if(obstacle is Circle)
                {
                    if(HitCircle(ball, obstacle, deltaTime) == true)
                    {
                        MoveCircle(ball, obstacle, deltaTime);
                    }
                }
                else
                {
                    HitPolygon(ball, obstacle, deltaTime);
                }
            }
            ball.Position += ball.Speed * deltaTime / 1000;

            return;
        }
        //检测挡板碰撞物体
        private void FlipperHit(int deltaTime)
        {
            //碰撞四条边
            if ((flipper.Position.x + flipper.Speed.x * deltaTime / 1000) < 0)
                return;
            else if ((flipper.Position.y + flipper.Speed.y * deltaTime / 1000) < 0)
                return;
            else if ((flipper.Position.x + flipper.Size.x + flipper.Speed.x * deltaTime / 1000) > 20)
                return;
            else if ((flipper.Position.y + flipper.Size.y + flipper.Speed.y * deltaTime / 1000) > 20)
                return;
            else
            {
                foreach(var obstacle in obstacles)
                {
                    Vector2 diff = obstacle.Center - (flipper.Center + flipper.Speed * deltaTime/1000);
                    if (diff.Magnitude < flipper.Size.x / 2 + obstacle.Size.x/2)
                        return;
                }
            }
            flipper.Position += flipper.Speed * deltaTime / 1000;
                
        }

        //边缘碰撞检测
        private bool HitEdge(Ball ball,int deltaTime)
        {
            bool ischanged = false;
            float diffdistance = 0;
            float distance = 0;
            if(ball.Center.x  <= ball.Size.x/2 && ball.Speed.x<0)  
            {
                ball.Position -= ball.Speed * deltaTime / 1000;
                distance = ball.Speed.x * deltaTime / 1000;
                diffdistance = ball.Position.x;
                ball.Position = new Vector2(distance - diffdistance,ball.Position.y + ball.Speed.y * deltaTime /1000);
                ball.Speed = new Vector2(-ball.Speed.x,ball.Speed.y);
                ischanged = true;
            }
            else if(ball.Center.x >= 20 - ball.Size.x/2 && ball.Speed.x>0)
            {
                ball.Position -= ball.Speed * deltaTime / 1000;
                distance = ball.Speed.x * deltaTime / 1000;
                diffdistance = 20 - ball.Position.x - ball.Size.x;
                ball.Position = new Vector2((20 - (distance - diffdistance) - ball.Size.x), ball.Position.y + ball.Speed.y * deltaTime / 1000);
                ball.Speed = new Vector2(-ball.Speed.x, ball.Speed.y);
                ischanged = true;
            }
            else if(ball.Center.y<= ball.Size.y / 2 && ball.Speed.y < 0)
            {
                ball.Position -= ball.Speed * deltaTime / 1000;
                distance = ball.Speed.y * deltaTime / 1000;
                diffdistance = ball.Position.y;
                ball.Position = new Vector2(ball.Position.x + ball.Speed.x * deltaTime / 1000, distance - diffdistance);
                ball.Speed = new Vector2(ball.Speed.x, -ball.Speed.y);
                ischanged = true;
            }
            else if (ball.Center.y + ball.Speed.y * deltaTime / 1000 >= 20 - ball.Size.y / 2 && ball.Speed.y > 0)
            {
                ball.Position -= ball.Speed * deltaTime / 1000;
                distance = ball.Speed.y * deltaTime / 1000;
                diffdistance = 20 - ball.Position.y - ball.Size.y;
                ball.Position = new Vector2(ball.Position.x + ball.Speed.x * deltaTime / 1000,(20 - (distance - diffdistance) - ball.Size.y));
                ball.Speed = new Vector2(ball.Speed.x, -ball.Speed.y);
                ischanged = true;
            }
            return ischanged;
        }

        //检测撞击球形
        private bool HitCircle(Ball ball, Rigidbody circle, int deltaTime)
        {
            float diffdistance = (float)Math.Sqrt((ball.Center.x - circle.Center.x) * (ball.Center.x - circle.Center.x) +
                                                 (ball.Center.y - circle.Center.y) * (ball.Center.y - circle.Center.y));
            if (diffdistance <= ((ball.Size.x/2) + (circle.Size.x/2)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //撞到球形之后的操作
        private void MoveCircle(Ball ball, Rigidbody circle, int deltaTime)
        {
            Console.WriteLine("现在速度" + ball.Speed);
            Console.WriteLine(circle.Center );
            Console.WriteLine(ball.Center);
            ball.Position -= ball.Speed * deltaTime / 1000;
            //向量夹角公式
            Vector2 a = new Vector2(-ball.Speed.x, -ball.Speed.y);
            Vector2 b = new Vector2((circle.Center.x - ball.Center.x), (circle.Center.y - ball.Center.y));
            Console.WriteLine("b is" + b);
            Vector2 c;
            if(b.x == 0 && b.y!=0)
            {
                c.x = ball.Speed.x;
                c.y = -ball.Speed.y;
            }
            else if(b.y == 0)
            {
                c.x = -ball.Speed.x;
                c.y = ball.Speed.y;
            }
            else
            {
                float k = b.y / b.x;
                c = VectorRebound(a,k);
            }
            ball.Speed = c;
            ball.Position += ball.Speed * deltaTime / 1000;
            
            return;
        }

        //碰撞其他多边形
        private bool HitPolygon(Ball ball,Rigidbody polygon ,int deltaTime)
        {
            Vector2 r1point1 = new Vector2(0,0);
            Vector2 r1point2 = new Vector2(0, 0);
            Vector2 hitpoint = new Vector2(0, 0);
            bool ifhit = false;
            for (int i = 0; i < polygon.Lines.Count; i++)
            {
                r1point1 = polygon.Lines[i];
                if (i == polygon.Lines.Count - 1)
                    r1point2 = polygon.Lines[0];
                else
                    r1point2 = polygon.Lines[i + 1];
                foreach (var ballPoint in ball.Lines)
                {
                    if(GetDistance(ballPoint,r1point1,r1point2) <= ball.Size.x/2)
                    {
                        hitpoint = ballPoint;
                        ifhit = true;
                        break;
                    }
                }
                if (ifhit == true) break;
            }
            if (ifhit == true)
            {
                ball.Position -= ball.Speed * deltaTime / 1000;
                if(r1point1.x == r1point2.x)
                {
                    ball.Speed = new Vector2(-ball.Speed.x,ball.Speed.y);
                }
                else
                {
                    float k = (r1point2.y - r1point1.y) / (r1point2.x - r1point1.x);
                    ball.Speed = VectorRebound(ball.Speed, k);
                }
                ball.Position += ball.Speed * deltaTime/1000;
                return true;
            }
            else
                return false;
        }

        //求一个向量的反弹向量，k法线斜率，a初始向量
        private Vector2 VectorRebound(Vector2 a,float k)
        {
            Vector2 c;
            c.x = ((1 - k * k) * a.x + 2 * k * a.y) / (k * k + 1);
            c.y = ((k * k - 1) * a.y + 2 * k * a.x) / (k * k + 1);
            return c;
        }

        //求点P到线段AB的距离
        private float GetDistance(Vector2 P, Vector2 A, Vector2 B)
        {
            Vector2 AP = P - A;
            Vector2 AB = B - A;
            Vector2 BP = P - B;
            float APcos =(AP.x * AB.x + AP.y * AB.y) / AB.Magnitude;
            if(APcos<0)
            {
                return AP.Magnitude;
            }
            else if(APcos>AB.Magnitude)
            {
                return BP.Magnitude;
            }
            else
            {
                return (float)Math.Sqrt(AP.Magnitude*AP.Magnitude - APcos * APcos);
            }
        }

        public object Clone()
        {
            Scene ret = new Scene((int)size.x, (int)size.y)
            {
                ball = ball == null ? null : this.ball.Clone() as Ball,
                flipper = flipper == null ? null : this.flipper.Clone() as Flipper,
                gravity = this.gravity,
                isKinematic = this.isKinematic
            };
            foreach (var rb in obstacles)
                ret.obstacles.Add(rb.Clone() as Rigidbody);
            foreach (var d in destroyers)
                ret.destroyers.Add(d.Clone() as Destroyer);
            return ret;
        }

        public Scene(int x, int y)
        {
            size = new Vector2(x, y);
        }

		public Scene() : this(20, 20)
		{
		
		}
    }
}
