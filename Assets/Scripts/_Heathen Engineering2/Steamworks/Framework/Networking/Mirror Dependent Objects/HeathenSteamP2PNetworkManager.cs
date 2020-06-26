#if UNITY_ANDROID || UNITY_IOS || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_WSA || UNITY_PS4 || UNITY_WII || UNITY_XBOXONE || UNITY_SWITCH
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS && MIRROR
using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HeathenEngineering.SteamApi.Networking
{

    [RequireComponent(typeof(SteamworksLobbyManager))]
    [RequireComponent(typeof(HeathenSteamP2PTransport))]
    public class HeathenSteamP2PNetworkManager : Mirror.NetworkManager
    {
        [HideInInspector]
        public SteamworksLobbyManager LobbyManager;
        [HideInInspector]
        public SteamworksLobbySettings LobbySettings;
        
        public UnityEvent OnHostStarted;
        public UnityEvent OnServerStarted;
        public UnityEvent OnClientStarted;
        public UnityEvent OnServerStoped;
        public UnityEvent OnClientStoped;
        public UnityEvent OnHostStoped;
        public UnityEvent OnRegisterServerMessages;
        public UnityEvent OnRegisterClientMessages;

        public override void Awake()
        {
            //Set the key references

            LobbyManager = GetComponent<SteamworksLobbyManager>();
            LobbySettings = LobbyManager.LobbySettings;
            transport = GetComponent<HeathenSteamP2PTransport>();

            // Set the networkSceneName to prevent a scene reload
            // if client connection to server fails.
            networkSceneName = offlineScene;

            base.Awake();
        }
        
        public override void LateUpdate()
        {
            if(LobbySettings == null)
                return;

            maxConnections = LobbySettings.MaxMemberCount.Value;
            base.LateUpdate();
        }

        public override void OnValidate()
        {
            // add transport if there is none yet. makes upgrading easier.
            if (transport == null)
            {
                // was a transport added yet? if not, add one
                transport = GetComponent<HeathenSteamP2PTransport>();
                if (transport == null)
                {
                    transport = gameObject.AddComponent<HeathenSteamP2PTransport>();
                    Debug.Log("NetworkManager: added default Transport because there was none yet.");
                }
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
            }

            if (LobbySettings != null)
                maxConnections = Mathf.Max(LobbySettings.MaxMemberCount.Value, 0);

            if (playerPrefab != null && playerPrefab.GetComponent<NetworkIdentity>() == null)
            {
                Debug.LogError("NetworkManager - playerPrefab must have a NetworkIdentity.");
                playerPrefab = null;
            }
        }
        
        public new void StartServer()
        {
            if (LobbySettings == null || !LobbySettings.IsHost)
            {
                Debug.LogError("[HeathenP2P_NetworkManager] Attempted to start server but is not a lobby owner.\nNo action taken");
                return;
            }

            networkAddress = LobbySettings.LobbyOwner.UserData.SteamId.m_SteamID.ToString();

            base.StartServer();
        }

        public new void StartClient()
        {
            if (LobbySettings == null || !LobbySettings.InLobby)
            {
                Debug.LogError("[HeathenP2P_NetworkManager] Attempted to start client without a lobby.");
                return;
            }

            networkAddress = LobbySettings.LobbyOwner.UserData.SteamId.m_SteamID.ToString();

            base.StartClient();
        }
          
        public override void OnStartHost()
        { OnHostStarted.Invoke(); }
        public override void OnStartServer()
        { OnServerStarted.Invoke(); }
        public override void OnStartClient()
        { OnClientStarted.Invoke(); }
        public override void OnStopServer()
        { OnServerStoped.Invoke(); }
        public override void OnStopClient()
        { OnClientStoped.Invoke(); }
        public override void OnStopHost()
        { OnHostStoped.Invoke(); }
    }
}
#endif