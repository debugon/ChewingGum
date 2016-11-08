#region File Description
//-----------------------------------------------------------------------------
// Keyframe.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace SkinnedModel
{
    /// <summary>
	/// １つのキーフレームの情報を保持するクラス
	/// 時間に対してその時のボーンの位置が保持されます。
    /// </summary>
    public class Keyframe
    {
        #region Fields

        int boneValue;
        TimeSpan timeValue;
        Matrix transformValue;

        #endregion


        /// <summary>
		/// コンストラクタ
        /// </summary>
        public Keyframe(int bone, TimeSpan time, Matrix transform)
        {
            boneValue = bone;
            timeValue = time;
            transformValue = transform;
        }


        /// <summary>
		/// ボーンインデックスの取得
        /// </summary>
        public int Bone
        {
            get { return boneValue; }
        }


        /// <summary>
		/// 時間情報の取得
        /// </summary>
        public TimeSpan Time
        {
            get { return timeValue; }
        }


        /// <summary>
		/// ボーンマトリックスの取得
        /// </summary>
        public Matrix Transform
        {
            get { return transformValue; }
        }
    }
}
