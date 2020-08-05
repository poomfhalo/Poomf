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
    public static T Make<T>(string p, Vector3 pos) where T : UnityEngine.Component
    {
        T t = Resources.Load<T>(p);
        T clone = UnityEngine.Object.Instantiate(t);
        clone.transform.position = pos;
        return clone;
    }
    public static Collider MakeContactPoint(Vector3 pos)
    {
        Collider contactPoint = Resources.Load<Collider>("ContactPoint");
        Collider clone = UnityEngine.Object.Instantiate(contactPoint);
        clone.transform.position = pos;
        return clone;
    }
}