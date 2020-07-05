using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Poomf.Data
{
    /// <summary>
    /// Stores the current properties of custom objects. Objects with similar options (like hair objects that are all colorable)
    /// should use the same CustomItemData instance.
    /// </summary>
    [CreateAssetMenu(fileName = "CustomItemData", menuName = "ScriptableObjects/Customization/CustomItemData", order = 0)]
    public class CustomItemData : ScriptableObject
    {
        // The current colors of each material in the order they are placed in the mesh renderer
        [SerializeField] Color[] currentColors = null;
        [SerializeField] int currentTextureIndex = 0;

        #region Setters/Getters
        public Color[] CurrentColors { get { return currentColors; } set { currentColors = value; } }
        public int CurrentTextureIndex { get { return currentTextureIndex; } set { currentTextureIndex = value; } }
        #endregion
    }
}
