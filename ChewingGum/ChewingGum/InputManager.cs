#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace ChewingGum
{
    /// <summary>
    /// 入力を管理します
    /// </summary>
    public class InputManager
    {
        #region フィールド
        /// <summary>
        /// 前回更新時のキーボード情報
        /// </summary>
        private static KeyboardState oldKeyState;
        /// <summary>
        /// 最新のキーボード情報
        /// </summary>
        private static KeyboardState currentKeyState;

        /// <summary>
        /// 最大人数
        /// Xbox360につなぐことのできるコントローラの最大数は4
        /// </summary>
        private const int MaxPlayer = 4;

        /// <summary>
        /// 前回更新時のパッド情報
        /// </summary>
        private static GamePadState[] oldPadState = new GamePadState[MaxPlayer];
        /// <summary>
        /// 最新のパッド情報
        /// </summary>
        private static GamePadState[] currentPadState = new GamePadState[MaxPlayer];
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InputManager()
        {
        }
        #endregion

        #region 初期化
        /// <summary>
        /// 初期化
        /// </summary>
        public static void Initialize()
        {
            // あらかじめ更新しておく
            Update();
        }
        #endregion

        #region アップデート
        /// <summary>
        /// 更新
        /// </summary>
        public static void Update()
        {
            // キーボード入力の更新
            oldKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();

            // パッド入力の更新
            for (int i = 0; i < MaxPlayer; i++)
            {
                oldPadState[i] = currentPadState[i];
                currentPadState[i] = GamePad.GetState(PlayerIndex.One + i);
            }
        }
        #endregion

        #region キーボード入力
        /// <summary>
        /// キーがこのフレームで押されたか？
        /// </summary>
        /// <param name="key">判断するキー</param>
        /// <returns>true:このフレームで押されてる/false:押されていない</returns>
        public static bool IsJustKeyDown(Keys key)
        {
            // 初期値はfalse
            bool down = false;

            // 前回更新時押されておらず、かつ、今回更新時押されていればtrueとする
            if (oldKeyState.IsKeyUp(key) && currentKeyState.IsKeyDown(key))
                down = true;

            // 値を返す
            return down;
        }

        /// <summary>
        /// キーがこのフレームで離されたか？
        /// </summary>
        /// <param name="key">判断するキー</param>
        /// <returns>true:このフレームで離された/false:離されていない</returns>
        public static bool IsJustKeyUp(Keys key)
        {
            // 初期値はfalse
            bool up = false;

            // 前回更新時押されており、かつ、今回更新時押されていなければtrueとする
            if (oldKeyState.IsKeyDown(key) && currentKeyState.IsKeyUp(key))
                up = true;

            // 値を返す
            return up;
        }

        /// <summary>
        /// キーが押されているか？
        /// </summary>
        /// <param name="key">状態を取得したいキー</param>
        /// <returns>true:押されている/false:押されていない</returns>
        public static bool IsKeyDown(Keys key)
        {
            bool isDown = false;

            if (currentKeyState.IsKeyDown(key))
                isDown = true;

            return isDown;
        }

        /// <summary>
        /// キーが離されているか？
        /// </summary>
        /// <param name="key">状態を取得したいキー</param>
        /// <returns>true:離されている/false:離されていない</returns>
        public static bool IsKeyUp(Keys key)
        {
            bool isUp = false;

            if (currentKeyState.IsKeyUp(key))
                isUp = true;

            return isUp;
        }
        #endregion

        #region パッド入力
        /// <summary>
        /// コントローラが繋がっているかどうか？
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <returns>true:繋がっている/false:繋がっていない</returns>
        public static bool IsConnected(PlayerIndex player)
        {
            int index = (int)player;

            // コントローラが繋がっているかどうかを返す
            return currentPadState[index].IsConnected;
        }

        /// <summary>
        /// ボタンが押されているか？
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <param name="button">ボタン</param>
        /// <returns>true:押されている/false:押されていない</returns>
        public static bool IsButtonDown(PlayerIndex player, Buttons button)
        {
            bool pressed = false;
            int index = (int)player;

            // コントローラが繋がっていないときは必ずfalseを返す
            if (!currentPadState[index].IsConnected)
                return false;

            // 押されていればtrueとする
            if (currentPadState[index].IsButtonDown(button))
                pressed = true;

            return pressed;
        }

        /// <summary>
        /// ボタンが離されているか？
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <param name="button">ボタン</param>
        /// <returns>true:離されている/false:離されていない</returns>
        public static bool IsButtonUp(PlayerIndex player, Buttons button)
        {
            bool released = false;
            int index = (int)player;

            // コントローラが繋がっていないときは必ずfalseを返す
            if (!currentPadState[index].IsConnected)
                return false;

            // 押されていなければtrueとする
            if (currentPadState[index].IsButtonUp(button))
                released = true;

            return released;
        }

        /// <summary>
        /// 現在のフレームでボタンが押されたか？
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <param name="button">ボタン</param>
        /// <returns>true:押されている/false:押されていない</returns>
        public static bool IsJustButtonDown(PlayerIndex player, Buttons button)
        {
            bool pressed = false;
            int index = (int)player;

            // コントローラが繋がっていないときは必ずfalseを返す
            if (!currentPadState[index].IsConnected)
                return false;

            // 前回更新時押されておらず、かつ、今回更新時押されていればtrueとする
            if (oldPadState[index].IsButtonUp(button) && currentPadState[index].IsButtonDown(button))
                pressed = true;

            return pressed;
        }

        /// <summary>
        /// 現在のフレームでボタンが離されたか？
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <param name="button">ボタン</param>
        /// <returns>true:離されている/false:離されていない</returns>
        public static bool IsJustButtonUp(PlayerIndex player, Buttons button)
        {
            bool released = false;
            int index = (int)player;

            // コントローラが繋がっていないときは必ずfalseを返す
            if (!currentPadState[index].IsConnected)
                return false;

            // 前回更新時押されておらず、かつ、今回更新時押されていればtrueとする
            if (oldPadState[index].IsButtonDown(button) && currentPadState[index].IsButtonUp(button))
                released = true;

            return released;
        }

        /// <summary>
        /// 左トリガーボタンが指定の値まで押されたか？
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <param name="value">0.0f～1.0fまでの値</param>
        /// <returns>true:押し下げられている/false:押し下げられていない</returns>
        public static bool IsJustLeftTriggerDown(PlayerIndex player, float value)
        {
            bool pressed = false;
            int index = (int)player;

            // コントローラが繋がっていないときは必ずfalseを返す
            if (!currentPadState[index].IsConnected)
                return false;

            // 今回更新時、初めて左トリガーが指定の値まで押し下げられていればtrue
            if (oldPadState[index].Triggers.Left < value && currentPadState[index].Triggers.Left >= value)
                pressed = true;

            return pressed;
        }

        /// <summary>
        /// 右トリガーボタンが指定の値まで押されたか？
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <param name="value">0.0f～1.0fまでの値</param>
        /// <returns>true:押し下げられている/false:押し下げられていない</returns>
        public static bool IsJustRightTriggerDown(PlayerIndex player, float value)
        {
            bool pressed = false;
            int index = (int)player;

            // コントローラが繋がっていないときは必ずfalseを返す
            if (!currentPadState[index].IsConnected)
                return false;

            // 今回更新時、初めて右トリガーが指定の値まで押し下げられていればtrue
            if (oldPadState[index].Triggers.Right < value && currentPadState[index].Triggers.Right >= value)
                pressed = true;

            return pressed;
        }

        /// <summary>
        /// 十字キー上を押したか
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <returns>true:押している/false:押していない</returns>
        public static bool IsJustPressedDPadUp(PlayerIndex player)
        {
            bool pressed = false;
            int index = (int)player;

            // コントローラが繋がっていないときは必ずfalseを返す
            if (!currentPadState[index].IsConnected)
                return false;

            // 前回更新時押されておらず、かつ、今回更新時押されていればtrueとする
            if (oldPadState[index].DPad.Up == ButtonState.Released && currentPadState[index].DPad.Up == ButtonState.Pressed)
                pressed = true;

            return pressed;
        }

        /// <summary>
        /// 十字キー下を押したか
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <returns>true:押している/false:押していない</returns>
        public static bool IsJustPressedDPadDown(PlayerIndex player)
        {
            bool pressed = false;
            int index = (int)player;

            // コントローラが繋がっていないときは必ずfalseを返す
            if (!currentPadState[index].IsConnected)
                return false;

            // 前回更新時押されておらず、かつ、今回更新時押されていればtrueとする
            if (oldPadState[index].DPad.Down == ButtonState.Released && currentPadState[index].DPad.Down == ButtonState.Pressed)
                pressed = true;

            return pressed;
        }

        /// <summary>
        /// 十字キー左を押したか
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <returns>true:押している/false:押していない</returns>
        public static bool IsJustPressedDPadLeft(PlayerIndex player)
        {
            bool pressed = false;
            int index = (int)player;

            // コントローラが繋がっていないときは必ずfalseを返す
            if (!currentPadState[index].IsConnected)
                return false;

            // 前回更新時押されておらず、かつ、今回更新時押されていればtrueとする
            if (oldPadState[index].DPad.Left == ButtonState.Released && currentPadState[index].DPad.Left == ButtonState.Pressed)
                pressed = true;

            return pressed;
        }

        /// <summary>
        /// 十字キー右を押したか
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <returns>true:押している/false:押していない</returns>
        public static bool IsJustPressedDPadRight(PlayerIndex player)
        {
            bool pressed = false;
            int index = (int)player;

            // コントローラが繋がっていないときは必ずfalseを返す
            if (!currentPadState[index].IsConnected)
                return false;

            // 前回更新時押されておらず、かつ、今回更新時押されていればtrueとする
            if (oldPadState[index].DPad.Right == ButtonState.Released && currentPadState[index].DPad.Right == ButtonState.Pressed)
                pressed = true;

            return pressed;
        }

        /// <summary>
        /// 左のアナログスティックを操作したか
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <returns>左スティックのアナログ入力値</returns>
        public static Vector2 GetThumbSticksLeft(PlayerIndex player)
        {
            int index = (int)player;

            // コントローラが繋がっていないときはVector2.Zeroを返す
            if (!currentPadState[index].IsConnected)
                return Vector2.Zero;

            return currentPadState[index].ThumbSticks.Left;
        }

        /// <summary>
        /// 右のアナログスティックを操作したか
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <returns>右スティックのアナログ入力値</returns>
        public static Vector2 GetThumbSticksRight(PlayerIndex player)
        {
            int index = (int)player;

            // コントローラが繋がっていないときはVector2.Zeroを返す
            if (!currentPadState[index].IsConnected)
                return Vector2.Zero;

            return currentPadState[index].ThumbSticks.Right;
        }

        /// <summary>
        /// 左トリガーの入力を取得する
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <returns>左トリガーの値(0.0～1.0)</returns>
        public static float GetLeftTrigger(PlayerIndex player)
        {
            int index = (int)player;

            // コントローラが繋がっていないときは0.0fを返す
            if (!currentPadState[index].IsConnected)
                return 0.0f;

            return currentPadState[index].Triggers.Left;
        }

        /// <summary>
        /// 右トリガーの入力を取得する
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <returns>右トリガーの値(0.0～1.0)</returns>
        public static float GetRightTrigger(PlayerIndex player)
        {
            int index = (int)player;

            // コントローラが繋がっていないときは0.0fを返す
            if (!currentPadState[index].IsConnected)
                return 0.0f;

            return currentPadState[index].Triggers.Right;
        }

        /// <summary>
        /// コントローラの振動を設定する
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        /// <param name="leftMotor">左モーターの強さ</param>
        /// <param name="rightMotor">右モーターの強さ</param>
        public static void SetVibration(PlayerIndex player, float leftMotor, float rightMotor)
        {
            GamePad.SetVibration(player, leftMotor, rightMotor);
        }

        /// <summary>
        /// コントローラの振動をストップする
        /// </summary>
        /// <param name="player">プレイヤーインデックス</param>
        public static void ResetVibration(PlayerIndex player)
        {
            GamePad.SetVibration(player, 0.0f, 0.0f);
        }

        /// <summary>
        /// 全てのコントローラの振動を設定する
        /// </summary>
        /// <param name="leftMotor">左モーターの強さ</param>
        /// <param name="rightMotor">右モーターの強さ</param>
        public static void SetVibrationAll(float leftMotor, float rightMotor)
        {
            GamePad.SetVibration(PlayerIndex.One, leftMotor, rightMotor);
            GamePad.SetVibration(PlayerIndex.Two, leftMotor, rightMotor);
            GamePad.SetVibration(PlayerIndex.Three, leftMotor, rightMotor);
            GamePad.SetVibration(PlayerIndex.Four, leftMotor, rightMotor);
        }

        /// <summary>
        /// 全てのコントローラの振動を停止する
        /// </summary>
        public static void ResetVibrationAll()
        {
            GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
            GamePad.SetVibration(PlayerIndex.Two, 0.0f, 0.0f);
            GamePad.SetVibration(PlayerIndex.Three, 0.0f, 0.0f);
            GamePad.SetVibration(PlayerIndex.Four, 0.0f, 0.0f);
        }

        #endregion
    }
}