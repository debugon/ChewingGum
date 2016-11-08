using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using SkinnedModel;
using System;
using System.ComponentModel;

namespace SkinnedModelPipeline
{
    /// <summary>
    /// アニメーション付きスキンモデル用カスタムプロセッサ
    /// </summary>
    [ContentProcessor]
    public class SkinnedModelProcessor : ModelProcessor
    {
        // 最大ボーン数
        const int MaxBones = 72;

        /// <summary>
        /// マージするアニメーションのパス
        /// ファイルが同じなら指定する必要はありません
        /// </summary>
        private string AnimationsPath { get; set; }
                
        /// <summary>
        /// マージするアニメーションのファイル名
        /// ファイル毎に;で区切って入力してください。
        /// </summary>
        private string MergeAnimationFiles { get; set; }

        /// <summary>
        /// NodeContentからModelContentへの変換処理
        /// MergeAnimationsに入力されたアニメーションをマージします。
        /// NodeContentの情報から独自のアニメーションデータとしてSkinningDataを構築します。
        /// ModelContentに変換した後、TagプロパティにSkinningDataをセットします。
        /// </summary>
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            // ファイル名を指定せず、マージ元のファイルと同じ名前のフォルダの中にあるアニメーションファイルを見つける際の処理
            AnimationsPath = System.IO.Path.GetFileNameWithoutExtension(input.Identity.SourceFilename);
            
            // プロセッサーの処理するコンテンツのパスを取得
            string ModelDirectory = System.IO.Path.GetDirectoryName(input.Identity.SourceFilename);

            // ディレクトリ内のfbxファイル名を取得
            // 同じディレクトリに、モデル名と同じフォルダを作成しているならそこから見る
            string[] Animations = System.IO.Directory.GetFiles(ModelDirectory + @"\" + AnimationsPath, "*.fbx", System.IO.SearchOption.TopDirectoryOnly);

            // ファイルを取得できてなかった場合は
            if (Animations.Length == 0)
                // 同じディレクトリのfbxファイルを全て見る
                Animations = System.IO.Directory.GetFiles(ModelDirectory, "*.fbx", System.IO.SearchOption.TopDirectoryOnly);
            else
                // 取得できていた場合、ディレクトリパスにフォルダ名を追加
                ModelDirectory += @"\" + AnimationsPath;

            if (Animations.Length != 0)
            {
                foreach (string mergeFile in Animations)
                {
                    if (mergeFile != input.Identity.SourceFilename)
                        MergeAnimationFiles += ModelDirectory + @"\" + System.IO.Path.GetFileNameWithoutExtension(mergeFile) + ";";
                }
            }
            else
            {
                // フォルダ内にアニメーションのファイルが存在しない時
                throw new InvalidContentException(ModelDirectory + "内にfbx形式のファイルが見つかりませんでした。");
            }

            // アニメーションをマージする
            if (!string.IsNullOrEmpty(MergeAnimationFiles))
            {
                foreach (string mergeFile in MergeAnimationFiles.Split(';')
                                                            .Select(s => s.Trim())
                                                            .Where(s => !string.IsNullOrEmpty(s)))
                {
                    MergeAnimation(input, context, mergeFile + ".fbx");
                }
            }

            // メッシュが有効なデータか確認する
            ValidateMesh(input, context, null);

            // ボーンオブジェクトを取得
            BoneContent skeleton = MeshHelper.FindSkeleton(input);

            if (skeleton == null)
                throw new InvalidContentException("Input skeleton not found.");

            // モデルのメッシュ情報を平滑化
            FlattenTransforms(input, skeleton);

            // 各種ボーン情報を構築
            IList<BoneContent> bones = MeshHelper.FlattenSkeleton(skeleton);

            if (bones.Count > MaxBones)
            {
                throw new InvalidContentException(string.Format(
                    "Skeleton has {0} bones, but the maximum supported is {1}.",
                    bones.Count, MaxBones));
            }

            List<Matrix> bindPose = new List<Matrix>();
            List<Matrix> inverseBindPose = new List<Matrix>();
            List<int> skeletonHierarchy = new List<int>();

            foreach (BoneContent bone in bones)
            {
                bindPose.Add(bone.Transform);
                inverseBindPose.Add(Matrix.Invert(bone.AbsoluteTransform));
                skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
            }

            // アニメーションデータを構築
            Dictionary<string, AnimationClip> animationClips;
            animationClips = ProcessAnimations(skeleton.Animations, bones);

            // ベースクラスのメソッドを利用してNodeContentからModelContentに変換
            ModelContent model = base.Process(input, context);

            // 構築したアニメーションデータ、各種ボーン情報をSkinningDataとしてTagプロパティにセット
            model.Tag = new SkinningData(animationClips, bindPose, inverseBindPose, skeletonHierarchy);

            return model;
        }

