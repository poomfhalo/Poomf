using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GW_Lib.Utility
{
    public class ButtonSwitcher : MonoBehaviour
    {
        [SerializeField] Button button = null;
        [SerializeField] float turnOffFor = 0.5f;
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            StartCoroutine(Switch());
        }

        IEnumerator Switch()
        {
            bool original = button.interactable;
            button.interactable = false;
            yield return new WaitForSeconds(turnOffFor);
            button.interactable = original;
        }
    }
}