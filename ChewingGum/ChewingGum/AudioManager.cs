using Microsoft.Xna.Framework.Audio;

namespace ChewingGum
{
    public class AudioManager
    {
        #region フィールド
        /// <summary>
        /// 再生用オーディオエンジン
        /// </summary>
        private static AudioEngine audioEngine;

        /// <summary>
        /// サウンドエフェクト
        /// </summary>
        private static WaveBank soundEffectWave;
        private static SoundBank soundEffectBank;

        /// <summary>
        /// ＢＧＭ
        /// </summary>
        private static WaveBank backgroundMusicWave;
        private static SoundBank backgroundMusicBank;

        public enum Sound
        {
            ItemSelect,
            ItemSelected,
            ItemCanceled
        }

        public enum BackgroundMusic
        {
            BGM1,
            BGM2,
            BGM3,
            BGM4,
            ResultThema,
            TitleThema
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
            //LoadContentではないので相対パスで指定してあげる必要がある
            audioEngine = new AudioEngine(@"Content\res\audio\Audio.xgs");

            soundEffectWave = new WaveBank(audioEngine, @"Content\res\audio\SoundEffectWave.xwb");
            soundEffectBank = new SoundBank(audioEngine, @"Content\res\audio\SoundEffectBank.xsb");

            backgroundMusicWave = new WaveBank(audioEngine, @"Content\res\audio\BackgroundMusicWave.xwb");
            backgroundMusicBank = new SoundBank(audioEngine, @"Content\res\audio\BackgroundMusicBank.xsb");
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

        public static void SoundBackgroundMusic(BackgroundMusic bgmName)
        {
            Cue cue = backgroundMusicBank.GetCue(bgmName.ToString());
            cue.Play();
        }

        public static void SoundBackgroundMusic(string bgmName)
        {
            Cue cue = backgroundMusicBank.GetCue(bgmName);
            cue.Play();
        }

    }
}
