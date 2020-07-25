using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LevelColType { BallCollider }
public abstract class LevelCollider : MonoBehaviour
{
    [SerializeField] TeamTag belongsTo = TeamTag.A;
    [SerializeField] LevelColType levelCol = LevelColType.BallCollider;

    [SerializeField] bool startState = true;

    protected List<Collider> cols = null;

    void Awake()
    {
        cols = GetComponentsInChildren<Collider>().ToList();
        cols.ForEach(c => c.enabled = startState);
    }
    public bool IsCorrectCol(TeamTag team, LevelColType colType) => team == belongsTo && levelCol == colType;
}