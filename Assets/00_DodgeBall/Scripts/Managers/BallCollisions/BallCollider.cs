using UnityEngine;

public class BallCollider : LevelCollider
{
    [SerializeField] bool activityOnMemberOut = false;

    public void OnTeamMemberOut()
    {
        cols.ForEach(c => c.enabled = activityOnMemberOut);
    }
}