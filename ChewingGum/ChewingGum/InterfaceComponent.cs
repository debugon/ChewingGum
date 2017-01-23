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
    public class InterfaceComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {

        #region フィールド
        /// <summary>
        /// グラフィック
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// プレイタイム
        /// </summary>
        private TimeSpan startTime;
        private TimeSpan playTime;

        private Texture2D lifeTexture;

        private ConvertTime convertTime;
        
        #endregion

        public InterfaceComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            game.Components.Add(this);

            convertTime = new ConvertTime(game);
            //lifeTexture = game.Content.Load<Texture2D>(@"res\img\InterfaceItem\life");
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

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
            playTime = gameTime.TotalGameTime - startTime;

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

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            for (int i = 0; i < wordCount; i++)
            {
                //n番目の桁の数を抽出し、変換する
                string s = (totalSeconds % 10).ToString();
                Texture2D item = convertTime.ToImage(s);

                //桁変更
                totalSeconds /= 10;

                //画面右上に表示
                spriteBatch.Draw(item, new Vector2(GraphicsDevice.Viewport.Width - item.Width * (i + 1), 0), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public TimeSpan PlayTime { get { return playTime; } set { startTime = value; } }
    }
}
