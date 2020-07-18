using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class CharaFocusCam : MonoBehaviour
{
    CinemachineVirtualCamera cam = null;
    List<TaggedSpawnPoint> cameras;
    bool firstRun = true;

    void Start()
    {
        GameIntroManager.instance.OnEntryCompleted += OnEntryCompleted;
        cam = GetComponent<CinemachineVirtualCamera>();
    }
    void OnDestroy()
    {
        if (GameIntroManager.instance)
            GameIntroManager.instance.OnEntryCompleted -= OnEntryCompleted;
    }

    private void OnEntryCompleted()
    {
        if (firstRun)
        {
            cam.LookAt = DodgeballGameManager.instance.GetLocalPlayer.transform;
            TeamTag team = TeamsManager.GetTeam(DodgeballGameManager.instance.GetLocalPlayer).teamTag;
            cameras = FindObjectsOfType<TaggedSpawnPoint>().ToList().FindAll(t => t.HasTag("MainCamera") && t.BelongsTo(team));

            firstRun = false;
        }

        cam.Priority = 16;
        cam.transform.rotation = GetCamera().transform.rotation;
        cam.transform.position = GetCamera().transform.position;
    }

    private CinemachineVirtualCamera GetCamera()
    {
        int highestPriority = int.MinValue;
        CinemachineVirtualCamera highestCam = null;
        foreach (var c in cameras)
        {
            CinemachineVirtualCamera testCam = c.GetComponent<CinemachineVirtualCamera>();
            if (testCam.Priority > highestPriority)
            {
                highestPriority = testCam.Priority;
                highestCam = testCam;
            }
        }
        return highestCam;
    }
}