using System;

public interface IEnergyAction
{
    Action<int> ConsumeEnergy { get; set; }
    Func<int,bool> CanConsumeEnergy { get; set; }
    Func<bool> AllowRegen { get; }
}