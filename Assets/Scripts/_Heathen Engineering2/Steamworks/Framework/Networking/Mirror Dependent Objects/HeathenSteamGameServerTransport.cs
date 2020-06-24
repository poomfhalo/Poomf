#if UNITY_ANDROID || UNITY_IOS || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_WSA || UNITY_PS4 || UNITY_WII || UNITY_XBOXONE || UNITY_SWITCH
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS && MIRROR
using System.Collections.Generic;
using UnityEngine;
using System;
using Steamworks;
using Mirror;
using System.Threading.Tasks;
using System.Collections;
using HeathenEngineering.SteamApi.Foundation;

namespace HeathenEngineering.SteamApi.Networking
{
    /// <summary>
    /// A Mirror compatable transport which leverages Steam's SteamGameServerNetworking interface for client/server networking situation.
    /// </summary>
    /// <remarks>
    /// Derived from FizzCube's FizzySteamyMirror available under the MIT license on GetHub as an extension of Mirror. 
    /// For the latest version check his GitHub at https://github.com/Raystorms/FizzySteamyMirror
    /// Note that this version has been modified to suit Heathen Steamworks.
    /// </remarks>
    public class HeathenSteamGameServerTransport : Transport
    {
        protected Client client = new Client();
        protected Server server = new Server();
        public float messageUpdateRate = 0.03333f;
        public EP2PSend[] channels = new EP2PSend[2] { EP2PSend.k_EP2PSendReliable, EP2PSend.k_EP2PSendUnreliable };

        private void Start()
        {
            CommonServer.secondsBetweenPolls = messageUpdateRate;
            if (channels == null)
            {
                channels = new EP2PSend[2] { EP2PSend.k_EP2PSendReliable, EP2PSend.k_EP2PSendUnreliable };
            }
            channels[0] = EP2PSend.k_EP2PSendReliable;
            CommonServer.channels = channels;
            CommonClient.channels = channels;
        }

        public HeathenSteamGameServerTransport()
        {
            // dispatch the events from the server
            server.OnConnected += (id) => OnServerConnected?.Invoke(id);
            server.OnDisconnected += (id) => OnServerDisconnected?.Invoke(id);

#if MIRROR_4_0_OR_NEWER
            server.OnReceivedData += (id, data, channel) => OnServerDataReceived?.Invoke(id, data, channel);
#else
            server.OnReceivedData += (id, data) => OnServerDataReceived?.Invoke(id, data);
#endif


            server.OnReceivedError += (id, exception) => OnServerError?.Invoke(id, exception);

            // dispatch events from the client
            client.OnConnected += () => OnClientConnected?.Invoke();
            client.OnDisconnected += () => OnClientDisconnected?.Invoke();

#if MIRROR_4_0_OR_NEWER
            client.OnReceivedData += (data, channel) => OnClientDataReceived?.Invoke(data, channel);
#else
            client.OnReceivedData += (data) => OnClientDataReceived?.Invoke(data);
#endif

            client.OnReceivedError += (exception) => OnClientError?.Invoke(exception);

            Debug.Log("Steam Transport initialized!");
        }

        // client
        public override bool ClientConnected() { return client.Connected; }
        public override void ClientConnect(string address) { client.Connect(address); }

#if MIRROR_4_0_OR_NEWER
        public override bool ClientSend(int channelId, ArraySegment<byte> segment) { return client.Send(segment.Array, channelId); }
#else
        public override bool ClientSend(int channelId, byte[] data) { return client.Send(data, channelId); }
#endif

        public override void ClientDisconnect() { client.Disconnect(); }

        // server
        public override bool ServerActive() { return server.Active; }
        public override void ServerStart()
        {
            server.Listen();
        }

        public override string ServerGetClientAddress(int connectionId) { return server.ServerGetClientAddress(connectionId); }
        public virtual void ServerStartWebsockets(string address, int port, int maxConnections)
        {
            Debug.LogError("SteamTransport.ServerStartWebsockets not possible!");
        }

#if MIRROR_4_0_OR_NEWER
        public override bool ServerSend(List<int> connectionIds, int channelId, ArraySegment<byte> segment)
        {
            bool result = true;

            foreach (var id in connectionIds)
            {
                var v = server.Send(id, segment.Array, channelId);
                if (!v)
                    result = v;
            }

            return result;
        }
#else
        public override bool ServerSend(int connectionId, int channelId, byte[] data) { return server.Send(connectionId, data, channelId); }
#endif

