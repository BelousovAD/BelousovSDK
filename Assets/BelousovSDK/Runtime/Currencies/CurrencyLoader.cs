using System.Collections.Generic;
using System.Linq;
using BelousovSDK.Bootstrap;
using Reflex.Attributes;
using UnityEngine;

namespace BelousovSDK.Currencies
{
    internal class CurrencyLoader : MonoBehaviour, ILoadable
    {
        private List<SaveableCurrency> _currencies;

        [Inject]
        private void Initialize(IEnumerable<Currency> currencies) =>
            _currencies = new List<SaveableCurrency>(
                currencies.Select(currency => currency as SaveableCurrency)
                .Where(currency => currency != null));

        public void Load() =>
            _currencies.ForEach(currency => currency.Load());
    }
}