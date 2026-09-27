using System.Collections.Generic;
using UnityEngine;

namespace BelousovSDK.Platform
{
    [RequireComponent(typeof(DeviceTypeDetector))]
    internal class PlatformDependentObjectSwitcher : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _mobileObjects;
        [SerializeField] private List<GameObject> _standaloneObjects;

        private DeviceTypeDetector _deviceDetector;

        private void Awake() =>
            _deviceDetector = GetComponent<DeviceTypeDetector>();

        private void OnEnable()
        {
            _mobileObjects.ForEach(gameObj =>
                gameObj.SetActive(_deviceDetector.IsMobile()));
            _standaloneObjects.ForEach(gameObj =>
                gameObj.SetActive(!_deviceDetector.IsMobile()));
        }
    }
}