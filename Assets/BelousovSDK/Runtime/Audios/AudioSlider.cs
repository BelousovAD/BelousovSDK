using System.Collections.Generic;
using System.Linq;
using BelousovSDK.UI.Slider;
using Reflex.Attributes;
using UnityEngine;

namespace BelousovSDK.Audios
{
    internal class AudioSlider : AbstractSlider
    {
        [SerializeField] private AudioType _type;

        private Audio _audio;

        [Inject]
        private void Initialize(IEnumerable<Audio> audios)
        {
            Unsubscribe();
            _audio = audios.Single(audioObject => audioObject.Type == _type);
            
            if (isActiveAndEnabled)
            {
                Subscribe();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (_audio != null)
            {
                Subscribe();
            }
        }

        protected override void OnDisable()
        {
            Unsubscribe();
            base.OnDisable();
        }

        protected override void HandleValue(float value) =>
            _audio.SetVolume(value);

        private void Subscribe()
        {
            Unsubscribe();
            _audio.VolumeChanged += UpdateValue;
            UpdateValue();
        }

        private void Unsubscribe()
        {
            if (_audio != null)
            {
                _audio.VolumeChanged -= UpdateValue;
            }
        }

        private void UpdateValue() =>
            Slider.SetValueWithoutNotify(_audio.Volume);
    }
}
