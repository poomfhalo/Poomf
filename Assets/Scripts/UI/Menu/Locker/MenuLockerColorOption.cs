using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poomf.UI
{
    public class MenuLockerColorOption : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;
        [SerializeField] private Color[] colorPresets = null;

        // The slot that this menu is controlling its colors
        public CustomEquipmentSlot EquipSlot { get; set; }
        private bool visible = false;

        public void Appear()
        {
            visible = !visible;
            animator.SetBool("appear", visible);
        }
    }
}