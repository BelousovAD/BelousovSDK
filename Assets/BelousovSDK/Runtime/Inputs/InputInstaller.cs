using Reflex.Core;
using UnityEngine;

namespace BelousovSDK.Inputs
{
    internal class InputInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder) =>
            builder.RegisterValue(new Input());
    }
}