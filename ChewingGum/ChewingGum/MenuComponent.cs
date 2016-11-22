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
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        /// <summary>
        /// テクスチャ
        /// </summary>
        private Texture2D titleTexture;
        private Texture2D menuTexture;

        /// <summary>
        /// アイテムテクスチャ：非選択状態
        /// </summary>
        private Texture2D menuStartTexture;
        private Texture2D menuOptionTexture;
        private Texture2D menuExitTexture;

        /// <summary>
        /// アイテムテクスチャ：選択状態
        /// </summary>
        private Texture2D menuStartSelectedTexture;
        private Texture2D menuOptionSelectedTexture;
        private Texture2D menuExitSelectedTexture;

        /// <summary>
        /// アイテムポジション
        /// </summary>
        private Vector2 menuStartPosition;
        private Vector2 menuOptionPosition;
        private Vector2 menuExitPosition;

        /// <summary>
        /// ビデオ
        /// </summary>
        /// 対応ファイルは.wmvのみ
        /// Single CBR, VC-1, DBR無し
        /// 他詳しい仕様は下記参照
        /// https://msdn.microsoft.com/ja-jp/library/dd254869.aspx
        private Video video;
        private VideoPlayer videoPlayer;
        
        //VideoPlayer.GetTexture()の返り値がTexture2Dなので不要
        //private Texture2D videoTexture;

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
            menuStartPosition = new Vector2(360.0f, 450.0f);
            menuOptionPosition = new Vector2(360.0f, 500.0f);
            menuExitPosition = new Vector2(360.0f, 550.0f);
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
            // テクスチャ読み込み
            LoadTexture();
            LoadVideo();
        }

        private void LoadTexture()
        {
            //タイトル
            titleTexture = Game.Content.Load<Texture2D>("img\\title");
            menuTexture = Game.Content.Load<Texture2D>("img\\menu");
                                    
            //メニューアイテム
            menuStartTexture = Game.Content.Load<Texture2D>("img\\MenuItem\\start_white");
            menuOptionTexture = Game.Content.Load<Texture2D>("img\\MenuItem\\option_white");
            menuExitTexture = Game.Content.Load<Texture2D>("img\\MenuItem\\exit_white");

            menuStartSelectedTexture = Game.Content.Load<Texture2D>("img\\MenuItem\\start_yellow");
            menuOptionSelectedTexture = Game.Content.Load<Texture2D>("img\\MenuItem\\option_yellow");
            menuExitSelectedTexture = Game.Content.Load<Texture2D>("img\\MenuItem\\exit_yellow");
        }

        private void LoadVideo()
        {
            //ビデオ読み込み
            video = Game.Content.Load<Video>("test");

            //プレーヤーのインスタンスを作成
            videoPlayer = new VideoPlayer();

            //ループする
            videoPlayer.IsLooped = true;
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

            //選択状態
            if (selected)
            {
                selected = false;
            }

            #region ビデオ再生

            //もし再生が止まっていたら再生
            if (videoPlayer.State == MediaState.Stopped)
                videoPlayer.Play(video);

            #endregion

            #region 入力処理

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

            #endregion

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

            #region ビデオ描画

            //ビデオが再生されていたら描画
            if (videoPlayer.State == MediaState.Playing)
                spriteBatch.Draw(videoPlayer.GetTexture(), Vector2.Zero, Color.White);

            #endregion

            //タイトルロゴ描画
            spriteBatch.Draw(menuTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.Draw(titleTexture, new Rectangle(GraphicsDevice.Viewport.Width / 10, GraphicsDevice.Viewport.Height / 10, GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), Color.White);

            #region メニューアイテム描画

            switch (menu)
            {
                case Menu.Start:
                    spriteBatch.Draw(menuStartSelectedTexture, menuStartPosition, Color.White);
                    spriteBatch.Draw(menuOptionTexture, menuOptionPosition, Color.White);
                    spriteBatch.Draw(menuExitTexture, menuExitPosition, Color.White);
                    break;

                case Menu.Option:
                    spriteBatch.Draw(menuStartTexture, menuStartPosition, Color.White);
                    spriteBatch.Draw(menuOptionSelectedTexture, menuOptionPosition, Color.White);
                    spriteBatch.Draw(menuExitTexture, menuExitPosition, Color.White);
                    break;

                case Menu.Exit:
                    spriteBatch.Draw(menuStartTexture, menuStartPosition, Color.White);
                    spriteBatch.Draw(menuOptionTexture, menuOptionPosition, Color.White);
                    spriteBatch.Draw(menuExitSelectedTexture, menuExitPosition, Color.White);
                    break;
            }

            #endregion

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