        public override bool ServerDisconnect(int connectionId)
        {
            return server.Disconnect(connectionId);
        }

        public override void ServerStop() { server.Stop(); }

        // common
        public override void Shutdown()
        {
            client.Disconnect();
            server.Stop();
        }

        public override int GetMaxPacketSize(int channelId)
        {
            switch (channelId)
            {
                case Channels.DefaultUnreliable:
                    return 1200; //UDP like - MTU size.

                case Channels.DefaultReliable:
                    return 1048576; //Reliable message send. Can send up to 1MB of data in a single message.

                default:
                    Debug.LogError("Unknown channel so uknown max size");
                    return 0;
            }
        }

        public override bool Available()
        {
            return true;
        }

        public override Uri ServerUri()
        {
            return new Uri("");
        }

        public class CommonServer
        {

            protected enum SteamChannels : int
            {
                SEND_DATA,
                SEND_INTERNAL = 100
            }

            protected enum InternalMessages : byte
            {
                CONNECT,
                ACCEPT_CONNECT,
                DISCONNECT
            }

            public static float secondsBetweenPolls = 0.03333f;

            //this is a callback from steam that gets registered and called when the server receives new connections
            private Callback<P2PSessionRequest_t> callback_OnNewConnection = null;
            //this is a callback from steam that gets registered and called when the ClientConnect fails
            private Callback<P2PSessionConnectFail_t> callback_OnConnectFail = null;

            readonly static protected byte[] connectMsgBuffer = new byte[] { (byte)InternalMessages.CONNECT };
            readonly static protected byte[] acceptConnectMsgBuffer = new byte[] { (byte)InternalMessages.ACCEPT_CONNECT };
            readonly static protected byte[] disconnectMsgBuffer = new byte[] { (byte)InternalMessages.DISCONNECT };
            public static EP2PSend[] channels;

            readonly static protected uint maxPacketSize = 1048576;
            readonly protected byte[] receiveBufferInternal = new byte[1];

            protected void deinitialise()
            {
                if (callback_OnNewConnection == null)
                {
                    callback_OnNewConnection.Dispose();
                    callback_OnNewConnection = null;
                }

                if (callback_OnConnectFail == null)
                {
                    callback_OnConnectFail.Dispose();
                    callback_OnConnectFail = null;
                }

            }

            protected virtual void initialise()
            {
                Debug.Log("initialise");

                if (SteamworksFoundationManager.Initialized)
                {
                    if (callback_OnNewConnection == null)
                    {
                        Debug.Log("initialise callback 1");
                        callback_OnNewConnection = Callback<P2PSessionRequest_t>.CreateGameServer(OnNewConnection);
                    }
                    if (callback_OnConnectFail == null)
                    {
                        Debug.Log("initialise callback 2");

                        callback_OnConnectFail = Callback<P2PSessionConnectFail_t>.CreateGameServer(OnConnectFail);
                    }
                }
                else
                {
                    Debug.LogError("Steam is not Initialized and so could not integrate with P2P");
                    return;
                }
            }

            protected void OnNewConnection(P2PSessionRequest_t result)
            {
                Debug.Log("OnNewConnection");
                OnNewConnectionInternal(result);
            }

            protected virtual void OnNewConnectionInternal(P2PSessionRequest_t result) { Debug.Log("OnNewConnectionInternal"); }

            protected virtual void OnConnectFail(P2PSessionConnectFail_t result)
            {
                Debug.Log("OnConnectFail " + result);
                throw new Exception("Failed to connect");
            }

            protected void SendInternal(CSteamID host, byte[] msgBuffer)
            {
                if (!SteamworksFoundationManager.Initialized)
                {
                    throw new ObjectDisposedException("Steamworks");
                }
                SteamGameServerNetworking.SendP2PPacket(host, msgBuffer, (uint)msgBuffer.Length, EP2PSend.k_EP2PSendReliable, (int)SteamChannels.SEND_INTERNAL);
            }

