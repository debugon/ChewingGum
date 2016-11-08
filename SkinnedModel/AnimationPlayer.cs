#region File Description
//-----------------------------------------------------------------------------
// AnimationPlayer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace SkinnedModel
{
    /// <summary>
	/// スキンモデルのアニメーションを行います
    /// </summary>
    public class AnimationPlayer
    {
        #region Fields

		// 現在のクリップのアニメーション再生に関する情報
        AnimationClip currentClipValue;
        TimeSpan currentTimeValue;
        int currentKeyframe;

		// 現在のクリップに関する各ボーンマトリックス
        Matrix[] boneTransforms;
        Matrix[] worldTransforms;
        Matrix[] skinTransforms;

		// スキンモデルのボーンマトリックスやスケルトン階層データ
        SkinningData skinningDataValue;

		// 前回のクリップのアニメーション再生に関する情報
		AnimationClip previousClipValue;
		TimeSpan previousTimeValue;
		int previousKeyframe;

		// 前回のクリップのボーンマトリックス
		Matrix[] previousBoneTransforms;

		// 補間の際のウェイト（重み）に関する情報
		float interpolationWeight;
		float interpolationWeightCount;

		// ループ再生の許可／禁止
		bool loopEnable;

        #endregion


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AnimationPlayer(SkinningData skinningData)
        {
            if (skinningData == null)
                throw new ArgumentNullException("skinningData");

            skinningDataValue = skinningData;

            boneTransforms = new Matrix[skinningData.BindPose.Count];
            worldTransforms = new Matrix[skinningData.BindPose.Count];
            skinTransforms = new Matrix[skinningData.BindPose.Count];

			previousBoneTransforms = new Matrix[skinningData.BindPose.Count];

			// デフォルトはループ再生を許可
			loopEnable = true;

			interpolationWeight = 0.0f;
			interpolationWeightCount=0.0f;
        }


        /// <summary>
		/// 指定されたアニメーションクリップの再生を開始
		/// ループ再生・補間処理無しの状態で再生されます。
        /// </summary>
        public void StartClip(AnimationClip clip)
        {
			StartClip(clip, true, 0.0f);
        }

        /// <summary>
		/// 指定されたアニメーションクリップの再生を開始
		/// ループ再生の許可／禁止と補間処理のウェイトが指定できます。
        /// </summary>
		public void StartClip(AnimationClip clip, bool loop, float weight)
		{
			if (clip == null)
                throw new ArgumentNullException("clip");

			interpolationWeight = 0.0f;
			interpolationWeightCount = 0.0f;

			// 補間をする場合は前回のデータを保存する
			if (weight > 0.0f)
			{
				//補間時のウェイトを設定
				interpolationWeight = 1.0f;
				interpolationWeightCount = 1.0f / weight;

				// 再生していたアニメーションの情報を保存
				previousClipValue = currentClipValue;
				previousTimeValue = currentTimeValue;
				previousKeyframe = currentKeyframe;

				// 再生していたアニメーションのボーンマトリックスを保存
				boneTransforms.CopyTo(previousBoneTransforms, 0);
			}

			// 指定されたクリップの保存と初期化
            currentClipValue = clip;
            currentTimeValue = TimeSpan.Zero;
            currentKeyframe = 0;

			// 指定されたループ再生の許可／禁止を設定
			loopEnable = loop;

            // ボーンマトリックスをバインドポーズで初期化
            skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
		}


        /// <summary>
        /// 各ボーンマトリックス配列を更新
		/// 最終的にカスタムエフェクト（SkinnedModel.fx）に渡すためのスキニングマトリックス配列を更新します。
        /// </summary>
        public void Update(TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform)
        {
			// 補間中か？
			if (interpolationWeight > 0)
			{
				// 前回と現在のクリップのボーンマトリックスを更新してから補間を行う
				UpdateBoneTransforms(time, 
									 relativeToCurrentTime,
									 ref previousBoneTransforms,
									 ref previousClipValue,
									 ref previousTimeValue,
									 ref previousKeyframe);

				UpdateBoneTransforms(time,
									 relativeToCurrentTime,
									 ref boneTransforms,
									 ref currentClipValue,
									 ref currentTimeValue,
									 ref currentKeyframe);

				UpdateInterpolationBoneTransforms();
			}
			else
			{
				// 現在のクリップのボーンマトリックスを更新する
				UpdateBoneTransforms(time,
									 relativeToCurrentTime,
									 ref boneTransforms,
									 ref currentClipValue,
									 ref currentTimeValue,
									 ref currentKeyframe);
			}

            UpdateWorldTransforms(rootTransform);
			UpdateSkinTransforms();
        }


		/// <summary>
		/// 指定されたクリップのボーンマトリックス配列を更新
		/// 今回更新されるボーンマトリックスをkeyframesから取得して
		/// ボーンマトリックス配列を更新します。
		/// </summary>
		public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime, ref Matrix[] targetBoneTransform, ref AnimationClip targetClip, ref TimeSpan targetTime, ref int targetKeyframe)
        {
            if (targetClip == null)
                throw new InvalidOperationException(
                            "AnimationPlayer.Update was called before StartClip");

			// 今回再生するキーフレームの時間を更新
            if (relativeToCurrentTime)
            {
                time += targetTime;

				// 最後まで再生したら時間を戻す
				while (time >= targetClip.Duration)
				{
					time -= targetClip.Duration;
				}
            }

			// もし時間が範囲外の場合は例外とする
            if ((time < TimeSpan.Zero) || (time >= targetClip.Duration))
                throw new ArgumentOutOfRangeException("time");

			// 時間が戻っているか？
            if (time < targetTime)
            {
				if (loopEnable)
				{
					// ループ再生する場合は現在のキーフレームとボーンマトリックスを初期化
					targetKeyframe = 0;
					skinningDataValue.BindPose.CopyTo(targetBoneTransform, 0);
				}
				else
				{
					// ループ再生しない場合は現在の再生時間をクリップの最後の再生時間を設定
					targetTime = targetClip.Duration;
					return;
				}
            }

            targetTime = time;

			// 今回再生するキーフレームのボーンマトリックスを設定
            IList<Keyframe> keyframes = targetClip.Keyframes;

			while (targetKeyframe < keyframes.Count)
            {
                Keyframe keyframe = keyframes[targetKeyframe];

                // 今回再生する分が終わったらループを抜ける
                if (keyframe.Time > targetTime)
                    break;

                targetBoneTransform[keyframe.Bone] = keyframe.Transform;
				
                targetKeyframe++;
            }
        }
		
        /// <summary>
        /// 現在と前回のクリップのボーンマトリックス配列を補間
        /// </summary>
		private void UpdateInterpolationBoneTransforms()
		{
			if (interpolationWeight > 0)
			{
				for (int i=0; i<skinningDataValue.BindPose.Count; i++)
				{
					boneTransforms[i] = Matrix.Lerp(previousBoneTransforms[i], boneTransforms[i], 1 - interpolationWeight);
				}

				interpolationWeight-=interpolationWeightCount;
			}
		}

        /// <summary>
        /// 現在のクリップのワールドマトリックス配列を更新
		/// 今回更新されたボーンマトリックスに指定された座標を表す
		/// マトリックスを掛けてワールドマトリックスを更新します。
        /// </summary>
        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            // ルートボーンのマトリックスをワールドマトリックスに変換
            worldTransforms[0] = boneTransforms[0] * rootTransform;

            // ルートボーン以外の親子関係を持ったボーンのマトリックスをワールドマトリックスに変換
            for (int bone = 1; bone < worldTransforms.Length; bone++)
            {
                int parentBone = skinningDataValue.SkeletonHierarchy[bone];

                worldTransforms[bone] = boneTransforms[bone] * worldTransforms[parentBone];
            }
        }


        /// <summary>
        /// 現在のクリップのスキニングマトリックス配列を更新
		/// インバースボーンマトリックスに対して今回更新された
		/// ワールドマトリックスを掛けてスキニングマトリックスを更新します。
        /// </summary>
        public void UpdateSkinTransforms()
        {
            for (int bone = 0; bone < skinTransforms.Length; bone++)
            {
                skinTransforms[bone] = skinningDataValue.InverseBindPose[bone] * worldTransforms[bone];
            }
        }


        /// <summary>
        /// 現在のクリップのボーンマトリックスを取得
        /// </summary>
        public Matrix[] GetBoneTransforms()
        {
            return boneTransforms;
        }


        /// <summary>
        /// 現在のクリップのワールドマトリックスを取得
        /// </summary>
        public Matrix[] GetWorldTransforms()
        {
            return worldTransforms;
        }


        /// <summary>
        /// 現在のクリップのスキニングマトリックスを取得
        /// </summary>
        public Matrix[] GetSkinTransforms()
        {
            return skinTransforms;
        }

        /// <summary>
        /// 現在のクリップを取得
        /// </summary>
        public AnimationClip CurrentClip
        {
            get { return currentClipValue; }
        }


        /// <summary>
        /// 現在再生している箇所(時間)を取得
        /// </summary>
        public TimeSpan CurrentTime
        {
            get { return currentTimeValue; }
        }
	}
}
