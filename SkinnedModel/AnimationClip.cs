#region File Description
//-----------------------------------------------------------------------------
// AnimationClip.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace SkinnedModel
{
    /// <summary>
	/// アニメーションに必要な情報を保持するクラス
	/// １つのアニメーションに必要な情報です。
    /// </summary>
    public class AnimationClip
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AnimationClip(String name, TimeSpan duration, IList<Keyframe> keyframes)
        {
			nameValue = name;
            durationValue = duration;
            keyframesValue = keyframes;
        }


        /// <summary>
        /// 合計時間の取得
        /// </summary>
        public TimeSpan Duration
        {
            get { return durationValue; }
        }

        TimeSpan durationValue;


        /// <summary>
		/// キーフレーム配列の取得
		/// 時間によってソートされている必要があります。
        /// </summary>
        public IList<Keyframe> Keyframes
        {
            get { return keyframesValue; }
        }

        IList<Keyframe> keyframesValue;


        /// <summary>
		/// クリップ名の取得
        /// </summary>
		public String Name
		{
			get { return nameValue; }
		}

		String nameValue;
    }
}
