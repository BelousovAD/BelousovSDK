using System;
using BelousovSDK.Spawn;

namespace BelousovSDK.Audios
{
    public class AudioSourceSpawner : SiblingsSpawner, IDisposable
    {
        private bool _disposed;

        public void Prewarm(int count)
        {
            PooledAudioSource[] sources = new PooledAudioSource[count];
            
            try
            {
                for (int i = 0; i < count; i++)
                {
                    sources[i] = Spawn();
                }
            }
            finally
            {
                foreach (PooledAudioSource source in sources)
                {
                    if (source != null)
                    {
                        source.Release();
                    }
                }
            }
        }

        public new PooledAudioSource Spawn() => _disposed
            ? throw new ObjectDisposedException(nameof(AudioSourceSpawner))
            : base.Spawn().GetComponent<PooledAudioSource>();

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            
            _disposed = true;
            
            if (this == null)
            {
                return;
            }
            
            Clear();
            Destroy(gameObject);
        }
    }
}
