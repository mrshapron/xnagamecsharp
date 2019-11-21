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
    class Turret : Drawable
    {
        public float MaxDistanceBullet { get; set; }
        public List<Bullet> BulletList { get; set; }
        public Texture2D BulletTexture { get; set; }
        private float Elapsed { get; set; }
        private float Delay { get; set; }

        public Turret(Vector2 newPosition, Texture2D newTexture, Vector2 newOrigin, int newScale,
            Texture2D newBulletTexture) 
            : base (newTexture,newPosition,null,Color.White,0f,newOrigin,newScale,SpriteEffects.None,0,true)
        {
            this.MaxDistanceBullet = BulletStat.MAX_DISTANCE_TURRET;
            this.BulletTexture = newBulletTexture;
            this.BulletList = new List<Bullet>();
            this.Delay = 1000;
            Game1.EVENT_DRAW += Draw;
            Game1.EVENT_UPDATE += Update;
        }
        public void Update(GameTime gameTime)
        {

            UpdateBullets();
            if (CurrentHp <= 0)
            {
                return;
            }
            

            float perfectRotation = 0;
            // Turret spotted the tank
            if (Vector2.Distance(this.Position, ControlCenter.AllTanks[0].Position) < 800)
            {
                Vector2 randPos = new Vector2(ControlCenter.AllTanks[0].Position.X - ControlCenter.AllTanks[0].Texture.Width / 2 + S.rnd.Next(0, ControlCenter.AllTanks[0].Texture.Width),
                    ControlCenter.AllTanks[0].Position.Y - ControlCenter.AllTanks[0].Texture.Height / 2 + S.rnd.Next(0, ControlCenter.AllTanks[0].Texture.Height));
                float MulLengh;
                float leyadLength;
                float tanTempAngle;
                double tempAngle2;

                // quadrant 1
                if (this.Position.X < ControlCenter.AllTanks[0].Position.X && this.Position.Y > ControlCenter.AllTanks[0].Position.Y)
                {
                    MulLengh = Math.Abs(randPos.X - this.Position.X);
                    leyadLength = Math.Abs(randPos.Y - this.Position.Y);
                    tanTempAngle = MulLengh / leyadLength;
                    tempAngle2 = Math.Atan(tanTempAngle);
                    perfectRotation = (float)tempAngle2 % (float)(2 * Math.PI);
                }
                // quadrant 3
                else if (this.Position.X > ControlCenter.AllTanks[0].Position.X && this.Position.Y < ControlCenter.AllTanks[0].Position.Y)
                {
                    MulLengh = Math.Abs(this.Position.X - randPos.X);
                    leyadLength = Math.Abs(this.Position.Y - randPos.Y);
                    tanTempAngle = MulLengh / leyadLength;
                    tempAngle2 = Math.Atan(tanTempAngle);
                    tempAngle2 += 1 * Math.PI;
                    perfectRotation = (float)tempAngle2 % (float)(2 * Math.PI);
                }
                // quadrant 2
                else if (this.Position.X < ControlCenter.AllTanks[0].Position.X && this.Position.Y < ControlCenter.AllTanks[0].Position.Y)
                {
                    MulLengh = Math.Abs(this.Position.X - randPos.X);
                    leyadLength = Math.Abs(this.Position.Y - randPos.Y);
                    tanTempAngle = leyadLength / MulLengh ;
                    tempAngle2 = Math.Atan(tanTempAngle);
                    tempAngle2 += 0.5 * Math.PI;
                    perfectRotation = (float)tempAngle2 % (float)(2 * Math.PI);
                }
                // quadrant 4
                else if (this.Position.X > ControlCenter.AllTanks[0].Position.X && this.Position.Y > ControlCenter.AllTanks[0].Position.Y)
                {
                    MulLengh = Math.Abs(this.Position.X - randPos.X);
                    leyadLength = Math.Abs(this.Position.Y - randPos.Y);
                    tanTempAngle = leyadLength / MulLengh;
                    tempAngle2 = Math.Atan(tanTempAngle);
                    tempAngle2 += 1.5 * Math.PI;
                    perfectRotation = (float)tempAngle2 % (float)(2 * Math.PI);
                }
                
                if(perfectRotation > this.Rotation % (2 * Math.PI))
                {
                    this.Rotation += 0.05f;
                }
                else if (perfectRotation < this.Rotation % (2 * Math.PI))
                {
                    this.Rotation -= 0.05f;
                }
                this.Elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (Elapsed >= Delay)
                {
                    Shoot();
                    this.Elapsed = 0;
                }

            }
        }

        public void Shoot()
        {
            float rotate1 = this.Rotation - (float)Math.PI / 2;
            float xPositionTank = this.Position.X;
            float yPositionTank = this.Position.Y;
            double sinTankTurrent = Math.Sin(rotate1);
            double cosTankTurrent = Math.Cos(rotate1);
            float tankTurretHeight = this.Texture.Height;
            Vector2 bulletPosition = new Vector2(xPositionTank + (tankTurretHeight - 60) * ((float)cosTankTurrent),
                yPositionTank + (tankTurretHeight - 60) * ((float)sinTankTurrent));

            Bullet b = Bullet.Shoot(this.BulletTexture, new Vector2(12, 12), this.Rotation, bulletPosition, this.MaxDistanceBullet);

            if (this.BulletList.Count() < 20)
            {
                this.BulletList.Add(b);
            }
        }

        public void UpdateBullets()
        {
            // updating every bullet in the list
            foreach (Bullet bullet in this.BulletList)
            {
                bullet.Update();
            }
            // removing bullets that isn't visible from the list (to save memory)
            for (int i = 0; i < this.BulletList.Count; i++)
            {
                if (!this.BulletList[i].IsVisible)
                {
                    this.BulletList.RemoveAt(i);
                    i--;
                }
            }
        }

        public override void DrawCircle()
        {
            Texture2D circle = S.CreateCircle((int)((this.Texture.Height - 55) / 2) * (int)this.Scale);
            Radius = Radius * Scale;
            CircleActualCenter = this.Position + CircleCenter * Scale;
            S.spriteBatch.Draw(circle, CircleActualCenter, null, Color.Green, 0f, new Vector2(circle.Width / 2, circle.Height / 2), Vector2.One, SpriteEffects.None, 0);
        }



    }
}
