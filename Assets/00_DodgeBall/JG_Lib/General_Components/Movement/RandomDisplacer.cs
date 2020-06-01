using StealthGame.Utility;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class RandomDisplacer : MonoBehaviour
    {
        [SerializeField] MinMaxRange displacementLength = MinMaxRange.SmallDefaultPositive;
        [SerializeField] bool displaceOnStart = true;
        [SerializeField] bool useLocal = true;

        private void Start()
        {
            if(displaceOnStart)
            {
                Displace(displacementLength.GetValue());
            }
        }

        public void Displace(float f)
        {
            if(useLocal)
            {
                transform.localPosition = UnityEngine.Random.onUnitSphere * f;
            }
            else
            {
                transform.position = transform.position + UnityEngine.Random.onUnitSphere * f;
            }
        }
    }
}