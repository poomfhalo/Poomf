using UnityEngine;

public class TaggedSpawnPoint : MonoBehaviour
{
    [SerializeField] TeamTag teamTag = TeamTag.A;
    [SerializeField] string[] tags = new string[0];

    public Vector3 position => transform.position;
    public Quaternion rotation => transform.rotation;

    public bool HasTag(string t)
    {
        foreach (string testTag in tags)
        {
            if (t == testTag)
                return true;
        }
        return false;
    }
    public bool BelongsTo(TeamTag t) => teamTag == t;
}