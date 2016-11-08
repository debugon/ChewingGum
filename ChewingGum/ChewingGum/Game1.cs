#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkinnedModel;
#endregion

namespace ChewingGum
{
    /// <summary>
    /// 基底 Game クラスから派生した、ゲームのメイン クラスです。
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region フィールド
        /// <summary>
        /// グラフィックスデバイスマネージャー
        /// </summary>
        GraphicsDeviceManager graphics;

        /// <summary>
        /// モデル
        /// </summary>
        private Model model;

        /// <summary>
        /// アニメーションデータ
        /// </summary>
        private SkinningData skinningData;

        /// <summary>
        /// アニメーションプレーヤー
        /// </summary>
        private AnimationPlayer animationPlayer;

        /// <summary>
        /// クリップ名配列のインデックス
        /// </summary>
        int clipIndex;

        /// <summary>
        /// クリップ名配列
        /// これはこのサンプルで使用しているC_Skinman.fbxに組み込まれているものです。
        /// </summary>
        string[] clipNames = { "idle", "walk", "jump", "run", "set", "ready", "winner", "appeal" };

        private enum ClipNames
        {
            idle,
            walk,
            jump,
            run,
            set,
            ready,
            winner,
            appeal
        };


        /// <summary>
        /// アニメーションのループ再生フラグ
        /// </summary>
        bool loopEnable;

        /// <summary>
        /// アニメーションの一時停止フラグ
        /// </summary>
        bool pauseEnable;

        /// <summary>
        /// アニメーションのスローモーション再生速度
        /// １より大きくなるにしたがって再生速度が遅くなります。
        /// </summary>
        int slowMotionOrder;
        int slowMotionCount;

        /// <summary>
        /// カメラ
        /// </summary>
        private Camera camera;

        /// <summary>
        /// ワールド変換行列
        /// </summary>
        private Matrix worldMatrix;

        /// <summary>
        /// 位置
        /// </summary>
        private Vector3 position;

        /// <summary>
        /// 回転量
        /// </summary>
        private Vector3 rotation;

        /// <summary>
        /// スプライトバッチ
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// スプライトフォント
        /// </summary>
        private SpriteFont font;

        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Game1()
        {
            // デバイスマネージャの生成する
            graphics = new GraphicsDeviceManager(this);
            
            // コンテントのディレクトリを"Content"に設定する
            Content.RootDirectory = "Content";
        }
        #endregion

        #region 初期化
        /// <summary>
        /// 初期化のタイミングにフレームワークから呼び出されます
        /// </summary>
        protected override void Initialize()
        {
            // インプットマネージャーの初期化
            InputManager.Initialize();

            // カメラの初期化
            InitializeCamera();

            // アニメーション用データを初期化
            InitializeAnimationValue();

            // 各種座標データを初期化
            InitializeCoordinateValue();

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
            camera.FarPlaneDistance = 5000.0f;
            camera.ReferenceTranslate = new Vector3(0.0f, 0.0f, 300.0f);
            camera.Target = new Vector3(0.0f, 100.0f, 0.0f);
        }

        /// <summary>
        /// アニメーション用の変数を初期化
        /// </summary>
        private void InitializeAnimationValue()
        {
            // クリップ名配列インデックスを初期化
            clipIndex = 0;

            // ループ再生を有効
            loopEnable = true;

            // 一時停止フラグを無効
            pauseEnable = false;

            // スローモーション速度を等速
            slowMotionOrder = 0;
            slowMotionCount = 0;
        }

        /// <summary>
        /// 座標データの初期化
        /// </summary>
        private void InitializeCoordinateValue()
        {
            position = Vector3.Zero;
            rotation = Vector3.Zero;
            worldMatrix = Matrix.Identity;
        }

        #endregion

        #region コンテンツの読み込み処理
        /// <summary>
        /// コンテンツ読み込みのタイミングにフレームワークから呼び出されます
        /// </summary>
        protected override void LoadContent()
        {
            // スプライトバッチの作成
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // スプライトフォントの作成
            font = Content.Load<SpriteFont>(@"SpriteFont1");

            LoadSkinnedModel(@"C_Skinman");
        }