            protected bool ReceiveInternal(out uint readPacketSize, out CSteamID clientSteamID)
            {
                if (!SteamworksFoundationManager.Initialized)
                {
                    throw new ObjectDisposedException("Steamworks");
                }
                return SteamGameServerNetworking.ReadP2PPacket(receiveBufferInternal, 1, out readPacketSize, out clientSteamID, (int)SteamChannels.SEND_INTERNAL);
            }

            protected void Send(CSteamID host, byte[] msgBuffer, EP2PSend sendType, int channel)
            {
                if (!SteamworksFoundationManager.Initialized)
                {
                    throw new ObjectDisposedException("Steamworks");
                }
                if (channel >= channels.Length)
                {
                    channel = 0;
                }
                SteamGameServerNetworking.SendP2PPacket(host, msgBuffer, (uint)msgBuffer.Length, sendType, channel);
            }

            protected bool Receive(out uint readPacketSize, out CSteamID clientSteamID, out byte[] receiveBuffer, int channel)
            {
                if (!SteamworksFoundationManager.Initialized)
                {
                    throw new ObjectDisposedException("Steamworks");
                }

                uint packetSize;
                if (SteamGameServerNetworking.IsP2PPacketAvailable(out packetSize, channel) && packetSize > 0)
                {
                    receiveBuffer = new byte[packetSize];
                    return SteamGameServerNetworking.ReadP2PPacket(receiveBuffer, packetSize, out readPacketSize, out clientSteamID, channel);
                }

                receiveBuffer = null;
                readPacketSize = 0;
                clientSteamID = CSteamID.Nil;
                return false;
            }

            protected void CloseP2PSessionWithUser(CSteamID clientSteamID)
            {
                if (!SteamworksFoundationManager.Initialized)
                {
                    throw new ObjectDisposedException("Steamworks");
                }
                SteamGameServerNetworking.CloseP2PSessionWithUser(clientSteamID);
            }

            public uint GetMaxPacketSize(EP2PSend sendType)
            {
                switch (sendType)
                {
                    case EP2PSend.k_EP2PSendUnreliable:
                    case EP2PSend.k_EP2PSendUnreliableNoDelay:
                        return 1200; //UDP like - MTU size.

                    case EP2PSend.k_EP2PSendReliable:
                    case EP2PSend.k_EP2PSendReliableWithBuffering:
                        return maxPacketSize; //Reliable message send. Can send up to 1MB of data in a single message.

                    default:
                        Debug.LogError("Unknown type so uknown max size");
                        return 0;
                }

            }

            protected EP2PSend channelToSendType(int channelId)
            {
                if (channelId >= channels.Length)
                {
                    Debug.LogError("Unknown channel id, please set it up in the component, will now send reliably");
                    return EP2PSend.k_EP2PSendReliable;
                }
                return channels[channelId];
            }

        }

        public class CommonClient
        {

            protected enum SteamChannels : int
            {
                SEND_DATA,
                SEND_INTERNAL = 100
            }

            protected enum InternalMessages : byte
            {
                CONNECT,
                ACCEPT_CONNECT,
                DISCONNECT
            }

            public static float secondsBetweenPolls = 0.03333f;

            //this is a callback from steam that gets registered and called when the server receives new connections
            private Callback<P2PSessionRequest_t> callback_OnNewConnection = null;
            //this is a callback from steam that gets registered and called when the ClientConnect fails
            private Callback<P2PSessionConnectFail_t> callback_OnConnectFail = null;

            readonly static protected byte[] connectMsgBuffer = new byte[] { (byte)InternalMessages.CONNECT };
            readonly static protected byte[] acceptConnectMsgBuffer = new byte[] { (byte)InternalMessages.ACCEPT_CONNECT };
            readonly static protected byte[] disconnectMsgBuffer = new byte[] { (byte)InternalMessages.DISCONNECT };
            public static EP2PSend[] channels;

            readonly static protected uint maxPacketSize = 1048576;
            readonly protected byte[] receiveBufferInternal = new byte[1];

