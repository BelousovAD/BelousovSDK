using BelousovSDK.UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace BelousovSDK.Timers
{
    [RequireComponent(typeof(Image))]
    public class TimerProgressView : AbstractImageView
    {
        private Timer _timer;

        public void Initialize(Timer timer)
        {
            Unsubscribe();
            _timer = timer;
            Subscribe();
            UpdateView();
        }

        private void OnEnable()
        {
            Subscribe();
            UpdateView();
        }

        private void OnDisable() =>
            Unsubscribe();

        private void Subscribe()
        {
            if (_timer is not null)
            {
                _timer.Changed += UpdateView;
            }
        }

        private void Unsubscribe()
        {
            if (_timer is not null)
            {
                _timer.Changed -= UpdateView;
            }
        }

        public override void UpdateView() =>
            Image.fillAmount = _timer?.Progress ?? 0f;
    }
}
