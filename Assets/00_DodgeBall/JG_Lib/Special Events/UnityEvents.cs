using UnityEngine.Events;
using System;
using UnityEngine;

namespace GW_Lib.Utility.Events
{
    [Serializable]
    public class UnityIntEvent : UnityEvent<int> { }
    [Serializable]
    public class UnityFloatEvent : UnityEvent<float> { }
    [Serializable]
    public class UnityFloatIntEvent : UnityEvent<float, int> { }
    [Serializable]
    public class UnityCallBackEvent : UnityEvent<Action> { }
    [Serializable]
    public class UnityTransformEvent : UnityEvent<Transform> { }
    [Serializable]
    public class UnityColliderEvent : UnityEvent<Collider> { }
    [Serializable] 
    public class UnityPSEvent : UnityEvent<ParticleSystem> { }
}