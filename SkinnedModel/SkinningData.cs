#region File Description
//-----------------------------------------------------------------------------
// SkinningData.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace SkinnedModel
{
    /// <summary>
	/// アニメーションデータをまとめたクラス
	/// ModelクラスのTagプロパティにセットされます。
    /// </summary>
    public class SkinningData
    {
        #region Fields

        IDictionary<string, AnimationClip> animationClipsValue;
        IList<Matrix> bindPoseValue;
        IList<Matrix> inverseBindPoseValue;
        IList<int> skeletonHierarchyValue;

        #endregion


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SkinningData(IDictionary<string, AnimationClip> animationClips,
                            IList<Matrix> bindPose, IList<Matrix> inverseBindPose,
                            IList<int> skeletonHierarchy)
        {
            animationClipsValue = animationClips;
            bindPoseValue = bindPose;
            inverseBindPoseValue = inverseBindPose;
            skeletonHierarchyValue = skeletonHierarchy;
        }


        /// <summary>
		/// アニメーションクリップ配列の取得
		/// クリップ名をキーにAnimationClipを取得します。
        /// </summary>
        public IDictionary<string, AnimationClip> AnimationClips
        {
            get { return animationClipsValue; }
        }


        /// <summary>
		/// ボーンのマトリックス配列の取得
        /// </summary>
        public IList<Matrix> BindPose
        {
            get { return bindPoseValue; }
        }


        /// <summary>
		/// ボーンのインバースマトリックス配列の取得
        /// </summary>
        public IList<Matrix> InverseBindPose
        {
            get { return inverseBindPoseValue; }
        }


        /// <summary>
		/// ボーン階層のインデックス配列の取得
        /// </summary>
        public IList<int> SkeletonHierarchy
        {
            get { return skeletonHierarchyValue; }
        }
    }
}
