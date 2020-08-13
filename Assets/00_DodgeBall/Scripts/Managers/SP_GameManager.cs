using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SP_GameManager : MonoBehaviour
{
    [SerializeField] DodgeballCharacter player = null;

    void Awake()
    {
        //GameIntroManager.instance.extActivateOnStart = false;
        //MatchStateManager.instance.extCanPrepareOnStart = false;
        //DodgeballGameManager.instance.extSetPlayerOnStart = false;
    }
    void Start()
    {

        //var p = FindObjectsOfType<TaggedSpawnPoint>().ToList().Find(s => s.HasTag("MainCamera") && s.BelongsTo(team.teamTag));
        //p.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 15;
    }
    void Update()
    {
        
    }
}