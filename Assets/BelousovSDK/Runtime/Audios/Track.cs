using UnityEngine;

namespace BelousovSDK.Audios
{
    [CreateAssetMenu(fileName = nameof(Track), menuName = nameof(Audios) + "/" + nameof(Track))]
    public class Track : ScriptableObject
    {
        [SerializeField] private AudioClipId _key;
        [SerializeField] private AudioClip _clip;
        
        public string Key => _key != null ? _key.Value : string.Empty;
        
        public AudioClip Clip => _clip;
    }
}