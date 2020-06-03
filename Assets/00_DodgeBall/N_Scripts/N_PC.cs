using UnityEngine;
using Photon.Realtime;

[RequireComponent(typeof(PC))]
public class N_PC : MonoBehaviour
{
    public Player controller = null;
    [SerializeField] int actorID = -1;
 
    public void Initialize(Player controller)
    {
        actorID = controller.ActorNumber;
        this.controller = controller;
        if (!controller.IsLocal)
        {
            GetComponent<PC>().enabled = false;
        }
    }
}