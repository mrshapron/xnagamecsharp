#region MyRegion
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
#endregion

namespace Tanks2dProject
{
    public static class S
    {
       
        public delegate void UpdateDelegate(GameTime gameTime);
        public delegate void DrawDelegate();
        
        public static ContentManager contentManager;
        public static SpriteBatch spriteBatch;
        public static Vector2 winSize;
        public static GraphicsDevice graphicsDevice;
        public static GraphicsDeviceManager graphicsDeviceManager;
        public static KeyboardState keyboardState, prvKeyboardState;
        public static Texture2D texture;
        public static KeyboardState kbs, prvkbs;
        public static Random rnd = new Random();
        public static Texture2D pointTex;

    

        public static void Init(GraphicsDeviceManager gdm, SpriteBatch spb, ContentManager cm,GraphicsDevice gd, int width, int height)
        {
            S.spriteBatch = spb;
            S.contentManager = cm;
            S.graphicsDeviceManager = gdm;
            S.graphicsDevice = gd;
            UpdateResolution(width, height);
            pointTex = new Texture2D(S.graphicsDevice, 1, 1);
            pointTex.SetData(new Color[1] { Color.White });

        }

        public static void UpdateResolution(int width, int height)
        {
            S.graphicsDeviceManager.PreferredBackBufferHeight = height;
            S.graphicsDeviceManager.PreferredBackBufferWidth = width;
            S.graphicsDeviceManager.ApplyChanges();
            winSize = new Vector2(width, height);
            
        }
        public static void Update()
        {
            #region update keyboard
            S.prvkbs = S.kbs;
            S.kbs = Keyboard.GetState();
            #endregion
        }
        public static void DrawRect(Vector2 p, int size, Color clr)
        {
            S.spriteBatch.Draw(pointTex,
                new Rectangle((int)p.X - size / 2, (int)p.Y - size / 2, size, size),
                clr);

        }

        public static void DrawLine(Vector2 a, Vector2 b, Color clr)
        {
            S.spriteBatch.Draw(
                pointTex,
                a, null, clr, (float)Math.Atan2(b.X - a.X, a.Y - b.Y),
                new Vector2(0.5f, 1),
                new Vector2(1, (a - b).Length()), SpriteEffects.None,
                1);
        }

        public static Texture2D CreateCircle(int radius)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(S.graphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            //width
            for (int i = 0; i < outerRadius; i++)
            {
                int yStart = -1;
                int yEnd = -1;


                //loop through height to find start and end to fill
                for (int j = 0; j < outerRadius; j++)
                {

                    if (yStart == -1)
                    {
                        if (j == outerRadius - 1)
                        {
                            //last row so there is no row below to compare to
                            break;
                        }

                        //start is indicated by Color followed by Transparent
                        if (data[i + (j * outerRadius)] == Color.White && data[i + ((j + 1) * outerRadius)] == Color.Transparent)
                        {
                            yStart = j + 1;
                            continue;
                        }
                    }
                    else if (data[i + (j * outerRadius)] == Color.White)
                    {
                        yEnd = j;
                        break;
                    }
                }

                //if we found a valid start and end position
                if (yStart != -1 && yEnd != -1)
                {
                    //height
                    for (int j = yStart; j < yEnd; j++)
                    {
                        data[i + (j * outerRadius)] = Color.Transparent;
                    }
                }
            }

            texture.SetData(data);
            return texture;
        }

    }

    public static class GroundColors
    {
        public static Color RoadColor = new Color(255, 255, 255);
        public static Color WallColor = new Color(0, 0, 0);
        public static Color GrassColor = new Color(34, 177, 76);
        public static Color EntryColor = new Color(185, 122, 87);
        public static Color TurretAiColor = new Color(237, 28, 36);
        public static Color MinionsAiColor = new Color(100, 100, 100);
    }
    public static class Scales
    {
        public static int MapScale = 2;
        public static int TankScale = 1;
        public static int TurrentScale = 1;
        public static float StartingScaleCamera = 0.8f;
    }
    public static class BulletStat
    {
        public static float MAX_DISTANCE_TANK = 800f;
        public static float MAX_DISTANCE_TURRET = 700f;
    }


    abstract class BaseKeys
    {
        public abstract bool UpKey();
        public abstract bool DownKey();
        public abstract bool RightKey();
        public abstract bool LeftKey();
        public abstract bool LeftTurret();
        public abstract bool RightTurret();
        public abstract bool ShootKey();
    }


    class UserBaseKeys : BaseKeys
    {
        Keys up, down, right, left, leftTurr, rightTurr, shoot;

        #region CTOR
        public UserBaseKeys(Keys up, Keys down, Keys right, Keys left, Keys keyA, Keys keyD, Keys shoot)
        {
            this.up = up;
            this.down = down;
            this.right = right;
            this.left = left;
            this.leftTurr = keyA;
            this.rightTurr = keyD;
            this.shoot = shoot;
        }
        #endregion

        #region bool funcs
        public override bool UpKey()
        {
            return S.kbs.IsKeyDown(up);
        }
        public override bool DownKey()
        {
            return S.kbs.IsKeyDown(down);
        }
        public override bool RightKey()
        {
            return S.kbs.IsKeyDown(right);
        }
        public override bool LeftKey()
        {
            return S.kbs.IsKeyDown(left);
        }
        public override bool LeftTurret()
        {
            return S.kbs.IsKeyDown(leftTurr);
        }
        public override bool RightTurret()
        {
            return S.kbs.IsKeyDown(rightTurr);
        }
        public override bool ShootKey()
        {
            return S.kbs.IsKeyDown(shoot) && S.prvkbs.IsKeyUp(shoot);
        }

        #endregion
    }
    #region " C " : influence general changes
    public static class C
    {
        public static bool debug = true;
    }
    #endregion
}
