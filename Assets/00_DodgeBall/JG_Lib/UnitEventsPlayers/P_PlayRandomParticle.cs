using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_PlayRandomParticle : UnitPlayable
    {
        [SerializeField] List<ParticleSystem> particles = new List<ParticleSystem>();
        [SerializeField] int playsCount = 1;
        [SerializeField] int maxTries = 10;
        [SerializeField] bool playParticlesInSeq = false;

        protected override void Reset()
        {
            base.Reset();
            for(int i =0;i<transform.childCount;i++)
            {
                ParticleSystem ps = transform.GetChild(i).GetComponent<ParticleSystem>();
                particles.Add(ps);
            }
        }
        public override IEnumerator Behavior()
        {
            List<ParticleSystem> playables = new List<ParticleSystem>();
            for (int i = 0; i < playsCount; i++)
            {
                int rnd = UnityEngine.Random.Range(0, particles.Count);
                ParticleSystem testParticle = particles[rnd];

                if (playables.Contains(testParticle))
                {
                    for (int j = 0; j < maxTries; j++)
                    {
                        rnd = UnityEngine.Random.Range(0, particles.Count);
                        testParticle = particles[rnd];
                        if(playables.Contains(testParticle) == false)
                        {
                            break;
                        }
                    }
                }
                if(playables.Contains(testParticle) == false)
                {
                    playables.Add(testParticle);
                }                
            }
            foreach (ParticleSystem ps in playables)
            {
                ps.Play(true);
                if(playParticlesInSeq)
                {
                    yield return new WaitForSeconds(ps.main.duration);
                }
            }
            yield break;
        }
    }
}