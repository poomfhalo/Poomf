using UnityEngine;
using TMPro;

public class CharaHitPointsUI : MonoBehaviour
{
    [SerializeField] CharaHitPoints hp = null;
    [SerializeField] TextMeshProUGUI hpText = null;

    void Awake()
    {
        hp.OnHPUpdated += OnHPUpdated;
    }

    private void OnHPUpdated()
    {
        hpText.text = hp.CurrHP.ToString();
    }
}