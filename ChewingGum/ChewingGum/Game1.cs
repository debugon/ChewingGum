#region using System
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

namespace ChewingGum
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region フィールド

        /// <summary>
        /// グラフィック
        /// </summary>
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// コンポーネント
        /// </summary>
        MenuComponent menuCompo;
        PlayComponent playCompo;
        GuideComponent guideCompo;
        ResultComponent resultCompo;

        /// <summary>
        /// メニューアイテム
        /// </summary>
        enum GameMode
        {
            Menu,
            Play,
            Guide,
            Result
        }

        GameMode mode;

        /// <summary>
        /// ゲームプレイ時間
        /// </summary>
        private TimeSpan menuSelectingTime;
            
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            //ウィンドウサイズ
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            //menuComponent初期化
            menuCompo = new MenuComponent(this);

            mode = GameMode.Menu;

            if (!Components.Contains(menuCompo))
            {
                Components.Add(menuCompo);
            }

            base.Initialize();
        }
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            switch (mode)
            {
                case GameMode.Menu:
                    if (menuCompo.IsSelected())
                    {
                        switch (menuCompo.selectedMenu)
                        {
                            case MenuComponent.Menu.Start:
                                Components.Remove(menuCompo);

                                //PlayComponentを初期化
                                playCompo = new PlayComponent(this);
                                Components.Add(playCompo);

                                //プレイ開始までのプレイ時間を格納
                                playCompo.PlayTime = gameTime.TotalGameTime;
                                mode = GameMode.Play;
                                break;

                            case MenuComponent.Menu.Guide:
                                Components.Remove(menuCompo);

                                guideCompo = new GuideComponent(this);
                                Components.Add(guideCompo);

                                mode = GameMode.Guide;
                                break;

                            case MenuComponent.Menu.Exit:
                                Exit();
                                break;
                        }
                    }
                    break;
                    
                case GameMode.Guide:
                    if (guideCompo.IsEnded())
                    {
                        Components.Remove(guideCompo);
                        Components.Add(menuCompo);

                        mode = GameMode.Menu;
                        break;
                    }
                    break;

                case GameMode.Play:
                    if (playCompo.IsEnded())
                    {
                        Components.Remove(playCompo);

                        resultCompo = new ResultComponent(this);
                        Components.Add(resultCompo);

                        //playCompoで使用したプレイ時間を渡す
                        resultCompo.PlayTime = playCompo.PlayTime;
                        mode = GameMode.Result;
                        break;
                    }
                    break;

                case GameMode.Result:
                    if (resultCompo.IsEnded())
                    {
                        Components.Remove(resultCompo);
                        Components.Add(menuCompo);

                        mode = GameMode.Menu;
                    }
                    break;

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //DrawSkinModel(player, playerTransform, playerWorld);

            base.Draw(gameTime);
        }

        public TimeSpan PlayTime { get { return playCompo.PlayTime; } }
    }
}

