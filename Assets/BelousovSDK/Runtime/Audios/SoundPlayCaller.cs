using System.Collections.Generic;
using System.Linq;
using Reflex.Attributes;
using UnityEngine;

namespace BelousovSDK.Audios
{
    internal class SoundPlayCaller : MonoBehaviour
    {
        private const AudioType SoundType = AudioType.Sound;

        [SerializeField] private AudioClipId _clipKey;
        [SerializeField] private Moment _moment;

        private Audio _audio;

        private enum Moment
        {
            OnEnable = 0,
            OnDisable = 1,
        }

        [Inject]
        private void Initialize(IEnumerable<Audio> audios) =>
            _audio = audios.FirstOrDefault(audioObject => audioObject.Type == SoundType);

        private void OnEnable()
        {
            if (_moment == Moment.OnEnable)
            {
                _audio.Play(_clipKey.Value);
            }
        }

        private void OnDisable()
        {
            if (_moment == Moment.OnDisable)
            {
                _audio.Play(_clipKey.Value);
            }
        }
    }
}