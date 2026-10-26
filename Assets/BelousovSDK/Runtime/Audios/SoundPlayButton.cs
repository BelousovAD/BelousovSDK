using System;
using System.Collections.Generic;
using System.Linq;
using BelousovSDK.UI.Button;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace BelousovSDK.Audios
{
    internal class SoundPlayButton : AbstractButton
    {
        private const AudioType SoundType = AudioType.Sound;

        [SerializeField] private AudioClipId _clipKey;
        [SerializeField] private float _delay;

        private Audio _audio;

        [Inject]
        private void Initialize(IEnumerable<Audio> audios) =>
            _audio = audios.FirstOrDefault(audioObject => audioObject.Type == SoundType);

        protected override void HandleClick() =>
            UniTask.Delay(TimeSpan.FromSeconds(_delay)).ContinueWith(() => _audio.Play(_clipKey.Value)).Forget();
    }
}