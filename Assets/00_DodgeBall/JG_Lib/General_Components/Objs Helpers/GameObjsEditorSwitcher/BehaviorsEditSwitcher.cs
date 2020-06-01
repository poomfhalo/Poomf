using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class BehaviorsEditSwitcher : MonoBehaviour
    {
        public Behaviour[] behaviors = new Behaviour[0];

        private void Reset()
        {
            GetBehaviors();
        }
        private void Awake()
        {
            if (Application.isPlaying)
            {
                Destroy(this);
            }
        }

        public void ActivateBehavior(int x)
        {
            if (behaviors.Length == 0)
            {
                return;
            }
            if (x < 0 || x >= behaviors.Length)
            {
                x = Mathf.Clamp(x, 0, behaviors.Length - 1);
            }
            foreach (Behaviour g in behaviors)
            {
                if (g == null)
                {
                    continue;
                }

                if (g.enabled)
                {
                    g.enabled = false;
                }
            }
            if (behaviors[x] == null)
            {
                return;
            }
            behaviors[x].enabled = true;
        }

        public void GetBehaviors()
        {
            List<Behaviour> b = GetComponents<Behaviour>().ToList();
            if(b.Contains(this))
            {
                b.Remove(this);
            }
            behaviors = b.ToArray();
        }
    }
}