#region File Description
//-----------------------------------------------------------------------------
// TypeReaders.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
#endregion

namespace SkinnedModel
{
    /// <summary>
	/// XNBファイルからSkinningDataの読み込み
    /// </summary>
    public class SkinningDataReader : ContentTypeReader<SkinningData>
    {
        protected override SkinningData Read(ContentReader input, SkinningData existingInstance)
        {
            IDictionary<string, AnimationClip> animationClips;
            IList<Matrix> bindPose, inverseBindPose;
            IList<int> skeletonHierarchy;

            animationClips = input.ReadObject<IDictionary<string, AnimationClip>>();
            bindPose = input.ReadObject<IList<Matrix>>();
            inverseBindPose = input.ReadObject<IList<Matrix>>();
            skeletonHierarchy = input.ReadObject<IList<int>>();

            return new SkinningData(animationClips, bindPose, inverseBindPose, skeletonHierarchy);
        }
    }


    /// <summary>
	/// XNBファイルからAnimationClipの読み込み
    /// </summary>
    public class AnimationClipReader : ContentTypeReader<AnimationClip>
    {
        protected override AnimationClip Read(ContentReader input, AnimationClip existingInstance)
        {
			String name = input.ReadObject<String>();
            TimeSpan duration = input.ReadObject<TimeSpan>();
            IList<Keyframe> keyframes = input.ReadObject < IList<Keyframe>>();

            return new AnimationClip(name, duration, keyframes);
        }
    }


    /// <summary>
	/// XNBファイルからKeyframeの読み込み
    /// </summary>
    public class KeyframeReader : ContentTypeReader<Keyframe>
    {
        protected override Keyframe Read(ContentReader input, Keyframe existingInstance)
        {
            int bone = input.ReadObject<int>();
            TimeSpan time = input.ReadObject<TimeSpan>();
            Matrix transform = input.ReadObject<Matrix>();

            return new Keyframe(bone, time, transform);
        }
    }
}
