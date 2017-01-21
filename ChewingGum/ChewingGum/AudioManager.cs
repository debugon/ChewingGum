using Microsoft.Xna.Framework.Audio;

namespace ChewingGum
{
    public class AudioManager
    {
        #region フィールド
        /// <summary>
        /// オーディオ
        /// </summary>
        private AudioEngine audioEngine;
        private Cue cue;

        /// <summary>
        /// サウンドエフェクト
        /// </summary>
        private WaveBank soundEffectWave;
        private SoundBank soundEffectBank;

        /// <summary>
        /// ＢＧＭ
        /// </summary>
        private WaveBank backgroundMusicWave;
        private SoundBank backgroundMusicBank;

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

            //LoadContentではないので相対パスで指定してあげる必要がある
            audioEngine = new AudioEngine(@"Content\res\audio\Audio.xgs");

            soundEffectWave = new WaveBank(audioEngine, @"Content\res\audio\SoundEffectWave.xwb");
            soundEffectBank = new SoundBank(audioEngine, @"Content\res\audio\SoundEffectBank.xsb");

            backgroundMusicWave = new WaveBank(audioEngine, @"Content\res\audio\BackgroundMusicWave.xwb");
            backgroundMusicBank = new SoundBank(audioEngine, @"Content\res\audio\BackgroundMusicBank.xsb");
        }

        #region 初期化
        /// <summary>
        /// 初期化
        /// </summary>
        public static void Initialize()
        {
            
        }
        #endregion

        public void SoundItem(Sound soundName)
        {
            cue = soundEffectBank.GetCue(soundName.ToString());
            cue.Play();
        }

        public void SoundItem(string soundName)
        {
            cue = soundEffectBank.GetCue(soundName);
            cue.Play();
        }

        public void SoundBackgroundMusic(BackgroundMusic bgmName)
        {
            cue = backgroundMusicBank.GetCue(bgmName.ToString());
            cue.Play();
        }

        public void SoundBackgroundMusic(string bgmName)
        {
            cue = backgroundMusicBank.GetCue(bgmName);
            cue.Play();
        }

        public Cue GetCue { get { return cue; } }
    }
}
