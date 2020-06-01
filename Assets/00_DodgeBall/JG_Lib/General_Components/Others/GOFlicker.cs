using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GW_Lib.Utility
{
    public class GOFlicker : MonoBehaviour
    {
        [SerializeField] UnityEvent onFlick = null;
        [SerializeField] MinMaxRange timeBetweenTests = new MinMaxRange(1, 3, 2, 3);
        [SerializeField] MinMaxRange timeBetweenFlicks = new MinMaxRange(0.05f, 0.3f, 0.1f, 0.25f);
        [SerializeField] MinMaxRange flicksCount = new MinMaxRange(2, 5, 2, 4);

        [SerializeField] bool alpplyChance = false;
        [SerializeField] float flickChance = 0.5f;
        [SerializeField] GameObject g = null;

        float counter = 0;
        float currTime = 0;
        bool isFlicking;
        void Start()
        {
            currTime = timeBetweenTests.GetValue();
        }
        void Update()
        {
            if(isFlicking)
            {
                return;
            }

            counter = counter + Time.deltaTime / currTime;
            if (counter < 1)
            {
                return;
            }

            counter = 0;
            currTime = timeBetweenTests.GetValue();

            if (alpplyChance == false)
            {
                StartCoroutine(Flick());
                return;
            }
            float rnd = UnityEngine.Random.value;
            if (flickChance > rnd)
            {
                StartCoroutine(Flick());
            }
        }

        private IEnumerator Flick()
        {
            int maxFlicksCount = Mathf.RoundToInt(this.flicksCount.GetValue());
            int flickscounter = 0;
            isFlicking = true;
            while (isFlicking)
            {
                float time = timeBetweenFlicks.GetValue();
                yield return new WaitForSeconds(time);
                onFlick?.Invoke();
                g.SetActive(!g.activeSelf);
                flickscounter = flickscounter + 1;
                isFlicking = flickscounter<maxFlicksCount;
            }
        }
    }
}