using System.Collections.Generic;
using System.Linq;
using GW_Lib;
using UnityEngine;

public class SP_GameManager : MonoBehaviour
{
    [SerializeField] PC playerPrefab = null;
    [SerializeField] AIController aiChara = null;
    List<DodgeballCharacter> bornCharacters = new List<DodgeballCharacter>();

    void Awake()
    {
        GameIntroManager.instance.extActivateOnStart = false;
        MatchStateManager.instance.extCanPrepareOnStart = false;
        DodgeballGameManager.instance.extSetPlayerOnStart = false;

        List<DodgeballCharacter> charas = FindObjectsOfType<DodgeballCharacter>().ToList();
        charas.ForEach(c => Destroy(c.gameObject));
    }
    void Start()
    {
        PlayersRunDataSO data = PlayersRunDataSO.Instance;
        foreach (var d in data.playersRunData)
        {
            DodgeballCharacter createdCharacter = null;
            if (d.actorID == data.localPlayerID)
                createdCharacter = Instantiate(playerPrefab).chara;
            else
                createdCharacter = Instantiate(aiChara).GetComponent<DodgeballCharacter>();

            CharaSlot cs = createdCharacter.GetComponent<CharaSlot>();
            cs.setActiveOnStart = false; 
            cs.SetUp(d.actorID);

            createdCharacter.PrepareForGame();
            createdCharacter.SetName(d.charaName + "_" + createdCharacter.GetID());

            CustomizablePlayer cp = createdCharacter.GetComponentInChildren<CustomizablePlayer>();
            createdCharacter.GetComponentInChildren<CustomizablePlayer>().extLoadOnStart = false;
            cp.SetNewSkinData(d.charaSkinData.CreateSkinData());
        }

        DodgeballCharacter localPlayer = FindObjectOfType<PC>().chara;

        MatchStateManager.instance.PrerpareForGame();
        DodgeballGameManager.PrepareForGame(localPlayer);

        TeamTag localPlayerTeam = TeamsManager.GetTeam(localPlayer).teamTag;
        var p = FindObjectsOfType<TaggedSpawnPoint>().ToList().Find(s => s.HasTag("MainCamera") && s.BelongsTo(localPlayerTeam));
        p.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 15;

        this.InvokeDelayed(0.5f, GameIntroManager.instance.StartGame);
    }
}