using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace BelousovSDK.Audios
{
    internal class SoundPlayCaller : MonoBehaviour
    {
        private const AudioType SoundType = AudioType.Sound;

        [SerializeField] private AudioClipId _clipKey;
        [SerializeField] private Moment _moment;
        [SerializeField] private bool _ignoreListenerPause;

        private Audio _audio;

        private enum Moment
        {
            OnEnable = 0,
            OnDisable = 1,
        }

        [Inject]
        private void Initialize(IEnumerable<Audio> audios) =>
            _audio = audios.FirstOrDefault(audioObject => audioObject.Type == SoundType);

        private void Play()
        {
            if (_audio == null || _clipKey == null || destroyCancellationToken.IsCancellationRequested)
            {
                return;
            }
            
            AudioPlayback playback = _audio.Play(_clipKey.Value,
                new AudioPlaybackOptions(ignoreListenerPause: _ignoreListenerPause), destroyCancellationToken);
            playback?.Completion.Forget();
        }

        private void OnEnable()
        {
            if (_moment == Moment.OnEnable)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            if (_moment == Moment.OnDisable)
            {
                Play();
            }
        }
    }
}
