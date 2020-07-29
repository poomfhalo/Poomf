using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poomf.Data
{
    [CreateAssetMenu(fileName = "HeadItemData", menuName = "ScriptableObjects/Items/HeadItemData", order = 1)]
    public class HeadItemData : ItemDataBase
    {
        [SerializeField, HideInInspector] bool initialized = false;
        protected virtual void OnEnable()
        {
            HasVariants = false;
            if (!initialized)
            {
                ItemCategory = ItemCategory.Head;
                initialized = true;
            }
        }
    }
}
