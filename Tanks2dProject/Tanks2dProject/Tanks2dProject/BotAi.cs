using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Tanks2dProject
{
    #region AIBaseKeys
     class BotKeys : BaseKeys
    {
        #region BaseKeys
        private bool up = true, down = false, right = false, left = false, leftTurr = false, rightTurr = false,
            shoot = false;
        public override bool UpKey()
        {
            return up;
        }
        public override bool DownKey()
        {
            return down;
        }
        public override bool RightKey()
        {
            return right;
        }
        public override bool LeftKey()
        {
            return left;
        }
        public override bool LeftTurret()
        {
            return leftTurr;
        }
        public override bool RightTurret()
        {
            return rightTurr;
        }
        public override bool ShootKey()
        {
            return shoot;
        }
        #endregion

        private int RESPONCE_TIME = 1;
        private const int SLOW_FRAME_RATE = 10;
        private const float TURN_RAY_RATIO = 2.5f;
        private const float MIN_SPEED = 4;
        private const float MIN_SPEED_4REAL = 2f;
        private const int MIN_AHEAD_DISTANCE = 100;

        private int cMUL_VELOCITY = 20;
        private int cMAX_AHEAD_DISTANCE = 500;

        Vector2 leftCollide, rightCollide, center;

        int slowDown = 0;
        float leftFieldOfViewAngle;
        float rightFieldOfViewAngle;

        float wantedRot = 0;
        public float dRot = 0;

        private Vector2 meetPoint = Vector2.Zero;
        private float LookAheadDistance;

        int wut = 0;
        float rayRotation = MathHelper.ToRadians(20);
        Tank tank;
        float Elapsed;
        float Delay = 1000f;

        public BotKeys(Tank tank)
        {
            this.tank = tank;
            #region calc hayshan oreh
            cMUL_VELOCITY = S.rnd.Next(12, 22);
            //  cMUL_VELOCITY = S.rnd.Next(20, 35);
            cMAX_AHEAD_DISTANCE = cMUL_VELOCITY * 10 + 80;
            #endregion

            Game1.EVENT_UPDATE += Update;
            //Game1.EVENT_DRAW += Draw;
            RESPONCE_TIME = S.rnd.Next(10) + 1;

            
        }

        public void Update(GameTime gameTime)
        {
            #region show or not rays
            if (S.kbs.IsKeyDown(Keys.I))
            {
                up = false;
            }
            else
            {
                up = true;
            }
            #endregion
            #region diagram :
            //two measured points
            //|speed | distance|
            //|5        | 80         |
            //|10       |130         |
            //m = (80-130)/(5-10)
            //y-y0 = m(x-x0)
            //y - 80 = 10x-50
            //y = 10x + 30 
            #endregion
            #region not used
            //LookAheadDistance = car.Velocity.Length() * MUL_VELOCITY
            //    /*this.car.Engine.MaxSpeed*/ + MIN_AHEAD_DISTANCE;

            //if (LookAheadDistance > 150)
            //    LookAheadDistance = 150; 
            #endregion
            #region set & clamp look ahead "distance":
            LookAheadDistance = MathHelper.Clamp(
          tank.Speed * cMUL_VELOCITY + MIN_AHEAD_DISTANCE,
          MIN_AHEAD_DISTANCE, cMAX_AHEAD_DISTANCE);
            #endregion

            tank.Rotation -= (float)(Math.PI / 2);

            if (tank.Speed != 0)
            {
                #region set look ahead vector
                Vector2 lookAhead =
                 Vector2.Transform(Vector2.UnitX,
                 Matrix.CreateRotationZ(tank.Rotation) * LookAheadDistance);
                #endregion
                bool isGoodView = !ControlCenter.map.isWallClose(
                    tank.Position, lookAhead, out meetPoint);
                if (isGoodView)
                {
                    #region CASE GOOD VIEW:
                    leftFieldOfViewAngle
                               = MathHelper.Lerp(((float)S.rnd.NextDouble() / 3 + 0.5f),
                               leftFieldOfViewAngle, 0.8f);
                    //   leftFieldOfViewAngle = 1;
                    rightFieldOfViewAngle
                        = MathHelper.Lerp(((float)S.rnd.NextDouble() / 3 + 0.5f),
                        rightFieldOfViewAngle, 0.8f);
                    //  rightFieldOfViewAngle = 1;
                    #endregion
                }
                else
                {
                    #region BAD VIEW
                    leftFieldOfViewAngle
                               = MathHelper.Lerp(((float)S.rnd.NextDouble() / 3 + 1f),
                               leftFieldOfViewAngle, 0.8f);
                    // leftFieldOfViewAngle =1f;
                    rightFieldOfViewAngle
                        = MathHelper.Lerp(((float)S.rnd.NextDouble() / 3 + 1f),
                        rightFieldOfViewAngle, 0.8f);
                    // rightFieldOfViewAngle = 1f;
                    #endregion
                }

                Vector2 leftRay, rightRay;
                leftRay = Vector2.Transform(Vector2.UnitX,
                    Matrix.CreateRotationZ(leftFieldOfViewAngle + tank.Rotation));
                rightRay = Vector2.Transform(Vector2.UnitX,
                    Matrix.CreateRotationZ(-rightFieldOfViewAngle + tank.Rotation));

                leftRay.Normalize();
                rightRay.Normalize();

                leftCollide = ControlCenter.map.calc_ray_to_wall(leftRay, tank.Position);
                rightCollide = ControlCenter.map.calc_ray_to_wall(rightRay, tank.Position);

                center = (leftCollide + rightCollide) / 2;
                Vector2 dir = center - tank.Position;

                float rot = MathHelper.WrapAngle(
                    (float)Math.Atan2(dir.Y, dir.X));

                tank.Rotation = MathHelper.WrapAngle(tank.Rotation);

                dRot = MathHelper.WrapAngle(rot - tank.Rotation);


                if (++wut % RESPONCE_TIME == 0)
                {
                    left = dRot < 0;
                    right = dRot > 0;
                }

                float leftCollideLength = (leftCollide - tank.Position).Length();
                float rightCollideLength = (rightCollide - tank.Position).Length();

                slowDown--;

                if ((leftCollideLength / rightCollideLength > TURN_RAY_RATIO
                    || rightCollideLength / leftCollideLength > TURN_RAY_RATIO))
                {
                    slowDown = SLOW_FRAME_RATE;
                }

                if (slowDown > 0 && tank.Speed > MIN_SPEED)
                {
                    up = false;
                }
                if (!isGoodView && tank.Speed > MIN_SPEED_4REAL)
                    down = true;
                else
                    down = false;

            }
            if (Vector2.Distance(tank.Position, ControlCenter.AllTanks[0].Position) < 800)
            {
                Vector2 randPos = new Vector2(ControlCenter.AllTanks[0].Position.X - ControlCenter.AllTanks[0].Texture.Width / 2 + S.rnd.Next(0, ControlCenter.AllTanks[0].Texture.Width),
                    ControlCenter.AllTanks[0].Position.Y - ControlCenter.AllTanks[0].Texture.Height / 2 + S.rnd.Next(0, ControlCenter.AllTanks[0].Texture.Height));
                float MulLengh;
                float leyadLength;
                float tanTempAngle;
                double tempAngle2;
                float perfectRotation = 0;
                // quadrant 1
                if (tank.Position.X < ControlCenter.AllTanks[0].Position.X && tank.Position.Y > ControlCenter.AllTanks[0].Position.Y)
                {
                    MulLengh = Math.Abs(randPos.X - tank.Position.X);
                    leyadLength = Math.Abs(randPos.Y - tank.Position.Y);
                    tanTempAngle = MulLengh / leyadLength;
                    tempAngle2 = Math.Atan(tanTempAngle);
                    perfectRotation = (float)tempAngle2 % (float)(2 * Math.PI);
                }
                // quadrant 3
                else if (tank.Position.X > ControlCenter.AllTanks[0].Position.X && tank.Position.Y < ControlCenter.AllTanks[0].Position.Y)
                {
                    MulLengh = Math.Abs(tank.Position.X - randPos.X);
                    leyadLength = Math.Abs(tank.Position.Y - randPos.Y);
                    tanTempAngle = MulLengh / leyadLength;
                    tempAngle2 = Math.Atan(tanTempAngle);
                    tempAngle2 += 1 * Math.PI;
                    perfectRotation = (float)tempAngle2 % (float)(2 * Math.PI);
                }
                // quadrant 2
                else if (tank.Position.X < ControlCenter.AllTanks[0].Position.X && tank.Position.Y < ControlCenter.AllTanks[0].Position.Y)
                {
                    MulLengh = Math.Abs(tank.Position.X - randPos.X);
                    leyadLength = Math.Abs(tank.Position.Y - randPos.Y);
                    tanTempAngle = leyadLength / MulLengh;
                    tempAngle2 = Math.Atan(tanTempAngle);
                    tempAngle2 += 0.5 * Math.PI;
                    perfectRotation = (float)tempAngle2 % (float)(2 * Math.PI);
                }
                // quadrant 4
                else if (tank.Position.X > ControlCenter.AllTanks[0].Position.X && tank.Position.Y > ControlCenter.AllTanks[0].Position.Y)
                {
                    MulLengh = Math.Abs(tank.Position.X - randPos.X);
                    leyadLength = Math.Abs(tank.Position.Y - randPos.Y);
                    tanTempAngle = leyadLength / MulLengh;
                    tempAngle2 = Math.Atan(tanTempAngle);
                    tempAngle2 += 1.5 * Math.PI;
                    perfectRotation = (float)tempAngle2 % (float)(2 * Math.PI);
                }

                if (perfectRotation > tank.TurretRotation % (2 * Math.PI))
                {
                    tank.TurretRotation += 0.05f;
                }
                else if (perfectRotation < tank.Rotation % (2 * Math.PI))
                {
                    tank.TurretRotation -= 0.05f;
                }
                this.Elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (Elapsed >= Delay)
                {
                    shoot = true;
                    this.Elapsed = 0;
                }
                else
                    shoot = false;

            }
            else
                shoot = false;
            tank.Rotation += (float)(Math.PI / 2);
            //down = right = left = false;
        }

        public void Draw()
        {
            if (C.debug)
            {
                S.DrawLine(tank.Position, leftCollide, Color.Red);
                S.DrawRect(leftCollide, 6, Color.Red);
                S.DrawLine(tank.Position, rightCollide, Color.Yellow);
                S.DrawRect(rightCollide, 6, Color.Yellow);
                S.DrawLine(tank.Position, center, Color.AliceBlue);
                S.DrawLine(tank.Position, meetPoint, Color.Pink);
                S.DrawRect(meetPoint, 8, Color.Blue);
                S.DrawRect(center, 8, Color.AliceBlue);
            }
        }
    }
    #endregion
}
