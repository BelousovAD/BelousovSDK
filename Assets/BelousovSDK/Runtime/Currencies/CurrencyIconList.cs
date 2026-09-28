using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BelousovSDK.Currencies
{
    [CreateAssetMenu(fileName = nameof(CurrencyIconList),
        menuName = nameof(Currencies) + "/" + nameof(CurrencyIconList))]
    public class CurrencyIconList : ScriptableObject
    {
        [SerializeField] private List<Entry> _entries;

        public Sprite Get(CurrencyType type) =>
            (from entry in _entries where entry.Type == type select entry.Icon).FirstOrDefault();

        [Serializable]
        private struct Entry
        {
            public CurrencyType Type;
            public Sprite Icon;
        }
    }
}