            protected void deinitialise()
            {
                if (callback_OnNewConnection == null)
                {
                    callback_OnNewConnection.Dispose();
                    callback_OnNewConnection = null;
                }

                if (callback_OnConnectFail == null)
                {
                    callback_OnConnectFail.Dispose();
                    callback_OnConnectFail = null;
                }

            }

            protected virtual void initialise()
            {
                Debug.Log("initialise");

                if (SteamworksFoundationManager.Initialized)
                {
                    if (callback_OnNewConnection == null)
                    {
                        Debug.Log("initialise callback 1");
                        callback_OnNewConnection = Callback<P2PSessionRequest_t>.Create(OnNewConnection);
                    }
                    if (callback_OnConnectFail == null)
                    {
                        Debug.Log("initialise callback 2");

                        callback_OnConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnConnectFail);
                    }
                }
                else
                {
                    Debug.LogError("Steam is not Initialized and so could not integrate with P2P");
                    return;
                }
            }

            protected void OnNewConnection(P2PSessionRequest_t result)
            {
                Debug.Log("OnNewConnection");
                OnNewConnectionInternal(result);
            }

            protected virtual void OnNewConnectionInternal(P2PSessionRequest_t result) { Debug.Log("OnNewConnectionInternal"); }

            protected virtual void OnConnectFail(P2PSessionConnectFail_t result)
            {
                Debug.Log("OnConnectFail " + result);
                throw new Exception("Failed to connect");
            }

            protected void SendInternal(CSteamID host, byte[] msgBuffer)
            {
                if (!SteamworksFoundationManager.Initialized)
                {
                    throw new ObjectDisposedException("Steamworks");
                }
                SteamNetworking.SendP2PPacket(host, msgBuffer, (uint)msgBuffer.Length, EP2PSend.k_EP2PSendReliable, (int)SteamChannels.SEND_INTERNAL);
            }

            protected bool ReceiveInternal(out uint readPacketSize, out CSteamID clientSteamID)
            {
                if (!SteamworksFoundationManager.Initialized)
                {
                    throw new ObjectDisposedException("Steamworks");
                }
                return SteamNetworking.ReadP2PPacket(receiveBufferInternal, 1, out readPacketSize, out clientSteamID, (int)SteamChannels.SEND_INTERNAL);
            }

            protected void Send(CSteamID host, byte[] msgBuffer, EP2PSend sendType, int channel)
            {
                if (!SteamworksFoundationManager.Initialized)
                {
                    throw new ObjectDisposedException("Steamworks");
                }
                if (channel >= channels.Length)
                {
                    channel = 0;
                }
                SteamNetworking.SendP2PPacket(host, msgBuffer, (uint)msgBuffer.Length, sendType, channel);
            }

            protected bool Receive(out uint readPacketSize, out CSteamID clientSteamID, out byte[] receiveBuffer, int channel)
            {
                if (!SteamworksFoundationManager.Initialized)
                {
                    throw new ObjectDisposedException("Steamworks");
                }

                uint packetSize;
                if (SteamNetworking.IsP2PPacketAvailable(out packetSize, channel) && packetSize > 0)
                {
                    receiveBuffer = new byte[packetSize];
                    return SteamNetworking.ReadP2PPacket(receiveBuffer, packetSize, out readPacketSize, out clientSteamID, channel);
                }

                receiveBuffer = null;
                readPacketSize = 0;
                clientSteamID = CSteamID.Nil;
                return false;
            }

            protected void CloseP2PSessionWithUser(CSteamID clientSteamID)
            {
                if (!SteamworksFoundationManager.Initialized)
                {
                    throw new ObjectDisposedException("Steamworks");
                }
                SteamNetworking.CloseP2PSessionWithUser(clientSteamID);
            }

            public uint GetMaxPacketSize(EP2PSend sendType)
            {
                switch (sendType)
                {
                    case EP2PSend.k_EP2PSendUnreliable:
                    case EP2PSend.k_EP2PSendUnreliableNoDelay:
                        return 1200; //UDP like - MTU size.

                    case EP2PSend.k_EP2PSendReliable:
                    case EP2PSend.k_EP2PSendReliableWithBuffering:
                        return maxPacketSize; //Reliable message send. Can send up to 1MB of data in a single message.

                    default:
                        Debug.LogError("Unknown type so uknown max size");
                        return 0;
                }

            }

