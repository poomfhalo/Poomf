using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_ControlParticleSystem : UnitPlayable
    {
        [SerializeField] ParticleSystem m_particleSystem = null;
        [SerializeField] CommandMode command = CommandMode.Play;

        enum CommandMode
        {
            Play, Stop, StopAndClear, Pause
        }

        public override IEnumerator Behavior()
        {
            switch (command)
            {
                case CommandMode.Play:
                    m_particleSystem.Play(true);
                    break;
                case CommandMode.Stop:
                    m_particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    break;
                case CommandMode.StopAndClear:
                    m_particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    break;
                case CommandMode.Pause:
                    m_particleSystem.Pause(true);
                    break;
                default:
                    Debug.LogWarning("Undefined Command On Particle System");
                    break;
            }
            yield return new WaitForSeconds(m_particleSystem.main.duration);
        }

    }
}