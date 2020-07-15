using UnityEngine;

public class RoundUI : MonoBehaviour
{
    [SerializeField] GameObject won, lost, notPlayed = null;

    public void WasWon()
    {
        won.SetActive(true);
        lost.SetActive(false);
        notPlayed.SetActive(false);
    }
    public void WasLost()
    {
        won.SetActive(false);
        lost.SetActive(true);
        notPlayed.SetActive(false);
    }
    public void WasNotPlayed()
    {
        won.SetActive(false);
        lost.SetActive(false);
        notPlayed.SetActive(true);
    }
}