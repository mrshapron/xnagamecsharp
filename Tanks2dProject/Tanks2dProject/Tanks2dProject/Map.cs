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
    
    class Map
    {
        public Texture2D Texture { get; set; }
        public Rectangle Rectangle { get; set; }
        public int Scale { get; set; }
        public Color[] TextureArray { get; set; } 
        public Type[,] TypeMap { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }


        public int CountTurrents = 0;
        public int CountMinions = 0;
        public Vector2 entryPoint;
        public List<Vector2> TurretsLocations = new List<Vector2>();
        public List<Vector2> MinionsAiPositions = new List<Vector2>();
        public bool isEntryPoint = false;

        public Map(Texture2D newTexture)
        {
            this.Scale = Scales.MapScale;
            this.Texture = newTexture;
            this.Width = Texture.Width;
            this.Height = Texture.Height;
            this.Rectangle = new Rectangle(0, 0, this.Width * (int)this.Scale, this.Height * (int)this.Scale);
            this.TextureArray = new Color[this.Width * this.Height];
            this.TypeMap = new Type[this.Width, this.Height];
            Game1.EVENT_DRAW += Draw;
        }

        public void LoadMap()
        {
            this.Texture.GetData(this.TextureArray);
            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < this.Height; j++)
                {
                    if (Color.Equals(this.TextureArray[i + j*this.Width], GroundColors.RoadColor))
                    {
                        this.TypeMap[i, j] = Type.Road;
                    }
                    if (Color.Equals(this.TextureArray[i + j * this.Width], GroundColors.WallColor))
                    {
                        this.TypeMap[i, j] = Type.Wall;
                    }
                    if (Color.Equals(this.TextureArray[i + j * this.Width], GroundColors.GrassColor))
                    {
                        this.TypeMap[i,j] = Type.Grass;
                    }
                    if (Color.Equals((this.TextureArray[i + j * this.Width]), GroundColors.TurretAiColor))
                    {
                        this.TypeMap[i,j] = Type.TurretAi;
                        this.TurretsLocations.Add(new Vector2(i * (float)Scale, j * (float)Scale));
                        this.CountTurrents++;
                    }
                    if (Color.Equals(this.TextureArray[i + j * this.Width], GroundColors.EntryColor))
                    {
                        this.TypeMap[i,j] = TypeMap[i - 1,j];
                        isEntryPoint = true;
                        entryPoint = new Vector2(i * (float)Scale, j * (float)Scale);
                    }
                    if (Color.Equals((this.TextureArray[i + j * this.Width]), GroundColors.MinionsAiColor))
                    {
                        this.TypeMap[i, j] = Type.MinionsAi;
                        this.MinionsAiPositions.Add(new Vector2(i * (float)Scale, j * (float)Scale));
                        this.CountMinions++;
                    }

                }
            }
        }

        public void Draw()
        {
            S.spriteBatch.Draw(this.Texture, this.Rectangle, Color.White);
        }

        public Type CheckPosition(Vector2 pos)
        {
            Type returnType;
            switch (this.TypeMap[(int)pos.X / (int)this.Scale, (int)pos.Y / (int)this.Scale])
            {
                case Type.Wall:
                    returnType = Type.Wall;
                    break;
                case Type.Grass:
                    returnType = Type.Grass;
                    break;
                default:
                    returnType = Type.Road;
                    break;
            }
            return returnType;
        }

        public Vector2 calc_ray_to_wall(Vector2 drc, Vector2 position)
        {
            Vector2 ray = Vector2.Zero;

            int nx = (int)(position.X + ray.X);
            int ny = (int)(position.Y + ray.Y);

            while (this.TypeMap[nx/(int)this.Scale, ny/(int)this.Scale] != Type.Wall)
            {
                ray += drc;
                nx = (int)(position.X + ray.X);
                ny = (int)(position.Y + ray.Y);
            }
            return ray + position;
        }
        public float? find_road_x(float y)
        {
            int yi = (int)(y / this.Scale);
            for (int x = 0; x < this.TypeMap.GetLength(1); x++)
            {
                if (TypeMap[x, yi] == Type.Road)
                {
                    return x * Scale;
                }
            }
            return null;
        }
        public bool isWallClose(Vector2 position, Vector2 lookTo, out Vector2 meetPoint)
        {
            meetPoint = position + lookTo;
            float maxlength = lookTo.Length();
            lookTo.Normalize();
            Vector2 rayPos = position;
            while (this.TypeMap[(int)rayPos.X / (int)Scale, (int)rayPos.Y / (int)Scale] != Type.Wall)
            {
                if ((rayPos - position).Length() > maxlength)
                    return false;
                rayPos += lookTo;
            }
            meetPoint = rayPos;
            return true;
        }
    }
}
