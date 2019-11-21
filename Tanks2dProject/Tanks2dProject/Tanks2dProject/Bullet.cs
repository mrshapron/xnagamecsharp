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
    class Bullet
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Origin { get; set; }
        public bool IsVisible { get; set; }
        public float Rotation { get; set; }
        public float MaxDistance { get; set; }
        public Vector2 StartPosition { get; set; }

        public Vector2 circleActualCenter { get; set; }
        public Vector2 circleCenter { get; set; }
        public float radius { get; set; }
        public TimeSpan Timer { get; set; }

        public Bullet(Texture2D newTexture, Vector2 newOrigin, float newRotation, Vector2 newPosition,float newMaxDistance)
        {
            this.Timer = new TimeSpan();
            this.StartPosition = newPosition;
            this.Texture = newTexture;
            this.IsVisible = true ;
            this.Origin = newOrigin;
            this.Rotation = newRotation;
            this.Position = newPosition;
            this.MaxDistance = newMaxDistance;
            if (this.IsVisible)
                Game1.EVENT_DRAW += Draw;
        }

        public void Draw()
        {
            S.spriteBatch.Draw(this.Texture, this.Position, null, Color.White,this.Rotation
                , this.Origin, 1f, SpriteEffects.None, 0);
            DrawCircle();
        }

        public void DrawCircle()
        {
            Texture2D circle = S.CreateCircle((int)(this.Texture.Height / 2));
            radius = radius;
            circleActualCenter = this.Position + circleCenter;
            S.spriteBatch.Draw(circle, circleActualCenter, null, Color.Blue, 0f, new Vector2(circle.Width / 2, circle.Height / 2), Vector2.One, SpriteEffects.None, 0);
        }

        public static Bullet Shoot(Texture2D newTexture, Vector2 newOrigin, float newRotation, Vector2 newPosition, float
            newMaxDistance)
        {
            Bullet bullet;
            bullet = new Bullet(newTexture, newOrigin, newRotation, newPosition, newMaxDistance);
            bullet.Velocity = new Vector2(
                (float)Math.Cos(newRotation - 0.5 * Math.PI) * (float)Math.PI, (float)Math.Sin(newRotation - 0.5 * Math.PI)
                * (float)Math.PI) * 5f;
            return bullet;
        }
        public void Update()
        {
            this.Position += this.Velocity;
            if (Vector2.Distance(this.Position,this.StartPosition) > this.MaxDistance || this.Velocity == Vector2.Zero)
            {
                this.IsVisible = false;
                Game1.EVENT_DRAW -= Draw;
            }
        }
    }
}
