using UnityEngine;

public abstract class PersistantSO : ScriptableObject
{
    protected virtual void OnEnable()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}