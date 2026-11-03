using System.Collections.Generic;
using UnityEngine;

namespace BelousovSDK.Audios
{
    [CreateAssetMenu(fileName = nameof(TrackList), menuName = nameof(Audios) + "/" + nameof(TrackList))]
    public class TrackList : ScriptableObject
    {
        [SerializeField] private List<Track> _tracks = new ();

        public IReadOnlyList<Track> Tracks => _tracks;
    }
}