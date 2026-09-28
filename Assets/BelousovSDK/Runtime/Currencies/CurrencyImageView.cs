using BelousovSDK.UI.View;
using UnityEngine;

namespace BelousovSDK.Currencies
{
    internal class CurrencyImageView : AbstractImageView
    {
        [SerializeField] private CurrencyType _type;
        [SerializeField] private CurrencyIconList _iconList;

        private Currency _currency;

        private void OnEnable()
        {
            if (_currency is not null)
            {
                Subscribe();
            }

            UpdateView();
        }

        private void OnDisable()
        {
            if (_currency is not null)
            {
                Unsubscribe();
            }
        }

        public void Initialize(Currency currency)
        {
            OnDisable();
            _currency = currency;
            OnEnable();
        }

        private void Subscribe() =>
            _currency.Changed += UpdateView;

        private void Unsubscribe() =>
            _currency.Changed -= UpdateView;

        public override void UpdateView() =>
            Image.sprite = _currency is null ? DefaultSprite : _iconList.Get(_type);
    }
}