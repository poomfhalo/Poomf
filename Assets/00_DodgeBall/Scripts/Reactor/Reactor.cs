using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Reactor : MonoBehaviour
{
    public UnityEvent onStarted = null;
    public UnityEvent onCompleted = null;
    [SerializeField] bool playOnStart = false;

    void Start()
    {
        if (playOnStart)
            React();
    }

    public void React()
    {
        StartCoroutine(ApplyReactions());
    }

    public IEnumerator ApplyReactions()
    {
        onStarted.Invoke();

        Reaction[] reactions = GetComponents<Reaction>();
        foreach (var r in reactions)
        {
            yield return StartCoroutine(r.React(this));
        }

        onCompleted.Invoke();
    }
}