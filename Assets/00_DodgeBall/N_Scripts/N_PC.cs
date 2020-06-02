using UnityEngine;
using Photon.Realtime;

[RequireComponent(typeof(PC))]
public class N_PC : MonoBehaviour
{
    public Player controller = null;
    [SerializeField] int actorID = -1;
    void Start()
    {
        if(!controller.IsLocal)
        {
            GetComponent<PC>().enabled = false;
        }
    }

    public void Initialize(Player controller)
    {
        actorID = controller.ActorNumber;
        this.controller = controller;
    }
}