            protected EP2PSend channelToSendType(int channelId)
            {
                if (channelId >= channels.Length)
                {
                    Debug.LogError("Unknown channel id, please set it up in the component, will now send reliably");
                    return EP2PSend.k_EP2PSendReliable;
                }
                return channels[channelId];
            }

        }

        public class Client : CommonClient
        {
            enum ConnectionState : byte
            {
                DISCONNECTED,
                CONNECTING,
                CONNECTED
            }

            public event Action<Exception> OnReceivedError;
            public event Action<ArraySegment<byte>, int> OnReceivedData;
            public event Action OnConnected;
            public event Action OnDisconnected;

            //how long to wait before connect timeout
            public static int clientConnectTimeoutMS = 25000;

            private ConnectionState state = ConnectionState.DISCONNECTED;
            private CSteamID hostSteamID = CSteamID.Nil;

            public bool Connecting { get { return state == ConnectionState.CONNECTING; } private set { if (value) state = ConnectionState.CONNECTING; } }
            public bool Connected
            {
                get { return state == ConnectionState.CONNECTED; }
                private set
                {
                    if (value)
                    {
                        bool wasConnecting = Connecting;
                        state = ConnectionState.CONNECTED;
                        if (wasConnecting)
                        {
                            OnConnected?.Invoke();
                        }

                    }
                }
            }
            public bool Disconnected
            {
                get { return state == ConnectionState.DISCONNECTED; }
                private set
                {
                    if (value)
                    {
                        bool wasntDisconnected = !Disconnected;
                        state = ConnectionState.DISCONNECTED;
                        if (wasntDisconnected)
                        {
                            OnDisconnected?.Invoke();
                        }

                        deinitialise();
                    }
                }
            }

            //internally used while connecting. Subscribe to onconnect and signal this task
            TaskCompletionSource<Task> connectedComplete;
            private void setConnectedComplete()
            {
                connectedComplete.SetResult(connectedComplete.Task);
            }

            System.Threading.CancellationTokenSource cancelToken;
            public async void Connect(string host)
            {
                cancelToken = new System.Threading.CancellationTokenSource();
                // not if already started
                if (!Disconnected)
                {
                    // exceptions are better than silence
                    Debug.LogError("Client already connected or connecting");
                    OnReceivedError?.Invoke(new Exception("Client already connected"));
                    return;
                }

                // We are connecting from now until Connect succeeds or fails
                Connecting = true;

                initialise();

                try
                {
                    hostSteamID = new CSteamID(Convert.ToUInt64(host));

                    InternalReceiveLoop();

                    connectedComplete = new TaskCompletionSource<Task>();

                    OnConnected += setConnectedComplete;
                    CloseP2PSessionWithUser(hostSteamID);

                    //Send a connect message to the steam client - this requests a connection with them
                    SendInternal(hostSteamID, connectMsgBuffer);

                    Task connectedCompleteTask = connectedComplete.Task;

                    if (await Task.WhenAny(connectedCompleteTask, Task.Delay(clientConnectTimeoutMS, cancelToken.Token)) != connectedCompleteTask)
                    {
                        //Timed out waiting for connection to complete
                        OnConnected -= setConnectedComplete;

                        Exception e = new Exception("Timed out while connecting");
                        OnReceivedError?.Invoke(e);
                        throw e;
                    }

                    OnConnected -= setConnectedComplete;

                    await ReceiveLoop();
                }
                catch (FormatException)
                {
                    Debug.LogError("Failed to connect ERROR passing steam ID address");
                    OnReceivedError?.Invoke(new Exception("ERROR passing steam ID address"));
                    return;
                }
                catch (Exception ex)
                {
                    Debug.LogError("Failed to connect " + ex);
                    OnReceivedError?.Invoke(ex);
                }
                finally
                {
                    Disconnect();
                }

            }

