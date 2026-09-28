using BelousovSDK.UI.View;
using UnityEngine;

namespace BelousovSDK.Currencies
{
    public class CurrencyTMPView : AbstractTMPView
    {
        [SerializeField] private CurrencyType _type;

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
            TextField.text = _currency is null ? string.Empty : string.Format(Format, _currency.Value, _currency.Max);
    }
}
