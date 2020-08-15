using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPlayStatsGameHolder : MonoBehaviour
{
    public static PlayerPlayStatsGameHolder instance = null;

    [SerializeField] List<string> cleanUpScene = new List<string> { "Menu","SP_Room","MP_Room" };

    [Header("Read Only")]
    [SerializeField] List<PlayerToStatsPair> statsPairs = new List<PlayerToStatsPair>();

    void Start()
    {
        if (instance == null)
        {
            transform.SetParent(null);
            instance = this;
            SceneManager.activeSceneChanged += OnSceneUpdated;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.activeSceneChanged -= OnSceneUpdated;
            instance = null;
        }
    }

    private void OnSceneUpdated(Scene arg0, Scene arg1)
    {
        if(cleanUpScene.Contains( arg1.name ))
        {
            Destroy(gameObject);
        }
    }

    public void AddStats(int character, PlayerPlayStats stats)
    {
        PlayerToStatsPair pair = null;
        if(DoesCharacterExist(character,out pair))
        {
            pair.totalStats = pair.totalStats + stats;
        }
        else
        {
            pair = new PlayerToStatsPair
            {
                chara = character,
                totalStats = stats
            };
            statsPairs.Add(pair);
        }
    }
    public PlayerPlayStats GetStats(int chara)
    {
        if (DoesCharacterExist(chara, out PlayerToStatsPair pair))
            return pair.totalStats;
        return null;
    }


    private bool DoesCharacterExist(int charaID,out PlayerToStatsPair pair)
    {
        pair = statsPairs.Find(p => p.chara == charaID);
        return pair != null;
    }
}
