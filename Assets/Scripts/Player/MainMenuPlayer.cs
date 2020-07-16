using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPlayer : MonoBehaviour
{
    [Header("Idle Animation Parameters")]
    [SerializeField] float minLongIdleTime = 10f;
    [SerializeField] float maxLongIdleTime = 20f;

    Animator menuPlayerAnim = null;
    float timer = 0f;
    private void Awake()
    {
        menuPlayerAnim = GetComponent<Animator>();
        // Start the long idle timer, which will play the long idle animation when the timer hits 0
        timer = Random.Range(minLongIdleTime, maxLongIdleTime);
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                // Play the long idle animation
                menuPlayerAnim.SetTrigger("TriggerLongIdle");
                // Reset the timer
                timer = Random.Range(minLongIdleTime, maxLongIdleTime);
            }
        }
    }
}
