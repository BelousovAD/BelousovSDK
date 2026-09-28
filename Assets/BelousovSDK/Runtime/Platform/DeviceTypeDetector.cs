using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BelousovSDK.Platform
{
    internal sealed class DeviceTypeDetector : MonoBehaviour
    {
#if !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        public static extern bool IsMobile();
#endif
#if UNITY_EDITOR
        private const string WindowTitleSimulator = "Simulator";
        private const string WindowTitleSimulatorDevice = "Simulator Device";

        public bool IsMobile() => IsSimulatorWindowOpen();

        private static bool IsSimulatorWindowOpen() =>
            Resources.FindObjectsOfTypeAll<EditorWindow>()
                .Any(window => window.titleContent.text is WindowTitleSimulator or WindowTitleSimulatorDevice);
#endif
    }
}
