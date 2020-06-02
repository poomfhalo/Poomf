using UnityEngine;
using Photon.Realtime;

[RequireComponent(typeof(PC))]
public class N_PC : MonoBehaviour
{
    public Player controller = null;
    void Start()
    {
        if(controller.IsLocal)
        {
            GetComponent<PC>().enabled = false;
        }
    }

    internal void Initialize(Player controller)
    {
        this.controller = controller;
    }
}