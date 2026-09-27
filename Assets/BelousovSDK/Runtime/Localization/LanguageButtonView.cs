using System.Collections.Generic;
using BelousovSDK.UI.View;
using UnityEngine;

namespace BelousovSDK.Localization
{
    public class LanguageButtonView : AbstractImageView
    {
        [SerializeField] private SwitchLanguageButton _button;
        [SerializeField] private List<Sprite> _flags = new ();

        private void OnEnable()
        {
            _button.LanguageIndexChanged += UpdateView;
            UpdateView();
        }

        private void OnDisable() =>
            _button.LanguageIndexChanged -= UpdateView;

        public override void UpdateView() =>
            Image.sprite = _flags[_button.Index];
    }
}