            public async void Disconnect()
            {
                if (!Disconnected)
                {
                    //Disconnected = true;
                    SendInternal(hostSteamID, disconnectMsgBuffer);
                    cancelToken.Cancel();
                    state = ConnectionState.DISCONNECTED;
                    //Wait a short time before calling steams disconnect function so the message has time to go out
                    await Task.Delay(100);
                    CloseP2PSessionWithUser(hostSteamID);
                }
            }

            private async Task ReceiveLoop()
            {
                Debug.Log("ReceiveLoop Start");

                uint readPacketSize;
                CSteamID clientSteamID;

                try
                {
                    byte[] receiveBuffer;

                    while (Connected)
                    {
                        for (int i = 0; i < channels.Length; i++)
                        {
                            while (Receive(out readPacketSize, out clientSteamID, out receiveBuffer, i))
                            {
                                if (readPacketSize == 0)
                                {
                                    continue;
                                }
                                if (clientSteamID != hostSteamID)
                                {
                                    Debug.LogError("Received a message from an unknown");
                                    continue;
                                }
                                // we received some data,  raise event
                                OnReceivedData?.Invoke(new ArraySegment<byte>(receiveBuffer), i);
                            }
                        }
                        //not got a message - wait a bit more
                        await Task.Delay(TimeSpan.FromSeconds(secondsBetweenPolls));
                    }
                }
                catch (ObjectDisposedException) { }

                Debug.Log("ReceiveLoop Stop");
            }

            protected override void OnNewConnectionInternal(P2PSessionRequest_t result)
            {
                Debug.Log("OnNewConnectionInternal in client");

                if (hostSteamID == result.m_steamIDRemote)
                {
                    SteamNetworking.AcceptP2PSessionWithUser(result.m_steamIDRemote);
                }
                else
                {
                    Debug.LogError("");
                }
            }

            //start a async loop checking for internal messages and processing them. This includes internal connect negotiation and disconnect requests so runs outside "connected"
            private async void InternalReceiveLoop()
            {
                Debug.Log("InternalReceiveLoop Start");

                uint readPacketSize;
                CSteamID clientSteamID;

                try
                {
                    while (!Disconnected)
                    {
                        while (ReceiveInternal(out readPacketSize, out clientSteamID))
                        {
                            if (readPacketSize != 1)
                            {
                                continue;
                            }
                            if (clientSteamID != hostSteamID)
                            {
                                Debug.LogError("Received an internal message from an unknown");
                                continue;
                            }
                            switch (receiveBufferInternal[0])
                            {
                                case (byte)InternalMessages.ACCEPT_CONNECT:
                                    Connected = true;
                                    break;
                                case (byte)InternalMessages.DISCONNECT:
                                    Disconnected = true;
                                    break;
                            }
                        }
                        //not got a message - wait a bit more
                        await Task.Delay(TimeSpan.FromSeconds(secondsBetweenPolls));
                    }
                }
                catch (ObjectDisposedException) { }

                Debug.Log("InternalReceiveLoop Stop");
            }

            // send the data or throw exception
            public bool Send(byte[] data, int channelId)
            {
                if (Connected)
                {
                    Send(hostSteamID, data, channelToSendType(channelId), channelId);
                    return true;
                }
                else
                {
                    throw new Exception("Not Connected");
                    //return false;
                }
            }

        }

        internal class SteamClient
        {
            public enum ConnectionState
            {
                CONNECTED,
                DISCONNECTING,
            }

            public CSteamID steamID;
            public ConnectionState state;
            public int connectionID;
            public float timeIdle = 0;

            public SteamClient(ConnectionState state, CSteamID steamID, int connectionID)
            {
                this.state = state;
                this.steamID = steamID;
                this.connectionID = connectionID;
                this.timeIdle = 0;
            }
        }

        internal class SteamConnectionMap : IEnumerable<KeyValuePair<int, SteamClient>>
        {
            public readonly Dictionary<CSteamID, SteamClient> fromSteamID = new Dictionary<CSteamID, SteamClient>();
            public readonly Dictionary<int, SteamClient> fromConnectionID = new Dictionary<int, SteamClient>();

            public SteamConnectionMap()
            {
            }

            public int Count
            {
                get { return fromSteamID.Count; }
            }

