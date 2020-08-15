using TMPro;
using UnityEngine;

public abstract class PlayerPlayStatsDisplayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI knockOuts = null;
    [SerializeField] TextMeshProUGUI downs = null;
    [SerializeField] TextMeshProUGUI kds = null;

    [SerializeField] TextMeshProUGUI passes = null;
    [SerializeField] TextMeshProUGUI throws = null;
    [SerializeField] TextMeshProUGUI revives = null;

    [Header("Read Only")]
    public PlayerPlayStats lastUsedStatsData = new PlayerPlayStats();

    protected void RefreshByData(PlayerPlayStats statsData)
    {
        this.lastUsedStatsData = statsData;
        if (statsData == null)
        {
            return;
        }

        knockOuts.text = statsData.knocks.ToString();
        downs.text = statsData.deaths.ToString();
        kds.text = statsData.GetKD();

        passes.text = statsData.passes.ToString();
        throws.text = statsData.throws.ToString();
        revives.text = statsData.revivies.ToString();
    }
}