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

public delegate void UpdateDelegate(GameTime gameTime);
public delegate void DrawDelegate();

namespace Tanks2dProject
{
    public enum Type { Grass, Road, Wall, EntryPoint, TurretAi, MinionsAi }
    public enum GameState { MainManu, Options, Playing, GameOver }
    public class Game1 : Game
    {
        public static UpdateDelegate EVENT_UPDATE;
        public static DrawDelegate EVENT_DRAW;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Tank tank1;
        Camera camera;
        Map map;
        Vector2 entryPoint;
        Tank[] minions;

        //Starting Menu
        GameState currentGameState = GameState.MainManu;

        public static int ScreenWidth = 1200, ScreenHeight = 720;
        cButton btnPlay;

        //bullets
        //List<Bullet> BulletList = new List<Bullet>(); 
        Turret[] Turrets;
        Tank[] tanks;

        public static string title = "Tank Game";

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            graphics.ApplyChanges();
            
            base.Initialize();
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            S.Init(graphics, spriteBatch ,Content,GraphicsDevice, ScreenWidth, ScreenHeight);


            btnPlay = new cButton(Content.Load<Texture2D>("startButton"), graphics.GraphicsDevice);
            btnPlay.setPosition(new Vector2(ScreenWidth / 2 - 60, ScreenHeight / 2 + 100));
            // Create a new SpriteBatch, which can be used to draw textures.

            
            

            map = new Map(Content.Load<Texture2D>("maps22"));
            map.LoadMap(); // decoding the map

            if (map.isEntryPoint == true)
            {
                entryPoint = map.entryPoint;
            }
            else
            {
                entryPoint = new Vector2(0);
            }

            Turrets = new Turret[map.CountTurrents];
            minions = new Tank[map.CountMinions];

            // tank1 is the player
            tank1 = new Tank(Content.Load<Texture2D>("Tank"), entryPoint,
                    new Vector2(94, 92), 1, 0f, 5, Content.Load<Texture2D>("GunTurret"),
                    Content.Load<Texture2D>("Bullet"),map);
            tank1.BaseKeys = new UserBaseKeys(Keys.Up, Keys.Down, Keys.Right, Keys.Left, Keys.A, Keys.D, Keys.Space);

            tanks = new Tank[1 + map.CountMinions];
            tanks[0] = tank1;

            for(int i = 0; i < map.TurretsLocations.Count(); i++)
            {
                Turrets[i] = new Turret(map.TurretsLocations[i], Content.Load<Texture2D>("TurretAiMini"),
                    new Vector2(82, 127), Scales.TurrentScale, Content.Load<Texture2D>("Bullet"));
            }
            for (int i = 0; i < map.MinionsAiPositions.Count(); i++)
            {
                minions[i] = new Tank(Content.Load<Texture2D>("Tank"), map.MinionsAiPositions[i],
                new Vector2(94, 92), 1, 2f, 5, Content.Load<Texture2D>("GunTurret"),
                Content.Load<Texture2D>("Bullet"),map);
                minions[i].BaseKeys = new BotKeys(minions[i]);
                tanks[i + 1] = minions[i];
            }

            //if there is an entry point it will start there, else it will start at (0,0)
            


        
            
            // Camera focusing on tank1
            camera = new Camera(tank1);
            // TODO: use this.Content to load your game content here
            ControlCenter.Init(Turrets, tanks, map);
 
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            Window.Title = title;

            S.Update();
            
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            switch (currentGameState)
            {
                case GameState.MainManu:
                    if (btnPlay.isClicked == true || S.kbs.IsKeyDown(Keys.Space)) currentGameState = GameState.Playing;
                    btnPlay.Update(mouse);
                    break;
                case GameState.Playing:
                    if (EVENT_UPDATE != null)
                        EVENT_UPDATE(gameTime);
                    break;
                case GameState.GameOver:

                    break;
            }
            ControlCenter.Update();
            if (ControlCenter.isGameOver()) currentGameState = GameState.GameOver;
            base.Update(gameTime);
        }

       

        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightBlue);
            //focusing camera on a the chosen object(tank).
            
            //spriteBatch.Begin();

            switch (currentGameState)
            {
                case GameState.MainManu:
                    GraphicsDevice.Clear(Color.LightBlue);
                    spriteBatch.Begin();
                    spriteBatch.Draw(Content.Load<Texture2D>("MainManu"), new Rectangle(0, 0, ScreenWidth, ScreenHeight),Color.White);
                    btnPlay.Draw();
                    spriteBatch.End();
                    break;
                case GameState.Playing:
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Mat);
                    if (EVENT_DRAW != null)
                        EVENT_DRAW();
                    spriteBatch.End();
                    break;
                case GameState.GameOver:
                    spriteBatch.Begin();
                    spriteBatch.Draw(Content.Load<Texture2D>("gameover"), new Rectangle(0, 0, ScreenWidth, ScreenHeight),Color.White);
                    spriteBatch.End();
                    break;
            }
            
            base.Draw(gameTime);
        }
    }
}
