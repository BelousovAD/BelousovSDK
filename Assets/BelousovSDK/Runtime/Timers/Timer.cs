using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BelousovSDK.Timers
{
    public class Timer : IDisposable
    {
        private const int Min = 0;
        private const int Second = 1;
        
        private float _time;
        private CancellationTokenSource _cancellationTokenSource = new ();
        private UniTask _countdown = UniTask.CompletedTask;

        public void Initialize(int max)
        {
            if (max <= Min)
            {
                throw new ArgumentOutOfRangeException(nameof(max), "Must be positive");
            }
            
            Max = max;
        }

        public event Action Changed;
        
        public event Action Finished;

        public int Max { get; private set; } = 1;

        public float Time
        {
            get => _time;
            
            private set
            {
                _time = Mathf.Clamp(value, Min, Max);
                Changed?.Invoke();
            }
        }

        public bool IsFinished => Mathf.Approximately(Time, Min);

        public float Ratio => Time / Max;

        public float Progress => 1f - Ratio;

        public void Dispose() =>
            Stop();

        public void Add(float seconds)
        {
            switch (seconds)
            {
                case < Min:
                    throw new ArgumentOutOfRangeException(nameof(seconds), "Must be positive");
                case Min:
                    return;
            }

            if (IsFinished && _countdown.Status == UniTaskStatus.Succeeded)
            {
                Time += seconds;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                _countdown = Countdown(_cancellationTokenSource.Token);
            }
            else
            {
                Time += seconds;
            }
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private async UniTask Countdown(CancellationToken token)
        {
            while (!token.IsCancellationRequested && Time > Min)
            {
                if (await UniTask.Delay(TimeSpan.FromSeconds(Second), cancellationToken: token)
                        .SuppressCancellationThrow())
                {
                    return;
                }
                
                Time -= Second;
            }

            Time = Min;
            _countdown = UniTask.CompletedTask;
            
            if (token.IsCancellationRequested)
            {
                return;
            }
            
            Finished?.Invoke();
        }
    }
}
