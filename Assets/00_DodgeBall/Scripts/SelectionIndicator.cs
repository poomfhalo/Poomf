using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    public DodgeballCharacter ActiveSelection => activeSelection;
    [SerializeField] GameObject enemy = null, friendly = null;

    Vector3 startLocalPos;
    [Header("Read Only")]
    [SerializeField] DodgeballCharacter owner = null;
    [SerializeField] DodgeballCharacter activeSelection = null;
    [SerializeField] DodgeballCharacter lastFriendly = null;
    [SerializeField] DodgeballCharacter lastEnemy = null;


    void Start()
    {
        enemy.SetActive(false);
        friendly.SetActive(false);
        startLocalPos = transform.localPosition;
    }
    public void SetOwner(DodgeballCharacter chara)
    {
        owner = chara;
    }
    public void SetFocus(DodgeballCharacter newFocus)
    {
        if(newFocus == null)
        {
            activeSelection = null;
            lastFriendly = null;
            lastEnemy = null;
            friendly.SetActive(false);
            enemy.SetActive(false);
            transform.SetParent(owner.transform);
            transform.localPosition = startLocalPos;
            return;
        }

        if (TeamsManager.AreFriendlies(newFocus, owner))
        {
            lastFriendly = newFocus;
            friendly.SetActive(true);
            enemy.SetActive(false);
        }
        else
        {
            lastEnemy = newFocus;
            enemy.SetActive(true);
            friendly.SetActive(false);
        }

        activeSelection = newFocus;
        transform.SetParent(activeSelection.transform);
        transform.localPosition = startLocalPos;
    }

    public void SetNewFocus(bool toFriendly)
    {
        DodgeballCharacter newFocus = null;
        bool wasLookingAtFriendly = TeamsManager.AreFriendlies(owner, activeSelection);

        if (toFriendly)
        {
            if (wasLookingAtFriendly || lastFriendly == null)
            {
                newFocus = TeamsManager.GetNextFriendly(owner, lastFriendly);
            }
            else
            {
                newFocus = lastFriendly;
            }
        }
        else
        {
            if (wasLookingAtFriendly && lastEnemy != null)
            {
                newFocus = lastEnemy;
            }
            else
            {
                newFocus = TeamsManager.GetNextEnemy(owner, lastEnemy, true);
            }
        }

        SetFocus(newFocus);
    }
}