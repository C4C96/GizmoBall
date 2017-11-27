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
            //foreach (var destroyer in destroyers)
            //{
            //    if (Hit(destroyer, ball, deltaTime)) //碰到吸收器，游戏结束
            //    {
            //        return;
            //    }
            //}

            //游戏继续
            ball.Speed = new Vector2(ball.Speed.x, (ball.Speed.y + deltaTime * gravity)); //重力加速度

            foreach (var obstacle in obstacles)
            {
                if(obstacle is Circle)
                {
                    if(HitCircle(ball, obstacle, deltaTime) == true)
                    {
                        MoveCircle(ball, obstacle, deltaTime);
                    }
                }
            }
            ball.Position += ball.Speed * deltaTime / 1000;
            return;
        }

        //检测撞击球形
        private bool HitCircle(Ball ball, Rigidbody circle, int deltaTime)
        {
            float diffdistance = (float)Math.Sqrt((ball.Position.x - circle.Position.x) * (ball.Position.x - circle.Position.x) +
                                                 (ball.Position.y - circle.Position.y) * (ball.Position.y - circle.Position.y));
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
            //向量夹角公式
            Vector2 a = new Vector2(-ball.Speed.x, -ball.Speed.y);
            Vector2 b = new Vector2((ball.Position.x - circle.Position.x), (ball.Position.y - circle.Position.y));
            Vector2 c;
            c.x = (b.Magnitude * ball.Speed.Magnitude * a.x * b.x) / (b.x * a.Magnitude * b.Magnitude);
            c.y = (b.Magnitude * ball.Speed.Magnitude * a.y * b.y) / (b.y * a.Magnitude * b.Magnitude);
            ball.Speed = c;
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
