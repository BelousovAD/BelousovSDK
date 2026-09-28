using System.Collections.Generic;
using UnityEngine;

namespace BelousovSDK.Animation
{
    [RequireComponent(typeof(Animator))]
    internal class CustomAnimator : MonoBehaviour
    {
        [SerializeField] private List<AnimationKey> _keys;

        private readonly Dictionary<AnimationKey, int> _parameters = new ();
        private Animator _animator;

        private void Awake() =>
            Initialize();

        private void Initialize()
        {
            if (_animator is not null)
            {
                return;
            }
            
            _animator = GetComponent<Animator>();
            _keys.ForEach(key => _parameters.Add(key, Animator.StringToHash(key.ToString())));
        }

        public void Play(AnimationKey key)
        {
            Initialize();
            
            if (_parameters.TryGetValue(key, out int parameterId) == false)
            {
                Debug.LogError($"{key} is not declared in field {nameof(_keys)} and can not be played");
                
                return;
            }

            foreach (int parameter in _parameters.Values)
            {
                _animator.SetBool(parameter, false);
            }
            
            _animator.SetBool(parameterId, true);
        }
    }
}
