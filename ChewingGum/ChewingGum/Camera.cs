#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace ChewingGum
{
    /// <summary>
    /// カメラ
    /// </summary>
    public class Camera
    {
        #region フィールド
        /// <summary>
        /// カメラ位置
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
        }
        private Vector3 position = Vector3.Zero;

        /// <summary>
        /// 注視点
        /// </summary>
        public Vector3 Target
        {
            get { return target; }
            set { target = value; isCalc = true; }
        }
        private Vector3 target = Vector3.Zero;

        /// <summary>
        /// ターゲットからどのくらい離れているかを表します
        /// </summary>
        public Vector3 ReferenceTranslate
        {
            get { return referenceTranslate; }
            set { referenceTranslate = value; isCalc = true; }
        }
        private Vector3 referenceTranslate = new Vector3(0.0f, 10.0f, -10.0f);

        /// <summary>
        /// 回転適用済みの位置
        /// </summary>
        public Vector3 TransformedReference
        {
            get { return transformedReference; }
        }
        private Vector3 transformedReference = new Vector3(0.0f, 10.0f, -10.0f);

        /// <summary>
        /// アスペクト比
        /// </summary>
        public float AspectRatio
        {
            get { return aspectRatio; }
            set { aspectRatio = value; isCalc = true; }
        }
        float aspectRatio = 4.0f / 3.0f;

        /// <summary>
        /// 視野角
        /// </summary>
        public float FieldOfView
        {
            get { return fieldOfView; }
            set { fieldOfView = value; isCalc = true; }
        }
        float fieldOfView = MathHelper.ToRadians(45.0f);

        /// <summary>
        /// ニアクリップ面の距離
        /// </summary>
        public float NearPlaneDistance
        {
            get { return nearPlaneDistance; }
            set { nearPlaneDistance = value; isCalc = true; }
        }
        private float nearPlaneDistance = 1.0f;

        /// <summary>
        /// ファークリップ面の距離
        /// </summary>
        public float FarPlaneDistance
        {
            get { return farPlaneDistance; }
            set { farPlaneDistance = value; isCalc = true; }
        }
        private float farPlaneDistance = 10000.0f;

        /// <summary>
        /// ビュー行列
        /// </summary>
        public Matrix View
        {
            get { return view; }
        }
        private Matrix view;

        /// <summary>
        /// 射影行列
        /// </summary>
        public Matrix Projection
        {
            get { return projection; }
        }
        private Matrix projection;

        /// <summary>
        /// 回転量
        /// </summary>
        private Vector3 rotation = Vector3.Zero;

        /// <summary>
        /// カメラの回転行列
        /// </summary>
        private Matrix rotationMatrix = Matrix.Identity;

        /// <summary>
        /// 更新が必要かどうか
        /// </summary>
        private bool isCalc;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Camera()
        {
        }
        #endregion

        #region 初期化
        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            // 最初に更新しておく
            Calculate();
            isCalc = false;
        }
        #endregion

        #region メソッド
        /// <summary>
        /// マトリクスを更新する
        /// </summary>
        private void Calculate()
        {
            // 回転行列を計算する
            rotationMatrix = Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);

            // 回転を適用する
            transformedReference = Vector3.Transform(referenceTranslate, rotationMatrix);

            // ターゲットと距離からカメラの位置を計算する
            position = transformedReference + target;

            // ビュー行列を更新する
            view = Matrix.CreateLookAt(position, target, rotationMatrix.Up);

            // 射影行列を更新する
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        /// <summary>
        /// カメラ移動
        /// </summary>
        /// <param name="vec">移動距離</param>
        public void Move(Vector3 vec)
        {
            // X軸について移動
            target += rotationMatrix.Right * vec.X;
            // Y軸について移動
            target += rotationMatrix.Up * vec.Y;
            // Z軸について移動
            target += rotationMatrix.Forward * vec.Z;

            isCalc = true;
        }

        /// <summary>
        /// カメラの移動
        /// それぞれの軸と平行に移動します
        /// </summary>
        /// <param name="vec"></param>
        public void MoveAxis(Vector3 vec)
        {
            target += vec;
            isCalc = true;
        }

        /// <summary>
        /// カメラの回転
        /// </summary>
        /// <param name="x">X軸周りの回転角度(ラジアン)</param>
        /// <param name="y">Y軸周りの回転角度(ラジアン)</param>
        /// <param name="z">Z軸周りの回転角度(ラジアン)</param>
        public void Rotation(float x, float y, float z)
        {
            // 回転角度を加算する
            rotation.X += x;
            rotation.Y += y;
            rotation.Z += z;

            isCalc = true;
        }

        /// <summary>
        /// カメラの回転
        /// </summary>
        /// <param name="rot">X,Y,Z軸周りの回転角度(ラジアン)</param>
        public void Rotation(Vector3 rot)
        {
            // 回転角度を加算する
            rotation += rot;

            isCalc = true;
        }

        /// <summary>
        /// カメラの回転角度を設定する
        /// </summary>
        /// <param name="vec">X,Y,Z軸周りの回転角度(ラジアン)</param>
        public void SetRotation(Vector3 vec)
        {
            // 回転角度を設定する
            rotation = vec;

            isCalc = true;
        }
        #endregion

        #region アップデート
        /// <summary>
        /// アップデートのタイミングにフレームワークから呼び出されます
        /// </summary>
        /// <param name="gameTime">ゲームタイム</param>
        public void Update(GameTime gameTime)
        {
            // 行列の計算、計算が必要なときのみ処理する
            if (isCalc)
            {
                Calculate();
                isCalc = false;
            }
        }
        #endregion
    }
}