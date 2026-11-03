using System;
using BelousovSDK.Bootstrap;
using Reflex.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace BelousovSDK.Audios
{
    public class AudioInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioMixerGroup _musicGroup;
        [SerializeField] private AudioMixerGroup _soundGroup;
        [SerializeField] private PooledAudioSource _prefab;
        [SerializeField] private TrackList _musics;
        [SerializeField] private TrackList _sounds;

        private Audio _music;
        private Audio _sound;
        private ContainerBuilder _builder;

        public void InstallBindings(ContainerBuilder builder)
        {
            if (builder.HasBinding(typeof(Audio)) || builder.HasBinding(typeof(AudioSourceSpawner)))
            {
                throw new InvalidOperationException("Register audio only once in the root container.");
            }
            
            _builder = builder;
            GameObject spawnerObject = new (nameof(AudioSourceSpawner));
            AudioSourceSpawner spawner = spawnerObject.AddComponent<AudioSourceSpawner>();
            DontDestroyOnLoad(spawnerObject);
            spawner.Initialize(_prefab, spawnerObject.transform);
            spawner.Prewarm(20);
            _music = new Audio(AudioType.Music, _musicGroup, spawner, _musics.Tracks);
            _sound = new Audio(AudioType.Sound, _soundGroup, spawner, _sounds.Tracks);

            _builder.RegisterValue(spawner);
            _builder.RegisterValue(_music);
            _builder.RegisterValue(_sound);
            _builder.RegisterValue(new AudioMixerController(_audioMixer, _music, _sound));

            _builder.OnContainerBuilt += Initialize;
        }

        private void Initialize(Container container)
        {
            _builder.OnContainerBuilt -= Initialize;
            SavvyServicesProvider services = container.Resolve<SavvyServicesProvider>();
            _music.Initialize(services);
            _sound.Initialize(services);
        }
    }
}
