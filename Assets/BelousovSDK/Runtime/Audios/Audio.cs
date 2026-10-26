using System;
using System.Collections.Generic;
using BelousovSDK.Bootstrap;
using UnityEngine;
using UnityEngine.Audio;

namespace BelousovSDK.Audios
{
    public class Audio
    {
        private readonly AudioMixerGroup _group;
        private readonly AudioSourceSpawner _spawner;
        private readonly Dictionary<string, AudioClip> _tracks = new ();
        private SavvyServicesProvider _services;
        private bool _isActive = true;
        private float _volume = 1f;

        public Audio(
            AudioType type,
            AudioMixerGroup group,
            AudioSourceSpawner spawner,
            IEnumerable<Track> tracks)
        {
            Type = type;
            _group = group;
            _spawner = spawner;

            foreach (Track track in tracks)
            {
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
            _services = servicesProvider;

        public void SetActive(bool value)
        {
            IsActive = value;
            Save();
        }
        
        public void SetVolume(float value)
        {
            Volume = Mathf.Clamp01(value);
            Save();
        }

        public void Load()
        {
            IsActive = _services.Preferences.LoadBool(Type + nameof(IsActive), true);
            Volume = Mathf.Clamp01(_services.Preferences.LoadFloat(Type + nameof(Volume), 0.5f));
        }

        public void Play(string key)
        {
            if (!_tracks.TryGetValue(key, out AudioClip clip))
            {
                return;
            }
            
            PooledAudioSource audioSource = _spawner.Spawn();
            audioSource.Initialize(_group, clip);
            audioSource.Play();
        }

        private void Save()
        {
            _services.Preferences.SaveBool(Type + nameof(IsActive), IsActive);
            _services.Preferences.SaveFloat(Type + nameof(Volume), Volume);
        }
    }
}
