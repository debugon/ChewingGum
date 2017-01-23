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

        #region
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
        
        #endregion

        public InterfaceComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            game.Components.Add(this);
            lifeTexture = game.Content.Load<Texture2D>(@"res\img\InterfaceItem\life");
            
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
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(lifeTexture, new Vector2(100, 200), Color.White);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public TimeSpan PlayTime
        {
            get
            {
                return playTime;
            }
            set
            {
                startTime = value;
            }
        }
    }
}
