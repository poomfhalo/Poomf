using UnityEngine;

namespace Poomf.UI
{
    public abstract class MenuLockerColorOption : MonoBehaviour
    {
        [SerializeField] protected Animator animator = null;
        [SerializeField] protected CharaSkinData skinData = null;

        protected ItemCategory itemToModify = ItemCategory.Head;
        protected bool visible = false;

        public virtual void Initialize(CharaSkinData skinData)
        {
            this.skinData = skinData;
        }
        public void Appear()
        {
            visible = !visible;
            animator.SetBool("appear", visible);
        }

        // Called when one of the Color buttons is pressed. Sets the current slot's color to the selected color.
        public abstract void OnColorButtonSelected();
    }
}