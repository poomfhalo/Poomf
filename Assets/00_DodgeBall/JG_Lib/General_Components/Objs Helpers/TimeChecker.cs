using UnityEngine;

namespace GW_Lib.Utility
{
    public class TimeChecker
    {
        float counter = 1;
        float duration = 1;

        public float Remaining => (1-counter);

        public TimeChecker()
        {

        }
        public TimeChecker(float duration)
        {
            this.duration=duration;
        }

        public void Update()
        {
            if(duration == 0)
            {
                counter = 1;
                return;
            }
            if (counter<=1)
            {
                counter += Time.deltaTime / duration;
            }
        }
        public void NewDuration(float duration)
        {
            this.duration=duration;
        }
        public bool IsReady()
        {
            return counter>=1;
        }
        public void Restart()
        {
            counter = 0;
        }
        public void Restart(float newDuration)
        {
            NewDuration(newDuration);
            counter = 0;
        }
    }
}