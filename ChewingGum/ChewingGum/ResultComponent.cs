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
    public class ResultComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region フィールド
        /// <summary>
        /// グラフィック
        /// </summary>
        private SpriteBatch spriteBatch;
        
        /// <summary>
        /// フォント
        /// </summary>
        private SpriteFont font;
        private const int fontSize = 64;

        private Texture2D resultTexture;

        private TimeSpan playTime;

        private ConvertTime convertTime;

        private bool isEnded = false;
        #endregion

        public ResultComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            convertTime = new ConvertTime(game);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            //InputManager初期化
            InputManager.Initialize();

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
            font = Game.Content.Load<SpriteFont>(@"memoFont");
            resultTexture = Game.Content.Load<Texture2D>(@"res\img\result");
            base.LoadContent();
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            if(InputManager.IsJustKeyDown(Keys.Enter) || InputManager.IsJustButtonDown(PlayerIndex.One, Buttons.B))
            {
                isEnded = true;
            }

            InputManager.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            //TimeSpan型の時間から総時間を秒数で取得
            //string型に変換して、さらにint型に変換
            int totalSeconds = Int32.Parse(Math.Floor(playTime.TotalSeconds).ToString());

            //文字数(＝桁数)取得
            int wordCount = totalSeconds.ToString().Length;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            //背景
            spriteBatch.Draw(resultTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            //スコア（秒数）表示
            for (int i = 0; i < wordCount; i++)
            {
                //n番目の桁の数を抽出し、変換する
                string s = (totalSeconds % 10).ToString();
                Texture2D item = convertTime.ToImage(s);

                //桁変更
                totalSeconds /= 10;

                //画面中央に表示
                spriteBatch.Draw(item, new Vector2(GraphicsDevice.Viewport.Width / 2 - item.Width * i, GraphicsDevice.Viewport.Height / 2 + item.Height), Color.White);
            }

            //spriteBatch.DrawString(font, "Congratulation!", Vector2.Zero, Color.White);
            //spriteBatch.DrawString(font, Math.Floor(playTime.TotalSeconds) + "sec", new Vector2(GraphicsDevice.Viewport.Width / 2,  GraphicsDevice.Viewport.Height / 2 + 90), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public bool IsEnded()
        {
            return isEnded;
        }

        public TimeSpan PlayTime { set { playTime = value; } }

    }
}
