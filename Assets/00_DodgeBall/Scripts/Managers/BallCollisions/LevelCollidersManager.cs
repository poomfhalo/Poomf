using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelCollidersManager : MonoBehaviour
{
    List<BallCollider> ballColliders = null;
    List<TeamTag> ballColsOutEnabled = new List<TeamTag>();

    void Awake()
    {
        GameIntroManager.instance.OnEntryCompleted += OnEntryCompleted;
        ballColliders = FindObjectsOfType<BallCollider>().ToList();
    }

    private void OnEntryCompleted()
    {
        TeamsManager.instance.AllCharacters.ForEach(c => {
            c.GetComponent<CharaKnockoutPlayer>().E_OnTeleportedOut += OnTeleportedOut;
        });
    }

    private void OnTeleportedOut(DodgeballCharacter chara)
    {
        if (chara == null)
            return;

        TeamTag t = TeamsManager.GetNextTeam(chara).teamTag;
        if (ballColsOutEnabled.Contains(t))
            return;

        ballColliders.ForEach(bc => {
            if (bc.IsCorrectCol(t, LevelColType.BallCollider))
            {
                ballColsOutEnabled.Add(t);
                bc.OnTeamMemberOut();
            }
        });
    }
}
