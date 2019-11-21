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
    class Camera
    {
        public Matrix Mat { get; set; }
        Vector2 pos;
        Tank focus;
        float scale = Scales.StartingScaleCamera;
        public Camera(Tank focus)
        {
            this.focus = focus;
            Game1.EVENT_UPDATE += Update;
        }
        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                scale += 0.01f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                scale -= 0.01f;
            }
            
            //Limiting the scale change option (0.2f to 0.5f)
            if (scale > 1.5f)
            {
                scale = 1.5f;
            }
            if (scale < 0.5f)
            {
                scale = 0.5f;
            }
            Mat = Matrix.Identity *
                  Matrix.CreateTranslation(-pos.X, -pos.Y, 0) *
                  Matrix.CreateScale(scale) *
                  Matrix.CreateRotationZ(0f) *
                  Matrix.CreateTranslation(Game1.ScreenWidth/2, Game1.ScreenHeight/2, 0);

            pos = Vector2.Lerp(focus.Position, pos, 0.92f);
        }
    }
}
