using System;
using UnityEngine;

namespace BelousovSDK.Currencies
{
    public class Currency
    {
        protected const int Min = 0;

        private int _value;

        public Currency(CurrencyType type, int defaultValue = Min, int max = int.MaxValue)
        {
            Type = type;
            Max = max;
            Value = defaultValue;
        }

        public event Action Changed;

        public CurrencyType Type { get; }

        public int Max { get; }

        public int Value
        {
            get => _value;

            protected set
            {
                if (value != _value)
                {
                    _value = Mathf.Clamp(value, Min, Max);
                    Changed?.Invoke();
                }
            }
        }

        public bool IsFull => Value == Max;

        public void Earn(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Can not be negative");
            }

            Value += amount;
        }

        public void Spend(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Can not be negative");
            }

            Value -= amount;
        }
    }
}
