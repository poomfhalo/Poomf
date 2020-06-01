using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResFactory
{
    public static GameObject Make(string p)
    {
        GameObject g = Resources.Load<GameObject>(p);
        GameObject clone = UnityEngine.Object.Instantiate(g);
        return clone;
    }
    public static T Make<T>(string p,Transform parent) where T: UnityEngine.Object
    {
        T t = Resources.Load<T>(p);
        T clone = UnityEngine.Object.Instantiate(t,parent); 
        return clone;
    }

}
