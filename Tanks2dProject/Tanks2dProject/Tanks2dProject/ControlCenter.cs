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
    static class ControlCenter
    {
        public static Turret[] AllTurrets;
        public static Tank[] AllTanks;
        //public static List<Bullet> AllBullets;
        public static Map map;
        public static void Init(Turret[] turrets, Tank[] tanks, Map maps)
        {
            AllTanks = tanks;
            AllTurrets = turrets;
            map = maps;
        }

        public static void Update()
        {
            #region colision with turrets and tanks
            foreach (Tank tank in AllTanks)
            {

                foreach (Turret turret in AllTurrets)
                {
                    foreach (Bullet bullet in turret.BulletList)
                    {
                        if ((Vector2.Distance(bullet.circleActualCenter, tank.CircleActualCenter)) * (int)Scales.MapScale - 300 < bullet.radius + tank.Radius / tank.Scale)
                        {
                            tank.CurrentHp -= 5;
                            bullet.IsVisible = false;
                            Game1.EVENT_DRAW -= bullet.Draw;
                        }

                    }
                    foreach (Bullet bullet in tank.BulletList)
                    {
                        if ((Vector2.Distance(bullet.circleActualCenter, turret.CircleActualCenter)) * (int)Scales.MapScale - 300 < bullet.radius + turret.Radius / turret.Scale)
                        {
                            turret.CurrentHp -= 10;
                            bullet.IsVisible = false;
                            Game1.EVENT_DRAW -= bullet.Draw;
                        }
                    }

                }
            }
            #endregion
            #region colision between main bullets tank and minions
            int i;
            foreach (Bullet bullet in AllTanks[0].BulletList)
            {
                i = -1;
                foreach (Tank tank in AllTanks)
                {
                    i++;
                    if (i == 0) continue;
                    if ((Vector2.Distance(bullet.circleActualCenter, tank.CircleActualCenter)) * (int)Scales.MapScale - 300 < bullet.radius + tank.Radius / tank.Scale)
                    {
                        tank.CurrentHp -= 10;
                        bullet.IsVisible = false;
                        Game1.EVENT_DRAW -= bullet.Draw;
                    }
                }
            }
            #endregion
            #region colision between minion bullets and main tank
            foreach (Tank minion in AllTanks.Skip(1))
            {
                foreach (Bullet bullet in minion.BulletList)
                {
                    if ((Vector2.Distance(bullet.circleActualCenter, AllTanks[0].CircleActualCenter)) * (int)Scales.MapScale - 300 < bullet.radius + AllTanks[0].Radius / AllTanks[0].Scale)
                    {
                        AllTanks[0].CurrentHp -= 10;
                        bullet.IsVisible = false;
                        Game1.EVENT_DRAW -= bullet.Draw;
                    }
                }
            }
            #endregion
            foreach (Tank tank in AllTanks.Skip(1))
            {
                if(tank.CurrentHp <= 0)
                {
                    tank.ResetProperties();
                    Game1.EVENT_DRAW -= tank.Draw;
                    Game1.EVENT_UPDATE -= tank.Update;
                }
            }
            foreach (Turret turrent in AllTurrets)
            {
                if (turrent.CurrentHp <= 0)
                {
                    turrent.ResetProperties();
                    Game1.EVENT_DRAW -= turrent.Draw;
                    Game1.EVENT_UPDATE -= turrent.Update;
                }
            }

        }
        public static bool isGameOver()
        {
            if (AllTanks[0].CurrentHp <= 0)
                return true;
            return false;
        }
    }
}
