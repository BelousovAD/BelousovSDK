using System.Collections.Generic;
using System.Linq;
using BelousovSDK.UI.Toggle;
using Reflex.Attributes;
using UnityEngine;

namespace BelousovSDK.Audios
{
    internal class AudioToggle : AbstractToggle
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

        protected override void HandleValue(bool value) =>
            _audio.SetActive(value);

        private void Subscribe()
        {
            Unsubscribe();
            _audio.ActivityChanged += UpdateValue;
            UpdateValue();
        }

        private void Unsubscribe()
        {
            if (_audio != null)
            {
                _audio.ActivityChanged -= UpdateValue;
            }
        }

        private void UpdateValue() =>
            Toggle.SetIsOnWithoutNotify(_audio.IsActive);
    }
}
