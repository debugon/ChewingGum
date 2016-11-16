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


namespace ChewingGum
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MenuComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region　フィールド

        /// <summary>
        /// グラフィック
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// テクスチャ：非選択状態
        /// </summary>
        private Texture2D menuStartTexture;
        private Texture2D menuOptionTexture;
        private Texture2D menuExitTexture;

        /// <summary>
        /// テクスチャ：選択状態
        /// </summary>
        private Texture2D menuStartSelectedTexture;
        private Texture2D menuOptionSelectedTexture;
        private Texture2D menuExitSelectedTexture;

        /// <summary>
        /// アイテムポジション
        /// </summary>
        private Vector2 position1;
        private Vector2 position2;
        private Vector2 position3;

        /// <summary>
        /// メニューアイテム
        /// </summary>
        public enum Menu
        {
            Start,
            Option,
            Exit
        }

        /// <summary>
        /// 選択状態
        /// </summary>
        Menu menu = Menu.Start;
        bool selected = false;

        #endregion

        public MenuComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            // InputManager初期化
            InputManager.Initialize();

            //メニューアイテムの座標セット
            InitializePosition();

            base.Initialize();
        }

        private void InitializePosition()
        {
            position1 = new Vector2(300.0f, 100.0f);
            position2 = new Vector2(300.0f, 200.0f);
            position3 = new Vector2(300.0f, 300.0f);
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
            // アイテムのテクスチャ読み込み
            LoadMenuItem();
        }

        private void LoadMenuItem()
        {
            menuStartTexture = Game.Content.Load<Texture2D>("MenuItem\\start_white");
            menuOptionTexture = Game.Content.Load<Texture2D>("MenuItem\\option_white");
            menuExitTexture = Game.Content.Load<Texture2D>("MenuItem\\exit_white");

            menuStartSelectedTexture = Game.Content.Load<Texture2D>("MenuItem\\start_yellow");
            menuOptionSelectedTexture = Game.Content.Load<Texture2D>("MenuItem\\option_yellow");
            menuExitSelectedTexture = Game.Content.Load<Texture2D>("MenuItem\\exit_yellow");
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
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (selected)
            {
                selected = false;
            }

            if (InputManager.IsJustKeyDown(Keys.Up) || InputManager.IsJustButtonDown(PlayerIndex.One, Buttons.LeftThumbstickUp))
            {
                if (Menu.Start < menu)
                {
                    menu--;
                }
            }
            else if (InputManager.IsJustKeyDown(Keys.Down) || InputManager.IsJustButtonDown(PlayerIndex.One, Buttons.LeftThumbstickDown))
            {
                if (menu < Menu.Exit)
                {
                    menu++;
                }
            }
            else if (InputManager.IsJustKeyDown(Keys.Enter) || InputManager.IsJustButtonDown(PlayerIndex.One, Buttons.A))
            {
                selected = true;
            }

            //入力取得および更新
            InputManager.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            // スプライト書き込み開始
            spriteBatch.Begin();

            switch (menu)
            {
                case Menu.Start:
                    spriteBatch.Draw(menuStartSelectedTexture, position1, Color.White);
                    spriteBatch.Draw(menuOptionTexture, position2, Color.White);
                    spriteBatch.Draw(menuExitTexture, position3, Color.White);
                    break;

                case Menu.Option:
                    spriteBatch.Draw(menuStartTexture, position1, Color.White);
                    spriteBatch.Draw(menuOptionSelectedTexture, position2, Color.White);
                    spriteBatch.Draw(menuExitTexture, position3, Color.White);
                    break;

                case Menu.Exit:
                    spriteBatch.Draw(menuStartTexture, position1, Color.White);
                    spriteBatch.Draw(menuOptionTexture, position2, Color.White);
                    spriteBatch.Draw(menuExitSelectedTexture, position3, Color.White);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// 選択中のアイテム
        /// </summary>
        public Menu selectedMenu { get { return menu; } }
        public bool IsSelected()
        {
            return selected;
        }
    }
}
