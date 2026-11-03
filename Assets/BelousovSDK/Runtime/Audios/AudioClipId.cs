using UnityEngine;

namespace BelousovSDK.Audios
{
    [CreateAssetMenu(fileName = nameof(AudioClipId), menuName = nameof(Audios) + "/" + nameof(AudioClipId))]
    public class AudioClipId : ScriptableObject
    {
        [SerializeField] private string _value;

        public string Value => _value;
    }
}