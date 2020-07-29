using TMPro;
using UnityEngine;

public class EnergyUI : MonoBehaviour
{
    [SerializeField] Energy energy = null;
    [SerializeField] TextMeshProUGUI text = null;

    void Update()
    {
        text.text = Mathf.RoundToInt(energy.GetEnergy()).ToString();
    }
}