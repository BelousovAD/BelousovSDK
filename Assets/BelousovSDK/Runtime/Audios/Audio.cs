using System;
using System.Collections.Generic;
using System.Threading;
using BelousovSDK.Bootstrap;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace BelousovSDK.Audios
{
    public class Audio : IDisposable
    {
        public const float DefaultVolume = 0.5f;

        private readonly AudioMixerGroup _group;
        private readonly AudioSourceSpawner _spawner;
        private readonly Dictionary<string, AudioClip> _tracks = new();
        private readonly HashSet<AudioPlayback> _playbacks = new();
        private readonly CancellationTokenSource _lifetime = new();
        private SavvyServicesProvider _services;
        private AudioPlayback _musicPlayback;
        private bool _isActive = true;
        private float _volume = 1f;
        private bool _loaded;
        private bool _disposed;

        public Audio(AudioType type, AudioMixerGroup group, AudioSourceSpawner spawner, IEnumerable<Track> tracks)
        {
            Type = type;
            _group = group;
            _spawner = spawner;
            
            if (tracks == null)
            {
                throw new ArgumentNullException(nameof(tracks));
            }
            
            foreach (Track track in tracks)
            {
                if (track == null || string.IsNullOrEmpty(track.Key) || track.Clip == null)
                {
                    throw new ArgumentException("Tracks require a key and AudioClip.", nameof(tracks));
                }
                
                _tracks.Add(track.Key, track.Clip);
            }
        }

        public event Action ActivityChanged;
        
        public event Action VolumeChanged;

        public AudioType Type { get; }

        public bool IsActive
        {
            get => _isActive;
            
            private set
            {
                _isActive = value;
                ActivityChanged?.Invoke();
            }
        }

        public float Volume
        {
            get => _volume;
            
            private set
            {
                _volume = value;
                VolumeChanged?.Invoke();
            }
        }

        public void Initialize(SavvyServicesProvider servicesProvider) =>
            _services = servicesProvider ?? throw new ArgumentNullException(nameof(servicesProvider));

        public void SetActive(bool value)
        {
            ThrowIfDisposed();
            
            if (IsActive == value)
            {
                return;
            }
            
            IsActive = value;
            Save();
        }

        public void SetVolume(float value)
        {
            ThrowIfDisposed();
            
            if (!float.IsFinite(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            
            value = Mathf.Clamp01(value);
            
            if (Mathf.Approximately(Volume, value))
            {
                return;
            }
            
            Volume = value;
            Save();
        }

        public void Load()
        {
            ThrowIfDisposed();
            
            if (_loaded)
            {
                return;
            }
            
            if (_services is null)
            {
                throw new InvalidOperationException("Initialize audio before loading settings.");
            }
            
            bool active = _services.Preferences.LoadBool(Type + nameof(IsActive), true);
            float volume = _services.Preferences.LoadFloat(Type + nameof(Volume), DefaultVolume);
            IsActive = active;
            Volume = float.IsFinite(volume) ? Mathf.Clamp01(volume) : DefaultVolume;
            _loaded = true;
        }

        // Preserve the existing one-shot API; looping is explicit in AudioPlaybackOptions.
        public void Play(string key)
        {
            AudioPlayback playback = Play(key, default);
            playback?.Completion.Forget();
        }

        public AudioPlayback Play(string key, AudioPlaybackOptions options, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            
            if (key == null || !_tracks.TryGetValue(key, out AudioClip clip))
            {
                return null;
            }
            
            return StartPlayback(clip, options, cancellationToken);
        }

        /// <summary>
        /// Play a Track directly, including a voice Track outside the startup catalog.
        /// </summary>
        public AudioPlayback Play(Track track, AudioPlaybackOptions options = default, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            
            if (track == null || track.Clip == null)
            {
                throw new ArgumentException("Playback requires a Track with an AudioClip.", nameof(track));
            }
            
            return StartPlayback(track.Clip, options, cancellationToken);
        }

        public UniTask PlayAsync(Track track, AudioPlaybackOptions options = default, CancellationToken cancellationToken = default) =>
            Play(track, options, cancellationToken).Completion;

        public UniTask PlayAsync(string key, AudioPlaybackOptions options = default, CancellationToken cancellationToken = default)
        {
            AudioPlayback playback = Play(key, options, cancellationToken);
            
            return playback?.Completion ?? throw new KeyNotFoundException("No audio track is registered for key: " + key);
        }

        public void Stop()
        {
            foreach (AudioPlayback playback in new List<AudioPlayback>(_playbacks))
            {
                playback.Stop();
            }
        }

        public void Pause()
        {
            foreach (AudioPlayback playback in _playbacks)
            {
                playback.Pause();
            }
        }

        public void Resume()
        {
            foreach (AudioPlayback playback in _playbacks)
            {
                playback.Resume();
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            
            _disposed = true;
            Stop();
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        private AudioPlayback StartPlayback(AudioClip clip, AudioPlaybackOptions options, CancellationToken cancellationToken)
        {
            if (Type == AudioType.Music)
            {
                _musicPlayback?.Stop();
            }
            
            PooledAudioSource source = _spawner.Spawn();
            AudioPlayback playback;
            
            try
            {
                source.Initialize(_group, clip);
                playback = source.Play(options, cancellationToken, _lifetime.Token);
            }
            catch
            {
                source.Release();
                throw;
            }
            
            if (!playback.IsCompleted)
            {
                _playbacks.Add(playback);
                playback.Finished += OnPlaybackFinished;
            }
            
            if (Type == AudioType.Music)
            {
                _musicPlayback = playback.IsCompleted ? null : playback;
            }
            
            return playback;
        }

        private void OnPlaybackFinished(AudioPlayback playback)
        {
            playback.Finished -= OnPlaybackFinished;
            _playbacks.Remove(playback);
            
            if (ReferenceEquals(_musicPlayback, playback))
            {
                _musicPlayback = null;
            }
        }

        private void Save()
        {
            if (_services is null)
            {
                throw new InvalidOperationException("Initialize audio before saving settings.");
            }
            
            _services.Preferences.SaveBool(Type + nameof(IsActive), IsActive);
            _services.Preferences.SaveFloat(Type + nameof(Volume), Volume);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Audio));
            }
        }
    }
}
