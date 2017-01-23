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
    public class PlayComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region フィールド

        /// <summary>
        /// グラフィック
        /// </summary>
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        
        /// <summary>
        /// プレイタイム
        /// </summary>
        private TimeSpan startTime;
        private TimeSpan playTime;

        /// <summary>
        /// プレイヤー
        /// </summary>
        private const int maxLife = 3;
        private int playerLife = 3;

        /// <summary>
        /// 終了フラグ
        /// </summary>
        private bool isEnded = false;

        private InterfaceComponent interfaceCompo;
        private ConvertTime convertTime;

        #endregion

        public PlayComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            interfaceCompo = new InterfaceComponent(game);
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

            if(InputManager.IsJustKeyDown(Keys.Enter) || InputManager.IsJustButtonDown(PlayerIndex.One, Buttons.A))
            {
                isEnded = true;
                Game.Components.Remove(interfaceCompo);
            }

            if (InputManager.IsJustKeyDown(Keys.Up) || InputManager.IsJustButtonDown(PlayerIndex.One, Buttons.LeftThumbstickUp))
            {
                PlayerLife++;
            }

            if (InputManager.IsJustKeyDown(Keys.Down) || InputManager.IsJustButtonDown(PlayerIndex.One, Buttons.LeftThumbstickDown))
            {
                PlayerLife--;
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
            double totalSeconds =  Math.Floor(playTime.TotalSeconds);
            
            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //spriteBatch.DrawString(font, "GameTime:" + convertTime.ToImage(Math.Floor(playTime.TotalSeconds).ToString()) + "sec", Vector2.Zero, Color.White);

            for (int i = 0; i < totalSeconds.ToString().Length; i++)
            {
                string s = (totalSeconds % 10.0f).ToString();
                totalSeconds /= 10.0f;
                spriteBatch.Draw(convertTime.ToImage(s), Vector2.Zero, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public bool IsEnded()
        {
            return isEnded;
        }

        public TimeSpan PlayTime {
            get
            {
                return playTime;
            }
            set
            {
                startTime = value;
                interfaceCompo.PlayTime = value;
            }
        }

        public int PlayerLife
        {
            get
            {
                return playerLife;
            }
            set
            {
                if (playerLife + value <= maxLife)
                    playerLife += value;
            }
        }
    }
}