        /// <summary>
        /// アニメーションをマージする
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <param name="mergeFile"></param>
        void MergeAnimation(NodeContent input, ContentProcessorContext context, string mergeFile)
        {
            NodeContent mergeModel = context.BuildAndLoadAsset<NodeContent, NodeContent>(
                                                   new ExternalReference<NodeContent>(mergeFile), null);

            BoneContent rootBone = MeshHelper.FindSkeleton(input);

            if (rootBone == null)
            {
                context.Logger.LogWarning(null, input.Identity, "Source model has no root bone.");
                return;
            }

            BoneContent mergeRoot = MeshHelper.FindSkeleton(mergeModel);

            if (mergeRoot == null)
            {
                context.Logger.LogWarning(null, input.Identity, "Merge model '{0}' has no root bone.", mergeFile);
                return;
            }

            foreach (string animationName in mergeRoot.Animations.Keys)
            {

                if (rootBone.Animations.ContainsKey(animationName))
                {
                    context.Logger.LogWarning(null, input.Identity,
                        "Cannot merge animation '{0}' from '{1}', because this animation already exists.",
                        animationName, mergeFile);

                    continue;
                }

                context.Logger.LogImportantMessage("アニメーション'{0}'のマージに成功しました。", animationName);

                rootBone.Animations.Add(animationName, mergeRoot.Animations[animationName]);
            }
        }


        /// <summary>
        /// NodeContentに含まれているアニメーションデータから独自形式のアニメーションデータを構築
        /// </summary>
        static Dictionary<string, AnimationClip> ProcessAnimations(AnimationContentDictionary animations, IList<BoneContent> bones)
        {
            // ボーン名をキーにしたボーンインデックス配列を作成
            Dictionary<string, int> boneMap = new Dictionary<string, int>();

            for (int i = 0; i < bones.Count; i++)
            {
                string boneName = bones[i].Name;

                if (!string.IsNullOrEmpty(boneName))
                    boneMap.Add(boneName, i);
            }

            // クリップ名をキーにしたアニメショーン配列を作成
            Dictionary<string, AnimationClip> animationClips;
            animationClips = new Dictionary<string, AnimationClip>();

            foreach (KeyValuePair<string, AnimationContent> animation in animations)
            {
                AnimationClip processed = ProcessAnimation(animation.Value, boneMap);

                animationClips.Add(animation.Key, processed);
            }

            if (animationClips.Count == 0)
            {
                throw new InvalidContentException(
                            "Input file does not contain any animations.");
            }

            return animationClips;
        }


        /// <summary>
        /// アニメーションデータを構築
        /// </summary>
        static AnimationClip ProcessAnimation(AnimationContent animation, Dictionary<string, int> boneMap)
        {
            List<Keyframe> keyframes = new List<Keyframe>();

            foreach (KeyValuePair<string, AnimationChannel> channel in animation.Channels)
            {
                // ボーン名をキーにしてボーンインデックスを取得
                int boneIndex;

                if (!boneMap.TryGetValue(channel.Key, out boneIndex))
                {
                    throw new InvalidContentException(string.Format(
                        "Found animation for bone '{0}', " +
                        "which is not part of the skeleton.", channel.Key));
                }

                // キーフレーム配列を作成
                foreach (AnimationKeyframe keyframe in channel.Value)
                {
                    keyframes.Add(new Keyframe(boneIndex, keyframe.Time, keyframe.Transform));
                }
            }

            // キーフレーム配列を昇順にソート
            keyframes.Sort(CompareKeyframeTimes);

            if (keyframes.Count == 0)
                throw new InvalidContentException("Animation has no keyframes.");

            if (animation.Duration <= TimeSpan.Zero)
                throw new InvalidContentException("Animation has a zero duration.");

            return new AnimationClip(animation.Name, animation.Duration, keyframes);
        }