        /// <summary>
        /// スキンモデルの読み込み処理
        /// </summary>
        private void LoadSkinnedModel(string assetName)
        {
            // モデルを読み込む
            model = Content.Load<Model>(assetName);

            // SkinningDataを取得
            skinningData = model.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // AnimationPlayerを作成
            animationPlayer = new AnimationPlayer(skinningData);

            // アニメーションを再生する
            ChangeAnimationClip(clipNames[clipIndex], loopEnable, 0.0f);
        }

        #endregion

        #region コンテンツの解放処理
        /// <summary>
        /// コンテンツ解放のタイミングにフレームワークから呼び出されます
        /// </summary>
        protected override void UnloadContent()
        {
        }
        #endregion

        #region アニメーションの操作
        /// <summary>
        /// アニメーションの切替処理
        /// </summary>
        public void ChangeAnimationClip(string clipName, bool loop, float weight)
        {
            // クリップ名からAnimationClipを取得して再生する
            AnimationClip clip = skinningData.AnimationClips[clipName];

            animationPlayer.StartClip(clip, loop, weight);
        }
        #endregion

        #region ゲームの更新処理
        /// <summary>
        /// アップデートのタイミングにフレームワークから呼び出されます
        /// </summary>
        /// <param name="gameTime">ゲームタイム</param>
        protected override void Update(GameTime gameTime)
        {
            // インプットマネージャーのアップデート
            InputManager.Update();

            // 終了ボタンのチェック
            if (InputManager.IsJustKeyDown(Keys.Escape) || InputManager.IsJustButtonDown(PlayerIndex.One, Buttons.Back))
                Exit();

            // 入力を取得する
            UpdateInput(gameTime);

            // カメラの更新
            camera.Update(gameTime);

            // アニメーションの更新
            UpdateAnimation(gameTime, true, worldMatrix);

            base.Update(gameTime);
        }

        /// <summary>
        /// アニメーションの更新
        /// </summary>
        private void UpdateAnimation(GameTime gameTime, bool relativeToCurrentTime, Matrix transform)
        {
            // 一時停止状態でないか？
            if (pauseEnable)
                return;

            // スローモーションが有効か？
            if (slowMotionOrder > 0)
            {
                if (slowMotionCount > 0)
                {
                    slowMotionCount--;
                    return;
                }
                slowMotionCount = slowMotionOrder;
            }

            // アニメーションの更新
            animationPlayer.Update(gameTime.ElapsedGameTime, true, transform);
        }
        #endregion

        #region 入力による処理
        /// <summary>
        /// 入力による処理
        /// </summary>
        private void UpdateInput(GameTime gameTime)
        {
            // モデルの座標を更新
            UpdateModelCoordinates(gameTime);

            // アニメーションの操作
            UpdateAnimationControl(gameTime);
        }

