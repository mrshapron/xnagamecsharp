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
    class Tank : Drawable
    {
        public Texture2D BulletTexture { get; set; }
        public Vector2 Direction { get; set; }
        public float Speed { get; set; }
        public Texture2D TurretTexture { get; set; }
        public float TurretRotation { get; set; }
        public Vector2 TurretOrigin { get; set; }
        public List<Bullet> BulletList { get; set; }
        public float MaxDistanceBullet { get; set; }
        public Map Maps { get; set; }
        public Vector2 OldPosition { get; set; }
        public Texture2D Circle { get;set; }
        public BaseKeys BaseKeys { get; set; }
        
        public Tank(Texture2D newTexture, Vector2 newPosition, Vector2 newOrigin, int newScale,
            float newRotation, float newSpeed, Texture2D newTurret, Texture2D newBulletTex, Map maps)
            : base(newTexture,newPosition,null,Color.White,newRotation,newOrigin,newScale,SpriteEffects.None,0,true)
        {
            this.Speed = newSpeed;
            this.TurretTexture = newTurret;
            this.TurretRotation = 0; // Reset 
            this.TurretOrigin = new Vector2(35,103);
            this.BulletTexture = newBulletTex;
            this.BulletList = new List<Bullet>();
            this.MaxDistanceBullet = BulletStat.MAX_DISTANCE_TANK;
            this.Maps = maps;

            Game1.EVENT_UPDATE += Update;
            Game1.EVENT_DRAW += DrawTurret;
        }

        public void Update(GameTime gameTime)
        {
            if (this.CurrentHp <= 0)
                return;
            UpdateBullets();
            //tank movement
            if (this.BaseKeys.RightKey())
            {
                this.Rotation += 0.04f;
                this.TurretRotation += 0.04f;
            }
            if (this.BaseKeys.LeftKey())
            {
                this.Rotation -= 0.04f;
                this.TurretRotation -= 0.04f;
            }
            if (this.BaseKeys.UpKey()) 
            {
                this.Speed += 1f; 
            }
            if (this.BaseKeys.DownKey())
            {
                this.Speed -= 1f;
            }
            // Turrent rotate
            if (this.BaseKeys.LeftTurret())
            {
                this.TurretRotation += 0.07f;
            }
            if (this.BaseKeys.RightTurret())
            {
                this.TurretRotation -= 0.07f;
            }
            //Limit speed (Forward is faster)
            if (this.Speed > 15f)
                this.Speed = 15f;
            else if (this.Speed < -10)
                this.Speed = -10f;

            // Shooting --> Space
            if (this.BaseKeys.ShootKey())
            {
                //shooting
                Shoot();
                //recoil turret
                recoil();
            }

            

            // friction
            if (!this.BaseKeys.UpKey() && !this.BaseKeys.DownKey())
            {
                if (this.Speed > 0)
                    this.Speed -= 0.2f;
                if (this.Speed < 0)
                    this.Speed += 0.2f;
            }
            this.OldPosition = this.Position;
           
            this.Position += this.Speed * this.Direction / 2;
            switch (Maps.CheckPosition(this.Position))
            {
                case Type.Wall:
                    this.Position = this.OldPosition;
                    if (this.Speed > 0)
                        this.Speed = -10f;
                    else
                        this.Speed = 10f;
                    break;
                case Type.Grass:
                    if (this.Speed > 10f)
                        this.Speed = 10f;
                    break;
            }
 //           int x = (int)this.Position.X + (int)this.Position.Y * (int)Maps.Width;
   //         if (Maps.TypeMap[x] == Type.Wall)
       //         this.Position = this.OldPosition;
            
            this.Direction = Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(Rotation));

        }

        public void Shoot()
        {
            float rotate1 = this.TurretRotation - (float)Math.PI / 2;
            float xPositionTank = this.Position.X;
            float yPositionTank = this.Position.Y;
            double sinTankTurrent = Math.Sin(rotate1);
            double cosTankTurrent = Math.Cos(rotate1);
            float tankTurretHeight = this.TurretTexture.Height;
            Vector2 bulletPosition = new Vector2(xPositionTank + (tankTurretHeight - 35) * ((float)cosTankTurrent),
                yPositionTank + (tankTurretHeight - 35) * ((float)sinTankTurrent));

            Bullet b = Bullet.Shoot(this.BulletTexture,new Vector2(12,12),this.TurretRotation,bulletPosition,this.MaxDistanceBullet);


            BulletList.Add(b);
        }

        public void UpdateBullets()
        {
            // updating every bullet in the list
            foreach (Bullet bullet in BulletList)
            {
                bullet.Update();
            }
            // removing bullets that isn't visible from the list (to save memory)
            for (int i = 0; i < BulletList.Count; i++)
            {
                if (!BulletList[i].IsVisible)
                {
                    BulletList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void DrawTurret()
        {
            if (this.CurrentHp <= 0)
                return;
            
            // drawing the turret of the tank
            S.spriteBatch.Draw(this.TurretTexture, this.Position, null, Color.White, this.TurretRotation,
                this.TurretOrigin, this.Scale, SpriteEffects.None, 0);


            //DrawCircle();
        }



        public void recoil()
        {
            float RemaindRotate = Math.Abs((this.Rotation - this.TurretRotation)) % (float)(2 * Math.PI);
            if (RemaindRotate >= Math.PI / 2 && RemaindRotate <= Math.PI * 1.5f)
                this.Speed += 10f;
            else
                this.Speed -= 10f;
        }
    }
    
}
