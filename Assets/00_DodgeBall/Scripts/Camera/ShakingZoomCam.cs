using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class ShakingZoomCam : MonoBehaviour
{
    [SerializeField] float zoomInMin = -5;
    [SerializeField] float zoomInMax = 2;
    [SerializeField] float zoomingDur = 1;

    List<TaggedSpawnPoint> cameras = new List<TaggedSpawnPoint>();
    CinemachineVirtualCamera cam = null;
    float startFOV = 0;
    Tween tween = null;
    void Start()
    {
        cameras = FindObjectsOfType<TaggedSpawnPoint>().ToList().FindAll(t=>t.HasTag("MainCamera"));
        cam = GetComponent<CinemachineVirtualCamera>();
        cam.LookAt = Dodgeball.instance.transform;
        startFOV = cam.m_Lens.FieldOfView;

        Dodgeball.instance.E_OnStateUpdated += OnStateUpdated;
        Dodgeball.instance.launchTo.onLaunchedTo += SetFocusOn;
        Dodgeball.instance.goTo.onGoto += SetFocusOn;
    }
    void OnDestroy()
    {
        if(Dodgeball.instance)
        {
            Dodgeball.instance.E_OnStateUpdated -= OnStateUpdated;
            Dodgeball.instance.launchTo.onLaunchedTo -= SetFocusOn;
            Dodgeball.instance.goTo.onGoto -= SetFocusOn;
        }
    }

    private void SetFocusOn()
    {
        transform.position = GetCamera().transform.position;
        transform.rotation = GetCamera().transform.rotation;

        cam.Priority = 16;
        float lerper = 0;
        tween.Kill();
        tween = DOTween.To(() => lerper, f => lerper = f, 1, zoomingDur).SetEase(Ease.InOutQuint).OnUpdate(() =>{
            float newFOV = startFOV;
            newFOV = Mathf.Lerp(startFOV + zoomInMin, startFOV + zoomInMax, lerper);
            cam.m_Lens.FieldOfView = newFOV;
        }).SetLoops(-1, LoopType.Yoyo);
    }
    private void OnStateUpdated(Dodgeball.BallState s)
    {
        switch (s)
        {
            case Dodgeball.BallState.OnGround:
                transform.position = GetCamera().transform.position;
                transform.rotation = GetCamera().transform.rotation;
                tween.Kill();

                cam.Priority = 10;

                float lerper = 0;
                float fromFOV = cam.m_Lens.FieldOfView;
                tween = DOTween.To(() => lerper, f => lerper = f, 1, zoomingDur).SetEase(Ease.InOutQuint).OnUpdate(() =>{
                    cam.m_Lens.FieldOfView = Mathf.Lerp(fromFOV, startFOV, lerper);
                }); 
                break;
        }
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