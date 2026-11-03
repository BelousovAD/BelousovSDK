using System;
using System.Threading;
using BelousovSDK.Spawn;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace BelousovSDK.Audios
{
    [RequireComponent(typeof(AudioSource))]
    public class PooledAudioSource : PooledComponent
    {
        private AudioSource _audioSource;
        private AudioPlayback _playback;

        private void Awake() =>
            _audioSource = GetComponent<AudioSource>();

        public void Initialize(AudioMixerGroup group, AudioClip clip)
        {
            if (_playback != null)
            {
                throw new InvalidOperationException("The source is already playing.");
            }
            
            _audioSource.outputAudioMixerGroup = group;
            _audioSource.clip = clip;
        }

        public void Play() =>
            Play(default).Completion.Forget();

        public AudioPlayback Play(AudioPlaybackOptions options, CancellationToken cancellationToken = default) =>
            Play(options, cancellationToken, CancellationToken.None);

        internal AudioPlayback Play(AudioPlaybackOptions options, CancellationToken cancellationToken,
            CancellationToken ownerToken)
        {
            if (_playback != null)
            {
                throw new InvalidOperationException("The source is already playing.");
            }
            
            _audioSource.playOnAwake = false;
            _audioSource.loop = options.Loop;
            _audioSource.ignoreListenerPause = options.IgnoreListenerPause;
            _audioSource.spatialBlend = 0f;
            _audioSource.volume = 1f;
            _audioSource.pitch = 1f;
            AudioPlayback playback = new(this, _audioSource, cancellationToken, ownerToken);
            _playback = playback;
            playback.Start();
            
            return playback;
        }

        internal void FinishPlayback(AudioPlayback playback)
        {
            // An external Release/disable can allow reuse before the old task observes cancellation.
            if (!ReferenceEquals(_playback, playback))
            {
                return;
            }
            
            _playback = null;
            ResetSource();
            Release();
        }

        private void OnDisable()
        {
            AudioPlayback playback = _playback;
            _playback = null;
            playback?.Stop();
            ResetSource();
        }

        private void ResetSource()
        {
            if (_audioSource == null)
            {
                return;
            }
            
            _audioSource.Stop();
            _audioSource.clip = null;
            _audioSource.outputAudioMixerGroup = null;
            _audioSource.loop = false;
            _audioSource.ignoreListenerPause = false;
        }
    }
}
