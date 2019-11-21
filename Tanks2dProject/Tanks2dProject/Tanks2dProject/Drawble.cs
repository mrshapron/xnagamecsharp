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
    interface IFocus
    {
        Vector2 Position { get; }
        float Rotation { get; }
    }

    class Drawable:IFocus
    {
        #region Data
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle? TexPart { get; set; }
        public float Scale { get; set; }
        public float Layer { get; set; }
        public Color Color { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects Flip { get; set; }

        public Vector2 CircleActualCenter { get; set; }
        public Vector2 CircleCenter { get; set; }
        public float Radius { get; set; }
        private int MaxHp { get;set; }
        public int CurrentHp { get; set; }
        private Vector2 HpPosition { get; set; }
        private Texture2D HpBarTexture = S.contentManager.Load<Texture2D>("SAOHealth");
        #endregion

        public Drawable(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, bool registerDrawEvent)
        {
            this.Texture = texture;
            this.Position = position;
            this.TexPart = sourceRectangle;
            this.Color = color;
            this.Rotation = rotation;
            this.Origin = origin;
            this.Scale = scale;
            this.Flip = effects;
            this.Layer = layerDepth;
            this.HpPosition = new Vector2(this.Position.X + 30, this.Position.Y);
            if (registerDrawEvent)
                Game1.EVENT_DRAW += Draw;
            MaxHp = 100;
            CurrentHp = 100;
        }
        public Drawable(Vector2 position, Vector2 origin, Texture2D texture)
        {
            this.Texture = texture;
            this.Position = position;
            this.TexPart = null;
            this.Color = Color.White;
            this.Rotation = 0f;
            this.Origin = origin;
            this.Scale = 1;
            this.Flip = SpriteEffects.None;
            this.Layer = 0f;
            Game1.EVENT_DRAW += Draw;
        }
        public virtual void Draw()
        {

            if (CurrentHp <= 0)
                return;

            S.spriteBatch.Draw(this.Texture, this.Position, this.TexPart, this.Color, this.Rotation
                , this.Origin, this.Scale, this.Flip, this.Layer);
            DrawHpBar();
            DrawCircle();

        }
        
        public virtual void DrawCircle()
        {
            Texture2D circle = S.CreateCircle((int)(this.Texture.Height / 2) * (int)this.Scale);
            Radius = Radius * Scale;
            CircleActualCenter = this.Position + CircleCenter * Scale;
            S.spriteBatch.Draw(circle, CircleActualCenter, null, Color.Green, 0f, new Vector2(circle.Width / 2, circle.Height / 2), Vector2.One, SpriteEffects.None, 0);
        }
        
        public virtual void DrawHpBar()
        {
            S.spriteBatch.Draw(this.HpBarTexture, new Vector2(this.Position.X, this.Position.Y - 150),
                new Rectangle(-50, 0, (int)(((float)HpBarTexture.Width) * ((float)CurrentHp/(float)MaxHp)), HpBarTexture.Height), this.Color, 0f
                , new Vector2(320,127),0.3f, this.Flip, this.Layer);
        }

        public void ResetProperties() // Using it when we don't need this draw anymore
        {
            this.Position = Vector2.Zero;
            this.CircleActualCenter = Vector2.Zero;
            this.CircleCenter = Vector2.Zero;
            this.Scale = 0;
        }
    }

}
