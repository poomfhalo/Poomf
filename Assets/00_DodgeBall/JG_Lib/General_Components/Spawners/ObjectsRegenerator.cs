using UnityEngine;

namespace GW_Lib.Utility
{
    public class ObjectsRegenerator : MonoBehaviour
    {
        [SerializeField] RegeneratablePoint[] points = new RegeneratablePoint[0];
        [SerializeField] GameObject objPrefab = null;
        [SerializeField] float timeBetweenRegens = 2.0f;

        [SerializeField] bool isActive = false;

        [Header("Read Olnly")]
        [SerializeField] float regenCounter = 0;


        public void Run()
        {
            isActive = true;
        }
        public void Run(float timeBetweenRegens)
        {
            this.timeBetweenRegens = timeBetweenRegens;
            Run();
        }
        public void Stop()
        {
            isActive = false;
        }

        private void Update()
        {
            if (isActive == false||GetEmptyPoint() == null)
            {
                return;
            }
            regenCounter = regenCounter + Time.deltaTime/timeBetweenRegens;
            if(regenCounter<1)
            {
                return;
            }
            regenCounter = 0;
            GetEmptyPoint().Add(objPrefab);
        }
        private RegeneratablePoint GetEmptyPoint()
        {
            foreach (RegeneratablePoint p in points)
            {
                if (p.hasPoint)
                {
                    continue;
                }
                return p;
            }
            return null;
        }
    }
}