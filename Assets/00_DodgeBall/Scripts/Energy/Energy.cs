using System;
using System.Collections.Generic;
using System.Linq;
using GW_Lib;
using UnityEngine;

public class Energy : MonoBehaviour
{
    public float GetEnergyAsPercent => currEnergy / maxEnergy;

    [SerializeField] int maxEnergy = 100;
    [Tooltip("1 means, if player moves 1 world unit of distance, 1 unit of energy is regenerated," +
    	" if its 5 then each 1 unit the palyer moves, then he gains 5 energy unts")]
    [SerializeField] float regenSpeed = 5;
    [Tooltip("Auto Fills the Energy once the IntroManager assigns the Entry as completed")]
    [SerializeField] bool autoFillOnStart = true;

    [Header("Risky To Change")]
    [Tooltip("With an And, all Tests must be true, to regen, with an OR, only a single test needs to be true to regen")]
    [SerializeField] DecisionOperator decisionMaker = DecisionOperator.And;

    [Header("Read Only")]
    [SerializeField] int currEnergy = 0;
    public bool isInfinity = false;

    public Func<bool> ExtAllowWork = () => true;
    List<IEnergyAction> actions = new List<IEnergyAction>();
    Vector3 lastXZPos = new Vector3();
    Vector3 xzPos => new Vector3(transform.position.x, 0, transform.position.z);
    float deltaDist = 0;
    bool wasStarted = false;
    List<Func<bool>> regenTests = new List<Func<bool>>();
    float regenPointscounter = 0;
    void Start()
    {
        actions = GetComponentsInChildren<IEnergyAction>(true).ToList();
        actions.ForEach(a => {
            a.ConsumeEnergy = ConsumeEnergy;
            a.CanConsumeEnergy = CanConsumeEnergy;
            regenTests.Add(a.AllowRegen);
        });
      
        GameIntroManager.instance.OnEntryCompleted += () => {
            if (autoFillOnStart)
                SetEnergy(maxEnergy);

            lastXZPos = xzPos;
            wasStarted = true;
        };
    }
    void FixedUpdate()
    {
        if (!ExtAllowWork())
            return;
        if (!wasStarted)
            return;

        SetRegenData();

        if (!CanRegen())
            return;
        regenPointscounter = regenPointscounter + deltaDist * regenSpeed;
        GameExtentions.StripFloat(regenPointscounter, out int ints, out float frac);
        if (ints > 0)
        {
            RegenrateEnergy(ints);
            regenPointscounter = frac;
        }
    }

    public int GetEnergy() => currEnergy;
    public bool CanConsumeEnergy(int ofVal) => currEnergy >= ofVal;
    public void ConsumeEnergy(int val)
    {
        if (isInfinity)
            return;
        if (!ExtAllowWork())
            return;

        SetEnergy(currEnergy - val);
    }

    //Called To Set the Energy Directly, no calculations involved
    //for now, used as part of the multiplayer syncing, can be used in cutscenes.
    public void SetEnergy(int energy)
    {
        currEnergy = energy;
        currEnergy = Mathf.Clamp(currEnergy, 0, maxEnergy);
    }
    private void SetRegenData()
    {
        deltaDist = (lastXZPos - xzPos).magnitude;
        lastXZPos = xzPos;
        if (isInfinity && Mathf.Abs(GetEnergy() - maxEnergy) > Mathf.Epsilon)
        {
            SetEnergy(maxEnergy);
        }
    }
    private void RegenrateEnergy(int byVal)=> SetEnergy(currEnergy + byVal);
    private bool CanRegen()
    {
        List<bool> results = new List<bool>();
        regenTests.ForEach(t => {
            results.Add(t());
        });
        bool result = results.CheckOperationOnList(decisionMaker);
        return result;
    }
}