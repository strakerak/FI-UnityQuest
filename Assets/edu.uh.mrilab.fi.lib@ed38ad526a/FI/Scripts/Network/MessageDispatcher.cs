using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace fi {
    [System.Serializable]
    public class AuthenticationResponseEvent : UnityEvent<AuthenticationResponse> { }

    [System.Serializable]
    public class ApplicationResponseEvent : UnityEvent<ApplicationResponse> { }

    [System.Serializable]
    public class ModuleResponseEvent : UnityEvent<ModuleResponse> { }

    [System.Serializable]
    public class DataResponseEvent : UnityEvent<DataResponse> { }

    [System.Serializable]
    public class ErrorResponseEvent : UnityEvent<Response> { }

    public class MessageDispatcher : MonoBehaviour {
        /// <summary>
        /// Triggered when a new authentication response is available.
        /// </summary>
        public AuthenticationResponseEvent NewAuthenticationResponse;

        /// <summary>
        /// Triggered when a new application response is available.
        /// </summary>
        public ApplicationResponseEvent NewApplicationResponse;

        /// <summary>
        /// Triggered when a new module response is available.
        /// </summary>
        public ModuleResponseEvent NewModuleResponse;

        /// <summary>
        /// Triggered when a new data response is available.
        /// </summary>
        public DataResponseEvent NewDataResponse;

        /// <summary>
        /// Triggered when an erroneous response is available.
        /// </summary>
        public ErrorResponseEvent NewErrorResponse;

        /// <summary>
        /// Messages that have been parsed are placed here.
        /// </summary>
        ConcurrentQueue<Response> ParsedMessages { get; set; } = new ConcurrentQueue<Response>();

        /// <summary>
        /// Dispatches the message by calling the appropriate event.
        /// </summary>
        protected virtual void Update() {
            if (!ParsedMessages.IsEmpty) {
                Response response;
                ParsedMessages.TryDequeue(out response);

                if (response == null) {
                    Debug.LogWarning("Failed to dequeue response");
                }

                if (response.ParseError) {
                    Debug.LogWarning(string.Format("Failed to parse response of type {0} because: {1}", response.MessageType, response.ParseErrorMessage));
                } else {
                    switch (response.MessageType) {
                        case EMessageType.AUTHENTICATION:
                            NewAuthenticationResponse?.Invoke(response as AuthenticationResponse);
                            break;
                        case EMessageType.APPLICATION:
                            NewApplicationResponse?.Invoke(response as ApplicationResponse);
                            break;
                        case EMessageType.MODULE:
                            NewModuleResponse?.Invoke(response as ModuleResponse);
                            break;
                        case EMessageType.DATA:
                            NewDataResponse?.Invoke(response as DataResponse);
                            break;
                        case EMessageType.ERROR:
                            NewErrorResponse?.Invoke(response);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Parses the string response and enqueues it for dispatch.
        /// </summary>
        /// <param name="message">The response as a string.</param>
        public virtual void onServerMessage(Message message) {
            Task.Run(() => {
                SimpleJSON.JSONObject msgObject = SimpleJSON.JSON.Parse(message.Info).AsObject;
                if (!msgObject.IsNull) {
                    Response response = MessageParser.parseResponse(msgObject);
                    response.Payload = message.Paylaod;
                    ParsedMessages.Enqueue(response);
                } else {
                    Debug.LogError("Failed to parse message");
                }
            });
        }
    }
}