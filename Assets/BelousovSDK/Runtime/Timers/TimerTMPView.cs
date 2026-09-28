using BelousovSDK.UI.View;
using TMPro;
using UnityEngine;

namespace BelousovSDK.Timers
{
    [RequireComponent(typeof(TMP_Text))]
    public class TimerTMPView : AbstractTMPView
    {
        private const int Min2Sec = 60;

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

        public override void UpdateView()
        {
            if (_timer is null)
            {
                TextField.text = string.Empty;
            }
            else
            {
                int minutes = Mathf.CeilToInt(_timer.Time / Min2Sec);
                int seconds = Mathf.CeilToInt(_timer.Time % Min2Sec);
                TextField.text = string.Format(Format, minutes, seconds);
            }
        }
    }
}