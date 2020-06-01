using UnityEngine;

namespace GW_Lib.Utility
{
    public class AwakeReParenter : MonoBehaviour
    {
        [SerializeField] GameObject objToReParent = null;
        [SerializeField] Transform newParent = null;
        [SerializeField] bool chaseOldParent = false;
        [Header("Read Only")]
        [SerializeField] Transform oldParent = null;
        Vector3 offSet;
        
        void Awake()
        {
            if (objToReParent)
            {
                oldParent = objToReParent.transform.parent;
                objToReParent.transform.SetParent(newParent);

                if(chaseOldParent)
                {
                    Vector3 oldPos = oldParent? oldParent.transform.position : Vector3.zero;
                    offSet = transform.position - oldPos;
                }
            }
            
        }
        void Update()
        {
            if(chaseOldParent && oldParent)
            {
                transform.position = oldParent.position + offSet;
            }
        }
    }
}