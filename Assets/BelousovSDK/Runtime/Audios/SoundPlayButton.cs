using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        [SerializeField][Min(0f)] private float _delay;
        [SerializeField] private bool _ignoreListenerPause = true;

        private Audio _audio;

        [Inject]
        private void Initialize(IEnumerable<Audio> audios) =>
            _audio = audios.Single(audioObject => audioObject.Type == SoundType);

        protected override void HandleClick() =>
            PlayAsync(destroyCancellationToken).Forget();

        private async UniTask PlayAsync(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Mathf.Max(0f, _delay)), ignoreTimeScale: true,
                cancellationToken: cancellationToken);
            
            cancellationToken.ThrowIfCancellationRequested();
            
            if (_audio == null || _clipKey == null)
            {
                return;
            }
            
            AudioPlayback playback = _audio.Play(_clipKey.Value,
                new AudioPlaybackOptions(ignoreListenerPause: _ignoreListenerPause), cancellationToken);
            
            if (playback != null)
            {
                await playback.Completion;
            }
        }
    }
}
