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

        /// <summary>
        /// メニューアイテム
        /// </summary>
        enum GameMode
        {
            Menu,
            Play
        }

        GameMode mode;


        /// <summary>
        /// PlayerModel
        /// </summary>
        private Model player;
        private Matrix[] playerTransform;
        private Matrix playerWorld;
        private BoundingBox boundingPlayer;
        private Vector3 playerPosition;
        private Vector3 rotation;

        /// <summary>
        /// カメラ
        /// </summary>
        private Camera camera;
        
        #endregion

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
            // TODO: Add your initialization logic here
            
            //ウィンドウサイズ
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            menuCompo = new MenuComponent(this);
            playCompo = new PlayComponent(this);

            mode = GameMode.Menu;

            if (!Components.Contains(menuCompo))
            {
                Components.Add(menuCompo);
            }

            base.Initialize();
        }

        /// <summary>
        /// カメラの初期化
        /// </summary>
        private void InitializeCamera()
        {
            // カメラを生成する
            camera = new Camera();

            // パラメータを設定
            camera.FieldOfView = MathHelper.ToRadians(45.0f);
            camera.AspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
            camera.NearPlaneDistance = 1.0f;
            camera.FarPlaneDistance = 20000.0f;
            camera.ReferenceTranslate = new Vector3(0.0f, 500.0f, 1000.0f);
            camera.Target = new Vector3(0.0f, 100.0f, 0.0f);
            
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
            LoadSkinnedModel(@"ouji_3");
        }

        /// <summary>
        /// スキンモデルの読み込み処理
        /// </summary>
        private void LoadSkinnedModel(string assetName)
        {
            // モデルを読み込む
            player = Content.Load<Model>(assetName);
            playerTransform = new Matrix[player.Bones.Count];
            player.CopyAbsoluteBoneTransformsTo(playerTransform);
            playerPosition = new Vector3(0, 0, 0);
            playerWorld = Matrix.CreateScale(0.1f) * Matrix.CreateTranslation(playerPosition);

            //// SkinningDataを取得
            //skinningData = model.Tag as SkinningData;

            //if (skinningData == null)
            //    throw new InvalidOperationException
            //        ("This model does not contain a SkinningData tag.");

            //// AnimationPlayerを作成
            //animationPlayer = new AnimationPlayer(skinningData);

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
                                Components.Add(playCompo);

                                mode = GameMode.Play;
                                break;

                            case MenuComponent.Menu.Exit:
                                Exit();
                                break;
                        }
                    }
                    break;

                case GameMode.Play:

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

        private void DrawSkinModel(Model model, Matrix[] transforms, Matrix world)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                //AnimationPlayerがないので保留
                //foreach (SkinnedEffect effect in mesh.Effects)
                //{
                //    effect.SetBoneTransforms(animationPlayer.GetSkinTransforms());
                //    effect.View = camera.View;
                //    effect.Projection = camera.Projection;
                //}

                //代わりにDrawModelから流用
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.PreferPerPixelLighting = true;
                    effect.EnableDefaultLighting();

                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.World = transforms[mesh.ParentBone.Index] * world;

                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight1.Enabled = true;
                    effect.DirectionalLight2.Enabled = true;

                }

                mesh.Draw();
            }
        }


    }
}

