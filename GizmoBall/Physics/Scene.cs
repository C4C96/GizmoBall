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
            foreach (var destroyer in destroyers)
            {
                if (Hit(destroyer, ball, deltaTime)) //碰到吸收器，游戏结束
                {
                    return;
                }
            }
            
            //游戏继续
            ball.Speed = new Vector2(ball.Speed.x,(ball.Speed.y + deltaTime * gravity)); //重力加速度
            // if (isKinematic==false)  障碍物刚体情况
            foreach(var obstacle in obstacles)  //检测小球是否撞
            {
                if(Hit(obstacle,ball,deltaTime) == true)  //小球跟这个相撞了
                {
                    for (int i = 0; i < obstacle.Lines.Count; i++)
                    {
                        Vector2 r1point1 = obstacle.Lines[i];
                        Vector2 r1point2;
                        if (i == obstacle.Lines.Count - 1)
                            r1point2 = obstacle.Lines[0];
                        else
                            r1point2 = obstacle.Lines[i + 1];

                        foreach (var ballPoint in ball.Lines)
                        {
                            if (HitLine(r1point1, r1point2, ballPoint, ball.Speed, deltaTime) != 0)
                            { 
                                Rebound(ballPoint, r1point1, r1point2, deltaTime);
                                return;
                            }
                        }
                    }
                }
            }

			ball.Position += ball.Speed * deltaTime / 1000;
			return;
        }

        //判断两个线段是否相交
        private bool Intersect(Vector2 Apoint1, Vector2 Apoint2, Vector2 Bpoint1, Vector2 Bpoint2)
        {
            if(Apoint1.x == Apoint2.x)  //A平行于y轴
            {
                if (Bpoint1.x == Bpoint2.x)
                    return false;
                else
                {
                    float aB = (Bpoint2.y - Bpoint1.y) / (Bpoint2.x - Bpoint1.x);
                    float bB = (Bpoint1.y - aB * Bpoint1.x);
                    if ((Bpoint1.x - Apoint1.x) * (Bpoint2.x - Apoint1.x) < 0 && (aB * Apoint1.x - Apoint1.y) * (aB * Apoint2.x - Apoint2.y) < 0)
                        return true;
                    else
                        return false;
                }
            }
            else   //y=ax+b
            {
                float aA = (Apoint2.y - Apoint1.y) / (Apoint2.x - Apoint1.x);
                float bA = (Apoint1.y - aA * Apoint1.x);
                if(Bpoint1.x==Bpoint2.x)
                {
                    if ((Apoint1.x - Bpoint1.x) * (Apoint2.x - Bpoint1.x) < 0 && (aA * Bpoint1.x - Bpoint1.y) * (aA * Bpoint2.x - Bpoint2.y) < 0)
                        return true;
                    else
                        return false;
                }
                else
                {
                    float aB = (Bpoint2.y - Bpoint1.y) / (Bpoint2.x - Bpoint1.x);
                    float bB = (Bpoint1.y - aB * Bpoint1.x);
                    if ((aB * Apoint1.x - Apoint1.y) * (aB * Apoint2.x - Apoint2.y) < 0 && (aA * Bpoint1.x - Bpoint1.y) * (aA * Bpoint2.x - Bpoint2.y) < 0)
                        return true;
                    else
                        return false;
                }
            }
        }

        //判断点是否在线上
        private bool OnLine(Vector2 linePoint1, Vector2 linePoint2, Vector2 point)
        {
            if(linePoint1.x == linePoint2.x)
            {
                if (point.x == linePoint1.x && (point.y - linePoint1.y) * (point.y - linePoint2.y) <= 0)
                    return true;
                else
                    return false;
            }
            else
            {
                float a = (linePoint2.y - linePoint1.y) / (linePoint2.x - linePoint1.x);
                float b = (linePoint1.y - a * linePoint1.x);
                if (a * point.x - point.y == 0)  //直线上
                    return true;
                else
                    return false;
            }
        }

        //判断这个点和这条边是否相撞，参数是边的两个点，那个点，那个点的速度 返回0不撞 ， 返回1正好落在线上 ，返回2穿过线
        private int HitLine(Vector2 linePoint1, Vector2 linePoint2, Vector2 point,Vector2 pointSpeed, int deltaTime)
        {
            Vector2 nextPoint = new Vector2(point.x+pointSpeed.x,point.y+pointSpeed.y);  //这一帧小球会落在这
            if (Intersect(linePoint1, linePoint2, point, nextPoint) == false)  //不会撞到
                return 0;
            else if(OnLine(linePoint1,linePoint2,point) == true)  //小球开始的时候在线上
                return 0;
            else if(OnLine(linePoint1,linePoint2,nextPoint) == true) //小球会落在线上
                return 1;
            else
            return 2;
        }

        //判断两个包围盒是否相撞，若相撞再去判断是否图形相撞,true为相撞
        private bool HitBox(Rigidbody rigidbody1, Rigidbody rigidbody2, int deltaTime)
        {
			if (rigidbody1.Position.x + rigidbody1.Size.x < rigidbody2.Position.x)
				return false;
			if (rigidbody1.Position.x > rigidbody2.Position.x + rigidbody2.Size.x)
				return false;
			if (rigidbody1.Position.y + rigidbody1.Size.y < rigidbody2.Position.y)
				return false;
			if (rigidbody1.Position.y > rigidbody2.Position.y + rigidbody2.Size.y)
				return false;
			return true;

			//Vector2 point1 = rigidbody1.Position;
   //         Vector2 point2 = new Vector2(rigidbody1.Position.x+rigidbody1.Size.x,rigidbody1.Position.y);
   //         Vector2 point3 = new Vector2(point2.x,point2.y - rigidbody1.Size.y);
   //         Vector2 point4 = new Vector2(point1.x, point3.y);
   //         foreach(var r2point in rigidbody2.Lines)
   //         {
   //             if (HitLine(point1, point2, r2point, rigidbody2.Speed, deltaTime) != 0) return true;
   //             else if (HitLine(point2, point3, r2point, rigidbody2.Speed, deltaTime) != 0) return true;
   //             else if (HitLine(point3, point4, r2point, rigidbody2.Speed, deltaTime) != 0) return true;
   //             else if (HitLine(point4, point1, r2point, rigidbody2.Speed, deltaTime) != 0) return true;
   //         }
   //         return false;
        }

        //检测相撞
        private bool Hit(Rigidbody rigidbody1, Rigidbody rigidbody2, int deltaTime)
        {
            if (HitBox(rigidbody1, rigidbody2,deltaTime) == false)  //包围盒不撞，那就是不撞了
                return false;
            for(int i =0; i<rigidbody1.Lines.Count; i++)
            {
                Vector2 r1point1 = rigidbody1.Lines[i];
                Vector2 r1point2;
                if(i==rigidbody1.Lines.Count-1)
                    r1point2 = rigidbody1.Lines[0];
                else
                    r1point2 = rigidbody1.Lines[i + 1];

                foreach(var r2point in rigidbody2.Lines)
                {
                    if (HitLine(r1point1,r1point2,r2point,rigidbody2.Speed,deltaTime) != 0)
                        return true;
                }
            }
            return false;
        }

        //小球撞到刚体之后反弹，参数是小球撞进去的那个点，撞到的边，deltatime
        private void Rebound(Vector2 ballPoint, Vector2 linePoint1, Vector2 linePoint2, int deltaTime)
        {
            if(linePoint1.x == linePoint2.x)   //平行于y轴
            {
                if (HitLine(linePoint1, linePoint2, ballPoint, ball.Speed, deltaTime) == 1)   //小球停在线上
                {
                    ball.Position = new Vector2(linePoint1.x, ball.Position.y + ball.Speed.y * deltaTime);
                    ball.Speed = new Vector2(-ball.Speed.x, ball.Speed.y);
                }
                else if (HitLine(linePoint1, linePoint2, ballPoint, ball.Speed, deltaTime) == 2) //小球穿过去了
                {
                    float distance = ball.Speed.x * deltaTime;
                    float diffdistance = linePoint1.x - ball.Position.x;
                    ball.Position = new Vector2(linePoint1.x - (distance - diffdistance), ball.Position.y + ball.Speed.y * deltaTime);
                    ball.Speed = new Vector2(-ball.Speed.x, ball.Speed.y);
                }
                else  //小球开始在线上
                {
                    ball.Position = new Vector2(ball.Position.x + ball.Speed.x * deltaTime, ball.Position.y + ball.Speed.y * deltaTime);
                }
                return;
            }
            else     //y=ax+b
            {
                //夹角公式,k1k2k3为初始速度斜率，法线斜率，弹回来的斜率
                float a = (linePoint1.y - linePoint2.y) / (linePoint1.x - linePoint2.x);
                float b = linePoint1.y - a * linePoint2.x;
                float k1 = ball.Speed.y / ball.Speed.x;
                float k2 = -(1 / a);
                float tmp = (k2 - k1) / (1 + k1 * k2);
                float k3 = (k2 + tmp) / (1 - tmp * k2);
                if (HitLine(linePoint1, linePoint2, ballPoint, ball.Speed, deltaTime) == 1) //小球停在线上
                {
                    float next_x2 = ball.Position.x + ball.Speed.x * deltaTime;
                    float next_y2 = ball.Position.y + ball.Speed.y * deltaTime;
                    ball.Position = new Vector2(next_x2, next_y2);
                    //再给他一段时间看看小球会在哪里
                    float distance = ball.Speed.Magnitude * (deltaTime + deltaTime);  //前进距离
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
                    }
                    else
                    {
                        ball.Speed = new Vector2(-speedx, -speedy);
                    }
                }
                else if(HitLine(linePoint1, linePoint2, ballPoint, ball.Speed, deltaTime) == 2)   //穿过去了
                {
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
                    }
                    else
                    {
                        ball.Position = new Vector2(hitpointx - considerx, hitpointy - considerx * k3);
                        ball.Speed = new Vector2(-speedx, -speedy);
                    }
                }
                else  //小球开始在线上
                {
                    ball.Position = new Vector2(ball.Position.x + ball.Speed.x * deltaTime, ball.Position.y + ball.Speed.y * deltaTime);
                }
                return;
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
    }
}
