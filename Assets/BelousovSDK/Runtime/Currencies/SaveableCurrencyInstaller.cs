using System.Collections.Generic;
using BelousovSDK.Bootstrap;
using Reflex.Core;
using UnityEngine;

namespace BelousovSDK.Currencies
{
    internal class SaveableCurrencyInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField, Min(0)] private int _startingMoney = 25;

        private List<SaveableCurrency> _currencies;
        private ContainerBuilder _builder;

        public void InstallBindings(ContainerBuilder builder)
        {
            _builder = builder;
            _currencies = new List<SaveableCurrency>
            {
                new (CurrencyType.Money, _startingMoney),
            };
            
            _currencies.ForEach(currency => _builder.RegisterValue(currency, new[] { typeof(Currency) }));

            _builder.OnContainerBuilt += Initialize;
        }

        private void Initialize(Container container)
        {
            _builder.OnContainerBuilt -= Initialize;
            SavvyServicesProvider services = container.Resolve<SavvyServicesProvider>();
            _currencies.ForEach(currency => currency.Initialize(services));
        }
    }
}
