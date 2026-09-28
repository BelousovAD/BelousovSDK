using System;
using BelousovSDK.Bootstrap;
using BelousovSDK.UI.Button;
using Reflex.Attributes;
using Savvy.Interfaces;
using UnityEngine;

namespace BelousovSDK.Localization
{
    internal class SwitchLanguageButton : AbstractButton
    {
        [SerializeField] private LocalizationSettings _settings;

        private int _index;
        private SavvyServicesProvider _services;

        public event Action LanguageIndexChanged;

        public int Index
        {
            get => _index;

            private set
            {
                if (value != _index)
                {
                    _index = value;
                    LanguageIndexChanged?.Invoke();
                }
            }
        }

        [Inject]
        private void Initialize(SavvyServicesProvider services)
        {
            _services = services;
            SystemLanguage currentLanguage = _services.Localisation.GetLanguage();

            for (int i = 0; i < _settings.TranslationsData.Length; i++)
            {
                if (_settings.TranslationsData[i].SystemLanguage == currentLanguage)
                {
                    Index = i;
                }
            }
        }

        protected override void HandleClick()
        {
            Index = (Index + 1) % _settings.TranslationsData.Length;
            _services.Localisation.SetLanguage(_settings.TranslationsData[Index].SystemLanguage);
        }
    }
}