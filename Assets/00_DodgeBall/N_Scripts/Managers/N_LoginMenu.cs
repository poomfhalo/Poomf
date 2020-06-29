using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Poomf.UI;
using Photon.Pun;

public class N_LoginMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField nickName = null;
    [SerializeField] Button login = null;
    [SerializeField] MenuAnimationsController animController = null;

    void Start()
    {
        login.onClick.AddListener(OnLoginPressed);
        animController.ShowScreen(animController.LoginAnimatedScreen);
    }

    private void OnLoginPressed()
    {
        if (string.IsNullOrEmpty(nickName.text)) 
            return;

        PhotonNetwork.NickName = nickName.text;
        animController.HideScreen(animController.LoginAnimatedScreen);
    }
}