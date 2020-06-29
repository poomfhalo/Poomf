using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Poomf.UI;
using Photon.Pun;

public class N_LoginMenu : MenuItemBase
{
    [SerializeField] TMP_InputField nickName = null;
    [SerializeField] Button login = null;

    void Start()
    {
        login.onClick.AddListener(OnLoginPressed);
        StartCoroutine(GetComponent<N_LoginMenuAnims>().AnimateIn());
    }

    private void OnLoginPressed()
    {
        if (string.IsNullOrEmpty(nickName.text)) 
            return;

        PhotonNetwork.NickName = nickName.text;
        StartCoroutine(GetComponent<N_LoginMenuAnims>().AnimateOut());
    }
}