            public SteamClient Add(CSteamID steamID, int connectionID, SteamClient.ConnectionState state)
            {
                var newClient = new SteamClient(state, steamID, connectionID);
                fromSteamID.Add(steamID, newClient);
                fromConnectionID.Add(connectionID, newClient);

                return newClient;
            }

            public void Remove(SteamClient steamClient)
            {
                fromSteamID.Remove(steamClient.steamID);
                fromConnectionID.Remove(steamClient.connectionID);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<KeyValuePair<int, SteamClient>> GetEnumerator()
            {
                return fromConnectionID.GetEnumerator();
            }
        }

        public class Server : CommonServer
        {

            enum ConnectionState : byte
            {
                OFFLINE,
                LISTENING
            }

            public event Action<int> OnConnected;
            public event Action<int, ArraySegment<byte>, int> OnReceivedData;
            public event Action<int> OnDisconnected;
            public event Action<int, Exception> OnReceivedError;

            private ConnectionState state = ConnectionState.OFFLINE;
            private SteamConnectionMap steamConnectionMap;
            private int nextConnectionID;
            private int maxConnections;

            public bool Listening { get { return state == ConnectionState.LISTENING; } private set { if (value) state = ConnectionState.LISTENING; } }
            public bool Offline
            {
                get { return state == ConnectionState.OFFLINE; }
                private set
                {
                    if (value)
                    {
                        state = ConnectionState.OFFLINE;

                        deinitialise();
                    }
                }
            }

            public async void Listen(int maxConnections = int.MaxValue)
            {
                Debug.Log("Listen Start");
                //todo check we are not already listening ?

                initialise();
                Listening = true;
                this.maxConnections = maxConnections;

                InternalReceiveLoop();

                await ReceiveLoop();

                Debug.Log("Listen Stop");
            }

            protected override void OnNewConnectionInternal(P2PSessionRequest_t result)
            {
                Debug.Log("OnNewConnectionInternal in server");

                SteamGameServerNetworking.AcceptP2PSessionWithUser(result.m_steamIDRemote);
            }


            /// <summary>
            /// start a async loop checking for internal messages and processing them. This includes internal connect negotiation and disconnect requests so runs outside "connected"
            /// </summary>
            /// <returns></returns>
            private async Task InternalReceiveLoop()
            {
                Debug.Log("InternalReceiveLoop Start");

                uint readPacketSize;
                CSteamID clientSteamID;

                try
                {
                    while (!Offline)
                    {

                        while (ReceiveInternal(out readPacketSize, out clientSteamID))
                        {
                            Debug.Log("InternalReceiveLoop - data");
                            if (readPacketSize != 1)
                            {
                                continue;
                            }
                            Debug.Log("InternalReceiveLoop - received " + receiveBufferInternal[0]);
                            switch (receiveBufferInternal[0])
                            {
                                //requesting to connect to us
                                case (byte)InternalMessages.CONNECT:
                                    if (steamConnectionMap.Count >= maxConnections)
                                    {
                                        SendInternal(clientSteamID, disconnectMsgBuffer);
                                        continue;
                                        //too many connections, reject
                                    }
                                    SendInternal(clientSteamID, acceptConnectMsgBuffer);

                                    int connectionId = nextConnectionID++;
                                    steamConnectionMap.Add(clientSteamID, connectionId, SteamClient.ConnectionState.CONNECTED);
                                    OnConnected?.Invoke(connectionId);
                                    break;

                                //asking us to disconnect
                                case (byte)InternalMessages.DISCONNECT:
                                    try
                                    {
                                        SteamClient steamClient = steamConnectionMap.fromSteamID[clientSteamID];
                                        steamConnectionMap.Remove(steamClient);
                                        OnDisconnected?.Invoke(steamClient.connectionID);
                                        CloseP2PSessionWithUser(steamClient.steamID);
                                    }
                                    catch (KeyNotFoundException)
                                    {
                                        //we have no idea who this connection is
                                        Debug.LogError("Trying to disconnect a client thats not known SteamID " + clientSteamID);
                                    }

                                    break;
                            }
                        }

                        //not got a message - wait a bit more
                        await Task.Delay(TimeSpan.FromSeconds(secondsBetweenPolls));
                    }
                }
                catch (ObjectDisposedException) { }

                Debug.Log("InternalReceiveLoop Stop");
            }

