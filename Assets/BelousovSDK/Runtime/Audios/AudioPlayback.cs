using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BelousovSDK.Audios
{
    /// <summary>
    /// An individual playback. Stop and cancellation cancel Completion; only natural completion succeeds.
    /// </summary>
    public sealed class AudioPlayback : IDisposable
    {
        private readonly PooledAudioSource _owner;
        private readonly CancellationTokenSource _cancellation;
        private readonly CancellationToken _token;
        private readonly UniTaskCompletionSource _completion = new();
        private AudioSource _source;
        private bool _started;

        internal AudioPlayback(PooledAudioSource owner, AudioSource source,
            CancellationToken cancellationToken, CancellationToken ownerToken)
        {
            _owner = owner;
            _source = source;
            _cancellation = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, ownerToken, owner.destroyCancellationToken);
            _token = _cancellation.Token;
        }

        internal event Action<AudioPlayback> Finished;

        public UniTask Completion => _completion.Task;
        
        public bool IsCompleted { get; private set; }
        
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Available only while this operation owns its source. Do not retain it after completion or cancellation.
        /// </summary>
        public AudioSource Source => IsCompleted || _token.IsCancellationRequested ? null : _source;

        public void Stop()
        {
            if (IsCompleted)
            {
                return;
            }
            
            if (!_token.IsCancellationRequested && _source != null)
            {
                _source.Stop();
            }
            
            _cancellation.Cancel();
        }

        public void Pause()
        {
            if (Source == null || IsPaused)
            {
                return;
            }
            
            IsPaused = true;
            
            if (_started)
            {
                _source.Pause();
            }
        }

        public void Resume()
        {
            if (Source == null || !IsPaused)
            {
                return;
            }
            
            IsPaused = false;
            
            if (_started)
            {
                _source.UnPause();
            }
        }

        public void Dispose() =>
            Stop();

        internal void Start() =>
            RunAsync().Forget();

        private async UniTask RunAsync()
        {
            Exception failure = null;
            
            try
            {
                _token.ThrowIfCancellationRequested();
                AudioClip clip = _source.clip;
                
                if (clip == null)
                {
                    throw new InvalidOperationException("Playback requires an AudioClip.");
                }
                
                if (clip.loadState == AudioDataLoadState.Unloaded && !clip.LoadAudioData())
                {
                    throw new InvalidOperationException("AudioClip data could not be loaded.");
                }
                
                await UniTask.WaitUntil(() => clip.loadState != AudioDataLoadState.Loading, cancellationToken: _token);
                
                if (clip.loadState != AudioDataLoadState.Loaded)
                {
                    throw new InvalidOperationException("AudioClip data is unavailable.");
                }
                
                await UniTask.WaitUntil(() => !IsPaused, cancellationToken: _token);
                
                _token.ThrowIfCancellationRequested();
                _source.Play();
                _started = true;
                
                await UniTask.Yield(PlayerLoopTiming.Update, _token);
                // isPlaying is false during Pause too. Paused operations must keep their pool lease.
                await UniTask.WaitUntil(() => !IsPaused &&
                    (!AudioListener.pause || _source.ignoreListenerPause) && !_source.isPlaying,
                    cancellationToken: _token);
                
                _token.ThrowIfCancellationRequested();
            }
            catch (Exception exception)
            {
                failure = exception;
            }
            finally
            {
                IsCompleted = true;
                _source = null;
                
                try
                {
                    if (_owner != null)
                    {
                        _owner.FinishPlayback(this);
                    }
                }
                catch (Exception exception)
                {
                    failure ??= exception;
                }
                finally
                {
                    _cancellation.Dispose();
                    Finished?.Invoke(this);
                }
            }
            
            if (failure is OperationCanceledException canceled)
            {
                _completion.TrySetCanceled(canceled.CancellationToken);
            }
            else if (failure != null)
            {
                _completion.TrySetException(failure);
            }
            else
            {
                _completion.TrySetResult();
            }
        }
    }
}
