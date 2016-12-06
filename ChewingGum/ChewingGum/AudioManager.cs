using Microsoft.Xna.Framework.Audio;

namespace ChewingGum
{
    public class AudioManager
    {
        #region フィールド

        /// <summary>
        /// サウンドエフェクト
        /// </summary>
        private static AudioEngine audioEngine;
        private static WaveBank soundEffectWave;
        private static SoundBank soundEffectBank;

        public enum Sound
        {
            ItemSelect,
            ItemSelected,
            ItemCanceled
        }
        #endregion

        public AudioManager()
        {
            //コンストラクタ
        }

        #region 初期化
        /// <summary>
        /// 初期化
        /// </summary>
        public static void Initialize()
        {
            audioEngine = new AudioEngine(@"Content\res\audio\Audio.xgs");
            soundEffectWave = new WaveBank(audioEngine, @"Content\res\audio\SoundEffectWave.xwb");
            soundEffectBank = new SoundBank(audioEngine, @"Content\res\audio\SoundEffectBank.xsb");
        }
        #endregion

        public static void SoundItem(Sound soundName)
        {
            Cue cue = soundEffectBank.GetCue(soundName.ToString());
            cue.Play();
        }

        public static void SoundItem(string soundName)
        {
            Cue cue = soundEffectBank.GetCue(soundName);
            cue.Play();
        }

    }
}