            private async Task ReceiveLoop()
            {
                Debug.Log("ReceiveLoop Start");

                uint readPacketSize;
                CSteamID clientSteamID;

                try
                {
                    byte[] receiveBuffer;
                    while (!Offline)
                    {
                        for (int i = 0; i < channels.Length; i++)
                        {
                            while (Receive(out readPacketSize, out clientSteamID, out receiveBuffer, i))
                            {
                                if (readPacketSize == 0)
                                {
                                    continue;
                                }

                                try
                                {
                                    int connectionId = steamConnectionMap.fromSteamID[clientSteamID].connectionID;
                                    // we received some data,  raise event
                                    OnReceivedData?.Invoke(connectionId, new ArraySegment<byte>(receiveBuffer), i);
                                }
                                catch (KeyNotFoundException)
                                {
                                    CloseP2PSessionWithUser(clientSteamID);
                                    //we have no idea who this connection is
                                    Debug.LogError("Data received from steam client thats not known " + clientSteamID);
                                }
                            }
                        }
                        //not got a message - wait a bit more
                        await Task.Delay(TimeSpan.FromSeconds(secondsBetweenPolls));
                    }
                }
                catch (ObjectDisposedException) { }

                Debug.Log("ReceiveLoop Stop");
            }

            // check if the server is running
            public bool Active
            {
                get { return Listening; }
            }

            public void Stop()
            {
                Debug.LogWarning("Server Stop");
                // only if started
                if (!Active) return;

                //Disconnect all clients
                foreach(var steamClient in steamConnectionMap)
                {
                    Disconnect(steamClient.Value);
                }

                Offline = true;

                deinitialise();
                Debug.Log("Server Stop Finished");
            }

            // disconnect (kick) a client
            public bool Disconnect(int connectionId)
            {
                try
                {
                    SteamClient steamClient = steamConnectionMap.fromConnectionID[connectionId];
                    Disconnect(steamClient);
                    return true;
                }
                catch (KeyNotFoundException)
                {
                    //we have no idea who this connection is
                    Debug.LogWarning("Trying to disconnect a connection thats not known " + connectionId);
                }
                return false;
            }

            private async void Disconnect(SteamClient steamClient)
            {
                if (steamClient.state != SteamClient.ConnectionState.CONNECTED)
                {
                    return;
                }

                SendInternal(steamClient.steamID, disconnectMsgBuffer);
                steamClient.state = SteamClient.ConnectionState.DISCONNECTING;

                //Wait a short time before calling steams disconnect function so the message has time to go out
                await Task.Delay(100);
                steamConnectionMap.Remove(steamClient);
                OnDisconnected?.Invoke(steamClient.connectionID);
                CloseP2PSessionWithUser(steamClient.steamID);
            }

            public bool Send(int connectionId, byte[] data, int channelId = 0)
            {
                try
                {
                    SteamClient steamClient = steamConnectionMap.fromConnectionID[connectionId];
                    //will default to reliable at channel 0 if sent on an unknown channel
                    Send(steamClient.steamID, data, channelToSendType(channelId), channelId >= channels.Length ? 0 : channelId);
                    return true;
                }
                catch (KeyNotFoundException)
                {
                    //we have no idea who this connection is
                    Debug.LogError("Tryign to Send on a connection thats not known " + connectionId);
                    return false;
                }

            }

            public string ServerGetClientAddress(int connectionId)
            {
                try
                {
                    SteamClient steamClient = steamConnectionMap.fromConnectionID[connectionId];
                    return steamClient.steamID.ToString();
                }
                catch (KeyNotFoundException)
                {
                    //we have no idea who this connection is
                    Debug.LogError("Trying to get info on an unknown connection " + connectionId);
                }

                return null;
            }

            protected override void initialise()
            {
                base.initialise();

                nextConnectionID = 1;
                steamConnectionMap = new SteamConnectionMap();
            }
        }
    }
}
#endif