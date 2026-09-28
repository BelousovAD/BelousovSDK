using UnityEngine;

namespace BelousovSDK.Triggers
{
    public class Tag : MonoBehaviour
    {
        [SerializeField] private TagType _type;

        public TagType Type => _type;
    }
}