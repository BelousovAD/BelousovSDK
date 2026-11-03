using System;
using UnityEngine;
using UnityEngine.Audio;

namespace BelousovSDK.Audios
{
    internal class AudioMixerController : IDisposable
    {
        private const string MusicVolume = nameof(MusicVolume);
        private const string SoundVolume = nameof(SoundVolume);
        private const float MinValue = 0.0625f;
        private const float MaxValue = 1f;
        private const float LogarithmBase = 2;
        private const float Multiplier = 20;

        private readonly AudioMixer _audioMixer;
        private readonly Audio _music;
        private readonly Audio _sound;

        public AudioMixerController(AudioMixer audioMixer, Audio music, Audio sound)
        {
            _audioMixer = audioMixer;
            _music = music;
            _sound = sound;

            _music.VolumeChanged += UpdateMusicVolume;
            _sound.VolumeChanged += UpdateSoundVolume;
            _music.ActivityChanged += UpdateMusicVolume;
            _sound.ActivityChanged += UpdateSoundVolume;
            UpdateMusicVolume();
            UpdateSoundVolume();
        }

        public void Dispose()
        {
            _music.VolumeChanged -= UpdateMusicVolume;
            _sound.VolumeChanged -= UpdateSoundVolume;
            _music.ActivityChanged -= UpdateMusicVolume;
            _sound.ActivityChanged -= UpdateSoundVolume;
        }

        private void UpdateMusicVolume()
        {
            float value = Mathf.Clamp(float.IsFinite(_music.Volume) ? _music.Volume : Audio.DefaultVolume, MinValue, MaxValue);
            _audioMixer.SetFloat(MusicVolume, _music.IsActive ? Mathf.Log(value, LogarithmBase) * Multiplier : -80f);
        }

        private void UpdateSoundVolume()
        {
            float value = Mathf.Clamp(float.IsFinite(_sound.Volume) ? _sound.Volume : Audio.DefaultVolume, MinValue, MaxValue);
            _audioMixer.SetFloat(SoundVolume, _sound.IsActive ? Mathf.Log(value, LogarithmBase) * Multiplier : -80f);
        }
    }
}
