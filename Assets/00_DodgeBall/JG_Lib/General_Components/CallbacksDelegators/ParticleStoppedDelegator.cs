using GW_Lib.Utility.Events;
using UnityEngine;

namespace GW_Lib.Utility
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleStoppedDelegator : MonoBehaviour
    {
        public UnityPSEvent onPSStopped = null;
        private void OnParticleSystemStopped()
        {
            onPSStopped?.Invoke(GetComponent<ParticleSystem>());
        }
    }
}