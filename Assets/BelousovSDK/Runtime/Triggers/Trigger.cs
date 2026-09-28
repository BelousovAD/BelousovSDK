using System;
using System.Collections.Generic;
using UnityEngine;

namespace BelousovSDK.Triggers
{
    [RequireComponent(typeof(Collider))]
    public class Trigger : MonoBehaviour
    {
        [SerializeField] private TagType _type;

        private readonly HashSet<Tag> _tags = new ();
        
        public event Action<bool> StateChanged;

        public bool IsOccupied => _tags.Count > 0;

        public void Forget(Tag tagComponent)
        {
            if (tagComponent.Type == _type &&
                _tags.Remove(tagComponent) &&
                _tags.Count == 0)
            {
                StateChanged?.Invoke(false);
            }
        }

        private void OnDisable()
        {
            if (!IsOccupied)
            {
                return;
            }
            
            _tags.Clear();
            StateChanged?.Invoke(false);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.TryGetComponent(out Tag tagComponent) &&
                tagComponent.Type == _type &&
                _tags.Add(tagComponent) &&
                _tags.Count == 1)
            {
                StateChanged?.Invoke(true);
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (collision.TryGetComponent(out Tag tagComponent))
            {
                Forget(tagComponent);
            }
        }
    }
}
