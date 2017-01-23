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
        private Cue bgm;

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
        
        public void SoundItem(Sound soundName)
        {
            Cue se = null;

            se = soundEffectBank.GetCue(soundName.ToString());
            se.Play();
        }

        public void SoundItem(string soundName)
        {
            Cue se = null;

            se = soundEffectBank.GetCue(soundName);
            se.Play();
        }

        public void SoundBackgroundMusic(BackgroundMusic bgmName)
        {
            bgm = backgroundMusicBank.GetCue(bgmName.ToString());
            bgm.Play();
        }

        public void SoundBackgroundMusic(string bgmName)
        {
            bgm = backgroundMusicBank.GetCue(bgmName);
            bgm.Play();
        }

        public bool StopBGM()
        {
            return bgm.IsStopped;
        }

        public Cue GetCue { get { return bgm; } }
    }
}
