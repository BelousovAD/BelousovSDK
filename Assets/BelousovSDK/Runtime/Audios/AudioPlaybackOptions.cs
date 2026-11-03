namespace BelousovSDK.Audios
{
    public readonly struct AudioPlaybackOptions
    {
        public AudioPlaybackOptions(bool loop = false, bool ignoreListenerPause = false)
        {
            Loop = loop;
            IgnoreListenerPause = ignoreListenerPause;
        }

        public bool Loop { get; }
        
        public bool IgnoreListenerPause { get; }
    }
}
