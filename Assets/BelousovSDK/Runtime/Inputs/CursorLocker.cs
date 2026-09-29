using System.Collections.Generic;
using UnityEngine;

namespace BelousovSDK.Inputs
{
    public class CursorLocker : MonoBehaviour
    {
        private static readonly LinkedList<CursorLocker> s_lockers = new ();
        
        [SerializeField] private bool _lock;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOwners() =>
            s_lockers.Clear();

        protected virtual void OnEnable()
        {
            s_lockers.AddLast(this);
            UpdateCursorState();
        }

        protected virtual void OnDisable()
        {
            s_lockers.Remove(this);
            UpdateCursorState();
        }

        protected virtual void UpdateCursorState()
        {
            bool isLocked = s_lockers.Last?.Value._lock ?? false;
            
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.Confined;
            Cursor.visible = !isLocked;
        }
    }
}