        /// <summary>
        /// キーフレーム配列を昇順でソートするための時間比較機能
        /// </summary>
        static int CompareKeyframeTimes(Keyframe a, Keyframe b)
        {
            return a.Time.CompareTo(b.Time);
        }


        /// <summary>
        /// メッシュが有効なデータか確認
        /// </summary>
        static void ValidateMesh(NodeContent node, ContentProcessorContext context, string parentBoneName)
        {
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                // メッシュを確認する
                if (parentBoneName != null)
                {
                    context.Logger.LogWarning(null, null,
                        "Mesh {0} is a child of bone {1}. SkinnedModelProcessor " +
                        "does not correctly handle meshes that are children of bones.",
                        mesh.Name, parentBoneName);
                }

                if (!MeshHasSkinning(mesh))
                {
                    context.Logger.LogWarning(null, null,
                        "Mesh {0} has no skinning information, so it has been deleted.",
                        mesh.Name);

                    mesh.Parent.Children.Remove(mesh);
                    return;
                }
            }
            else if (node is BoneContent)
            {
                parentBoneName = node.Name;
            }

            // 再帰呼び出し
            foreach (NodeContent child in new List<NodeContent>(node.Children))
                ValidateMesh(child, context, parentBoneName);
        }


        /// <summary>
        /// メッシュがスキニング情報を含むか確認
        /// </summary>
        static bool MeshHasSkinning(MeshContent mesh)
        {
            foreach (GeometryContent geometry in mesh.Geometry)
            {
                if (!geometry.Vertices.Channels.Contains(VertexChannelNames.Weights()))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// モデルの全メッシュ情報を平滑化
        /// </summary>
        static void FlattenTransforms(NodeContent node, BoneContent skeleton)
        {
            foreach (NodeContent child in node.Children)
            {
                // スケルトンは処理しない
                if (child == skeleton)
                    continue;

                // ローカル座標でシーンの階層を更新
                MeshHelper.TransformScene(child, child.Transform);

                // 単位マトリックスを設定
                child.Transform = Matrix.Identity;

                // 再帰呼び出し
                FlattenTransforms(child, skeleton);
            }
        }

        /// <summary>
        /// 全てのマテリアルにSkinnedEffectを適用する
        /// </summary>
        [DefaultValue(MaterialProcessorDefaultEffect.SkinnedEffect)]
        public override MaterialProcessorDefaultEffect DefaultEffect
        {
            get { return MaterialProcessorDefaultEffect.SkinnedEffect; }
            set { }
        }
        
        // XNA4.0から、SkinnedEffectが標準で用意されており、                      
        // モデル→プロパティ→DefaultEffectに↑のソースで設定されています。              
        // カスタムエフェクトを使用したい場合は、以下のソースを有効にし、                    
        // エフェクトファイルをメインプロジェクトのコンテンツに追加して参照してください。
        /*
		/// <summary>
		/// マテリアル変換処理
		/// 標準エフェクトからカスタムエフェクト(SkinnedModel.fx)に置き換えます
		/// </summary>
		protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
		{
			BasicMaterialContent basicMaterial = material as BasicMaterialContent;

			if (basicMaterial == null)
			{
				throw new InvalidContentException(string.Format(
					"SkinnedModelProcessor only supports BasicMaterialContent, " +
					"but input mesh uses {0}.", material.GetType()));
			}

			EffectMaterialContent effectMaterial = new EffectMaterialContent();

			// エフェクトファイル(SkinnedModel.fx)を外部参照で設定
			string effectPath = Path.GetFullPath("SkinnedModel.fx");

			effectMaterial.Effect = new ExternalReference<EffectContent>(effectPath);

			// BasicMaterialContentのテクスチャ設定をコピーする
			if (basicMaterial.Texture != null)
				effectMaterial.Textures.Add("Texture", basicMaterial.Texture);

			// カスタムエフェクトを返す
			return base.ConvertMaterial(effectMaterial, context);
		}
         * */
    }
}