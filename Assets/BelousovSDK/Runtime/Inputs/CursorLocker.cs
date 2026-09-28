using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;

namespace BelousovSDK.Inputs
{
    public class CursorLocker : MonoBehaviour
    {
        private static readonly LinkedList<CursorLocker> s_lockers = new ();
        
        [SerializeField] private bool _lock;

        private Input _input;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOwners() =>
            s_lockers.Clear();

        [Inject]
        private void Initialize(Input input)
        {
            _input = input;
            UpdateCursorState();
        }

        private void OnEnable()
        {
            s_lockers.AddLast(this);
            UpdateCursorState();
        }

        private void OnDisable()
        {
            s_lockers.Remove(this);
            UpdateCursorState();
        }

        private void UpdateCursorState()
        {
            bool isLocked = s_lockers.Last.Value._lock;
            
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.Confined;
            Cursor.visible = !isLocked;
            
            if (_input == null)
            {
                return;
            }

            if (isLocked)
            {
                _input.Player.Look.Disable();
                _input.Player.Attack.Disable();
            }
            else
            {
                _input.Player.Look.Enable();
                _input.Player.Attack.Enable();
            }
        }
    }
}
