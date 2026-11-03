using System.Collections.Generic;
using BelousovSDK.Bootstrap;
using Reflex.Attributes;
using UnityEngine;

namespace BelousovSDK.Audios
{
    public class AudioLoader : MonoBehaviour, ILoadable
    {
        private List<Audio> _audios;

        [Inject]
        private void Initialize(IEnumerable<Audio> audios) =>
            _audios = new List<Audio>(audios);

        public void Load() =>
            _audios.ForEach(audioObject => audioObject.Load());
    }
}