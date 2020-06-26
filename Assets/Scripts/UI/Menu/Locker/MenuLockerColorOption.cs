using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poomf.UI
{
    public class MenuLockerColorOption : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private bool visible = false;

        public void Appear()
        {
            visible = !visible;
            animator.SetBool("appear", visible);
        }
    }
}