        /// <summary>
        /// モデルの座標を更新
        /// </summary>
        private void UpdateModelCoordinates(GameTime gameTime)
        {
            // 移動速度として取得
            float velocity = (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.1f;

            // 計算用のベクトル
            Vector3 vec = Vector3.Zero;

            // 左右スティックの入力を取得する
            Vector2 leftStick = Vector2.Zero;
            Vector2 rightStick = Vector2.Zero;

            leftStick = InputManager.GetThumbSticksLeft(PlayerIndex.One);
            rightStick = InputManager.GetThumbSticksRight(PlayerIndex.One);

            // ゲームパッドでの移動操作
            // 上下左右移動
            vec.X = leftStick.X;
            vec.Y = leftStick.Y;

            // 前後移動
            if (InputManager.IsButtonDown(PlayerIndex.One, Buttons.LeftTrigger))
                vec.Z = -1.0f;
            if (InputManager.IsButtonDown(PlayerIndex.One, Buttons.RightTrigger))
                vec.Z = 1.0f;

            // キーボードでの移動操作
            // 前後移動
            if (InputManager.IsKeyDown(Keys.Z))
                vec.Z = -1.0f;
            if (InputManager.IsKeyDown(Keys.X))
                vec.Z = 1.0f;

            // 上下移動
            if (InputManager.IsKeyDown(Keys.W))
                vec.Y = 1.0f;
            if (InputManager.IsKeyDown(Keys.S))
                vec.Y = -1.0f;

            // 左右移動
            if (InputManager.IsKeyDown(Keys.A))
                vec.X = -1.0f;
            if (InputManager.IsKeyDown(Keys.D))
                vec.X = 1.0f;

            // 入力があったときのみ処理する(入力がなければ長さは0.0fとなるため)
            if (vec.Length() > 0.0f)
            {
                // 移動する
                position += vec * velocity;
            }


            // ゲームパッドでの回転操作
            // ＸＹ軸回転
            vec.X = rightStick.Y;
            vec.Y = rightStick.X;
            vec.Z = 0.0f;

            // キーボードでの回転操作
            // Ｙ軸回転
            if (InputManager.IsKeyDown(Keys.J))
                vec.Y = -1.0f;
            if (InputManager.IsKeyDown(Keys.L))
                vec.Y = 1.0f;
            // Ｘ軸回転
            if (InputManager.IsKeyDown(Keys.I))
                vec.X = -1.0f;
            if (InputManager.IsKeyDown(Keys.K))
                vec.X = 1.0f;

            // 入力があったときのみ処理する(入力がなければ長さは0.0fとなるため)
            if (vec.Length() > 0.0f)
            {
                // 入力を正規化する
                vec.Normalize();

                // 回転を加算する
                rotation.X += MathHelper.ToRadians(-vec.X * velocity);
                rotation.Y += MathHelper.ToRadians(-vec.Y * velocity);
                rotation.Z = 0.0f;

                // X軸中心の回転に制限をつける
                if (rotation.X >= MathHelper.ToRadians(90.0f))
                    rotation.X = MathHelper.ToRadians(90.0f);
                if (rotation.X <= MathHelper.ToRadians(-90.0f))
                    rotation.X = MathHelper.ToRadians(-90.0f);
            }


            // 回転行列の作成
            Matrix rotationMatrix = Matrix.CreateRotationX(rotation.X) *
                                    Matrix.CreateRotationY(rotation.Y) *
                                    Matrix.CreateRotationZ(rotation.Z);

            // 平行移動行列の作成
            Matrix translationMatrix = Matrix.CreateTranslation(position);

            // ワールド変換行列を計算する
            // モデルを拡大縮小し、回転した後、指定の位置へ移動する。
            worldMatrix = rotationMatrix * translationMatrix;

            // 初期値に戻す
            if (InputManager.IsJustButtonDown(PlayerIndex.One, Buttons.RightStick) || InputManager.IsJustKeyDown(Keys.R))
            {
                // 各種座標データを初期化
                InitializeCoordinateValue();
            }
        }

        /// <summary>
        /// アニメーションの操作
        /// </summary>
        private void UpdateAnimationControl(GameTime gameTime)
        {
            // スロー再生操作
            if (InputManager.IsJustPressedDPadLeft(PlayerIndex.One) || InputManager.IsJustKeyDown(Keys.Left))
                slowMotionOrder += 1;
            if (InputManager.IsJustPressedDPadRight(PlayerIndex.One) || InputManager.IsJustKeyDown(Keys.Right))
                slowMotionOrder -= 1;

            // 範囲を超えたら初期化
            if (slowMotionOrder <= 0)
            {
                slowMotionOrder = 0;
                slowMotionCount = 0;
            }

            // 一時停止操作
            if (InputManager.IsJustButtonDown(PlayerIndex.One, Buttons.Start) || InputManager.IsJustKeyDown(Keys.V))
                pauseEnable = (pauseEnable) ? false : true;

            // クリップ名変更操作
            if (InputManager.IsJustPressedDPadUp(PlayerIndex.One) || InputManager.IsJustKeyDown(Keys.Up))
            {
                clipIndex++;
                loopEnable = true;

                // ループ再生を禁止するか？
                if (InputManager.IsButtonDown(PlayerIndex.One, Buttons.B) || InputManager.IsKeyDown(Keys.LeftControl))
                    loopEnable = false;

            }
            if (InputManager.IsJustPressedDPadDown(PlayerIndex.One) || InputManager.IsJustKeyDown(Keys.Down))
            {
                clipIndex--;
                loopEnable = true;

                // ループ再生を禁止するか？
                if (InputManager.IsButtonDown(PlayerIndex.One, Buttons.B) || InputManager.IsKeyDown(Keys.LeftControl))
                    loopEnable = false;
            }

            // 範囲を超えたら初期化
            if (clipIndex >= clipNames.Length)
                clipIndex = clipNames.Length - 1;
            if (clipIndex < 0)
                clipIndex = 0;

            // クリップに変更があったか？
            if (animationPlayer.CurrentClip.Name.CompareTo(clipNames[clipIndex]) != 0)
                // クリップを切り替える
                ChangeAnimationClip(clipNames[clipIndex], loopEnable, 0.0f);


            // 初期値に戻す
            if (InputManager.IsJustButtonDown(PlayerIndex.One, Buttons.X) || InputManager.IsJustKeyDown(Keys.N))
            {
                // アニメーション用データを初期化
                InitializeAnimationValue();
                // クリップを切り替える
                ChangeAnimationClip(clipNames[0], true, 0.0f);
            }
        }
        #endregion

        #region ゲームの描画処理
        /// <summary>
        /// 描画のタイミングにフレームワークから呼び出されます
        /// </summary>
        /// <param name="gameTime">ゲームタイム</param>
        protected override void Draw(GameTime gameTime)
        {
            // 背景を塗りつぶす
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);


            Matrix[] bones = animationPlayer.GetSkinTransforms();
            Matrix view = camera.View;
            Matrix projection = camera.Projection;

            // モデルを描画
            foreach (ModelMesh mesh in model.Meshes)
            {
                string name = mesh.Name;
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }

            #region 操作説明等の描画
            float line = 15.0f;
            string clipName = "Clip Name : " + clipNames[clipIndex];

            // フォントを描画
            spriteBatch.Begin();

            // 現在再生中のクリップ名を描画
            spriteBatch.DrawString(font, clipName, new Vector2(10, line), Color.Black);

            // モデルの移動方法を描画
            spriteBatch.DrawString(font, "W/S/A/D Key(LeftStick) : Move(X/Y axis)", new Vector2(10, line * 3), Color.Black);
            spriteBatch.DrawString(font, "Z/X Key(LT/RT) : Move(Z axis)", new Vector2(10, line * 4), Color.Black);
            spriteBatch.DrawString(font, "I/K/J/L Key(RightStick) : Rotation(X/Y axis)", new Vector2(10, line * 5), Color.Black);
            spriteBatch.DrawString(font, "R Key(RightStick-Push) : Reset", new Vector2(10, line * 6), Color.Black);

            // アニメーションの操作方法を描画
            spriteBatch.DrawString(font, "Left Key(DPad-Left) : Slow+", new Vector2(10, line * 8), Color.Black);
            spriteBatch.DrawString(font, "Right Key(DPad-Right) : Slow-", new Vector2(10, line * 9), Color.Black);
            spriteBatch.DrawString(font, "V Key(Start Button) : Pause", new Vector2(10, line * 10), Color.Black);
            spriteBatch.DrawString(font, "Up Key(DPad-Up) : Next Clip(Loop)", new Vector2(10, line * 11), Color.Black);
            spriteBatch.DrawString(font, "Down Key(DPad-Down) : Prev Clip(Loop)", new Vector2(10, line * 12), Color.Black);
            spriteBatch.DrawString(font, "LCtrl+Up Key(B+DPad-Up) : Next Clip(Stop)", new Vector2(10, line * 13), Color.Black);
            spriteBatch.DrawString(font, "LCtrl+Down Key(B+DPad+Down) : Prev Clip(Stop)", new Vector2(10, line * 14), Color.Black);
            spriteBatch.DrawString(font, "N Key(X Button) : Reset", new Vector2(10, line * 15), Color.Black);

            spriteBatch.End();

            // ステートオブジェクトを初期化する
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            #endregion

            base.Draw(gameTime);
        }
        #endregion
    }
}
