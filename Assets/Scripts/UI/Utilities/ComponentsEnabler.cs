using UnityEngine;

public class ComponentsEnabler : MonoBehaviour
{
    [SerializeField] Behaviour component = null;

    void Start()
    {
        if (null != component)
        {
            if (false == component.enabled)
            {
                component.enabled = true;
            }
        }
    }

}
