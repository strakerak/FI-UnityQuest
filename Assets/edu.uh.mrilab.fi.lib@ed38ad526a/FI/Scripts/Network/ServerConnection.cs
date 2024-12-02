using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace fi {
    [System.Serializable]
    public class ServerMessage : UnityEvent<fi.Message> { }

    [System.Serializable]
    public class ServerStatus : UnityEvent<ServerConnection.ConnectionState> { }

    /// <summary>
    /// Communicates with the FI3D server.
    /// This class implements methods to work with Unity C# and Microsoft UWP C#.
    /// 
    /// TODO: The FI3D communication protocol was updated with the ability to 
    /// separate a message into info and payload. Currently, there is no support
    /// to send payloads to the server. At the moment, there is no need for
    /// sending payloads. However, this may be needed in the future if device
    /// sensor data should be transmitted over.
    /// </summary>
    public class ServerConnection : MonoBehaviour {
        /// <summary>
        /// The different states of the connection
        /// </summary>
        [System.Serializable]
        public enum ConnectionState {
            UNKNOWN,
            CONNECTED,
            CONNECTING,
            DISCONNECTED,
            ERROR_NO_SERVER_RESPONSE,
            ERROR_UNKNOWN
        }

        /// <summary>
        /// There can only be one object that sends messages.
        /// This INSTANCE represents the singleton.
        /// </summary>
        static ServerConnection INSTANCE;

        /// <summary>
        /// The ID given by the server. Uniquely identifes this connection.
        /// </summary>
        public static string ConnectionID { get; protected set; }

        /// <summary>
        /// Whether the server acknowledged our authentication.
        /// </summary>
        public static bool IsAuthenticated { get; protected set; }

        /// <summary>
        /// The connection information of the FI3D server. If the connection
        /// is currently active, the assignment is ignored.
        /// </summary>
        private static Connection connection = new Connection();
        public static Connection Connection {
            get {
                return connection;
            } set {
                // Do not change connection if there is an active connection
                if (ServerConnection.IsConnected) {
                    return;
                }

                connection.Name = value.Name;
                connection.Password = value.Password;
                connection.IP = value.IP;
                connection.Port = value.Port;
            }
        }

        /// <summary>
        /// Event that indicates a new message has been received.
        /// </summary>
        public ServerMessage ServerMessage = new ServerMessage();

        /// <summary>
        /// Event that states the status of the server connection.
        /// </summary>
        public ServerStatus ServerStatus = new ServerStatus();

        /// <summary>
        /// Flag used internally for when the connection is currently established. 
        /// </summary>
        public static bool IsConnected { get; private set; } = false;

        /// <summary>
        /// The token used to cancel the server connection and terminate
        /// the task.
        /// </summary>
        CancellationTokenSource CancellationTokenSource;

        /// <summary>
        /// The task object where the receiving of data occurs.
        /// </summary>
        Task ServerTask { get; set; }

        TcpClient Client { get; set; }

        /// <summary>
        /// Messages to be sent to server are queued here.
        /// </summary>
        ConcurrentQueue<SimpleJSON.JSONObject> RequestQueue { get; set; } = new ConcurrentQueue<SimpleJSON.JSONObject>();

        /// <summary>
        /// Messages to be sent to server are queued here.
        /// </summary>
        ConcurrentQueue<fi.Message> ResponseQueue { get; set; } = new ConcurrentQueue<fi.Message>();

        /// <summary>
        /// Authentication responses that have to be handled by the this TCP connection.
        /// </summary>
        ConcurrentQueue<fi.AuthenticationResponse> AuthenticationResponses { get; set; } = new ConcurrentQueue<fi.AuthenticationResponse>();

        /// <summary>
        /// Stores the status of the server so that it is given as part of the ServerStatus event.
        /// </summary>
        ConcurrentQueue<ConnectionState> StatusMessages { get; set; } = new ConcurrentQueue<ConnectionState>();

        /// <summary>
        /// Creates the ServerConnection singleton.
        /// </summary>
        void Awake() {
            if (INSTANCE != null) {
                Destroy(gameObject);
            } else {
                INSTANCE = this;
                GameObject.DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Sets the state of the connection to disconnected.
        /// </summary>
        void Start() {
            StatusMessages.Enqueue(ConnectionState.DISCONNECTED);
        }

        /// <summary>
        /// Checks for queued messages from the connection thread to trigger
        /// the appropriate events.
        /// </summary>
        void Update() {
            // Events related to the connection status
            if (!StatusMessages.IsEmpty) {
                ConnectionState status;
                StatusMessages.TryDequeue(out status);
                if (status != ConnectionState.UNKNOWN) {
                    ServerStatus?.Invoke(status);
                }
            }

            // Checks if there are any authentication requests to execute
            if (!AuthenticationResponses.IsEmpty) {
                fi.AuthenticationResponse response;
                AuthenticationResponses.TryDequeue(out response);
                if (response != null) {
                    this.handleAuthenticationResponse(response);
                }
            }

            // Checks for requests that should be sent to the server
            if (!RequestQueue.IsEmpty) {
                SimpleJSON.JSONObject message;
                RequestQueue.TryDequeue(out message);
                if (message != null) {
                    this.sendData(message);
                }
            }

            // Checks for messages received from the server
            if (!ResponseQueue.IsEmpty) {
                fi.Message response;
                ResponseQueue.TryDequeue(out response);
                if (response != null) {
                    ServerMessage?.Invoke(response);
                }
            }
        }

        /// <summary>
        /// When a new authentication response is received, enqueue it to be 
        /// executed on the next frame.
        /// </summary>
        /// <param name="response">The authentication response.</param>
        public void onNewAuthenticationResponse(fi.AuthenticationResponse response) {
            AuthenticationResponses.Enqueue(response);
        }

        /// <summary>
        /// Attempts to connect to the server with given connection 
        /// information. If the connection is already established, this function
        /// does nothing.
        /// </summary>
        public static void Connect(Connection connection) {
            if (connection == null) {
                Debug.LogWarning("Failed to connect because given connection parameters are null");
                return;
            }

            Connection = connection;
            INSTANCE.Connect();
        }

        /// <summary>
        /// Attempts to connect to the server with pre-stored connection 
        /// information. If the connection is already established, this function
        /// does nothing.
        /// </summary>
        public void Connect() {
            if (!ServerConnection.IsConnected) {
                CancellationTokenSource = new CancellationTokenSource();
                ServerTask = Task.Factory.StartNew(listenForDataAsync, TaskCreationOptions.LongRunning);
            }
        }

        

        /// <summary>
        ///  Checks the socket's stream for new messages to then process.
        /// </summary>
        async private void listenForDataAsync() {
            StatusMessages.Enqueue(ConnectionState.CONNECTING);
            ServerConnection.IsConnected = true;
            Debug.Log(string.Format("Connecting to server: IP={0}, Port={1}", Connection.IP, Connection.Port));
            try {

                Client = new TcpClient(Connection.IP, Connection.Port);
                StatusMessages.Enqueue(ConnectionState.CONNECTED);
                Debug.Log("Connection established");

                byte[] payloadFlag = new byte[1];
                byte[] length = new byte[4];
                byte[] bytes = new byte[0];
                bool hasPayload = false;
                int infoLength = -1;
                int payloadLength = -1;

                using (NetworkStream stream = Client.GetStream()) {
                    CancellationTokenSource.Token.Register(stream.Close);
                    while (true) {
                        // Check if incoming message has a payload
                        int read = await stream.ReadAsync(payloadFlag, 0, 1);
                        hasPayload = BitConverter.ToBoolean(payloadFlag, 0);

                        // Wait for 4 bytes to get message size
                        int infoLengthBytesRead = 0;
                        while (infoLengthBytesRead != 4) {
                            read = await stream.ReadAsync(length, infoLengthBytesRead, 4 - infoLengthBytesRead);
                            infoLengthBytesRead += read;
                        }
                        infoLength = BitConverter.ToInt32(length, 0);
                        Debug.Log(string.Format("Receiving message with info length: {0}", infoLength));

                        // Wait for 4 bytes to get payload if there is a payload
                        if (hasPayload) {
                            // Wait for 4 bytes to get message size
                            int payloadLengthBytesRead = 0;
                            while (payloadLengthBytesRead != 4) {
                                read = await stream.ReadAsync(length, payloadLengthBytesRead, 4 - payloadLengthBytesRead);
                                payloadLengthBytesRead += read;
                            }
                            payloadLength = BitConverter.ToInt32(length, 0);
                            Debug.Log(string.Format("Receiving message with payload length: {0}", payloadLength));
                        }

                        // Read the info part of the message
                        bytes = new byte[infoLength];
                        int readBytes = 0;
                        while (infoLength != 0) {
                            read = await stream.ReadAsync(bytes, readBytes, infoLength);
                            readBytes += read;
                            infoLength -= read;
                        }
                        string info = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
                        Debug.Log(string.Format("New message with info: {0}", info));

                        // Read the payload part, if there is a paylaod
                        if (hasPayload) {
                            bytes = new byte[payloadLength];
                            readBytes = 0;
                            while (payloadLength != 0) {
                                read = await stream.ReadAsync(bytes, readBytes, payloadLength);
                                readBytes += read;
                                payloadLength -= read;
                            }
                        }

                        fi.Message message = new fi.Message();
                        message.HasPayload = hasPayload;
                        message.Info = info;
                        message.Paylaod = bytes;

                        // Trigger event that a new message has arrived
                        ResponseQueue.Enqueue(message);
                    }
                }
            } catch (SocketException exp) {
                Debug.Log("Socket exception: " + exp.Message);
                StatusMessages.Enqueue(ConnectionState.ERROR_NO_SERVER_RESPONSE);
            } catch (ObjectDisposedException exp) {
                Debug.Log("Diposed exception: " + exp.Message);
                StatusMessages.Enqueue(ConnectionState.DISCONNECTED);
            } catch (System.Exception exp) {
                Debug.Log("Exception: " + exp.Message);
                StatusMessages.Enqueue(ConnectionState.ERROR_UNKNOWN);
            }

            Client = null;

            Debug.Log("Connection terminated");
            ConnectionID = "";
            IsAuthenticated = false;
            CancellationTokenSource = null;
            ServerConnection.IsConnected = false;
        }

        /// <summary>
        /// Converts the given JSONObject to a byte array and sends to the server.
        /// </summary>
        /// <param name="requestObject">The message to send.</param>
        private void sendData(SimpleJSON.JSONObject requestObject, bool ignoreAuthentication = false) {
            if (!IsAuthenticated && !ignoreAuthentication) {
                Debug.Log("Failed to send message because client is not authenticated");
                Debug.Log(string.Format("Failed message: {0}", requestObject.ToString()));
                return;
            }

            if (Client != null) {
                NetworkStream stream = Client.GetStream();
                if (stream.CanWrite) {
                    string request = requestObject.ToString();

                    byte[] payloadFlag = BitConverter.GetBytes(false);
                    byte[] msg = System.Text.ASCIIEncoding.ASCII.GetBytes(request);
                    byte[] msgSize = BitConverter.GetBytes(msg.Length);

                    Debug.Log(string.Format("Sending message of size {0} -: {1}", msg.Length + 4, requestObject.ToString()));

                    // TODO: For now, all messages sent have no payload so a 0 is prepended
                    stream.Write(payloadFlag, 0, payloadFlag.Length);
                    stream.Write(msgSize, 0, msgSize.Length);
                    stream.Write(msg, 0, msg.Length);
                    stream.Flush();
                } else {
                    Debug.LogError("Failed to send message because stream is blocked");
                }
            } else {
                Debug.LogError("Failed to send message because there's no conneciton");
            }
        }

        /// <summary>
        /// Parses and executes an authentication message.
        /// </summary>
        /// <param name="messageObject"></param>
        private void handleAuthenticationResponse(fi.AuthenticationResponse authenticationResponse) {
            switch (authenticationResponse.ResponseStatus) {
                case (int)fi.EResponseStatus.INFO_REQUIRED:
                    ConnectionID = authenticationResponse.ClientID;
                    SimpleJSON.JSONObject request = RequestMaker.makeAuthenticationRequest("admin");
                    sendData(request, true);
                    break;
                case (int)fi.EResponseStatus.ERROR:
                    //TODO: Error authenticating. Provide feedback to user.
                    break;
                case (int)fi.EResponseStatus.SUCCESS:
                    IsAuthenticated = true;
                    break;
            }
        }

        /// <summary>
        /// Adds given message to the queue for delivery.
        /// </summary>
        /// <param name="message">The message to send.</param>
        static public void sendMessage(SimpleJSON.JSONObject message) {
            if (ServerConnection.IsConnected) {
                INSTANCE.RequestQueue.Enqueue(message);
            } else {
                Debug.LogWarning("Failed to send message because there is no active connection");
            }
        }

        /// <summary>
        /// Sets the closeSocket flag to false in order for the client to terminate connection with the server.
        /// </summary>
        static public void stopConnection() {
            if (INSTANCE.CancellationTokenSource != null) {
                Debug.Log("Stopping Server Connection");
                INSTANCE.CancellationTokenSource.Cancel();
            }
        }
    }
}