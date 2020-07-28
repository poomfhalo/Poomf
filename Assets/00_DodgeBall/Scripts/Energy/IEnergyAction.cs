using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnergyAction
{
    Action<float> ConsumeEnergy { get; set; }
    Func<float,bool> CanConsumeEnergy { get; set; }
    Func<bool> AllowRegen { get; }
}