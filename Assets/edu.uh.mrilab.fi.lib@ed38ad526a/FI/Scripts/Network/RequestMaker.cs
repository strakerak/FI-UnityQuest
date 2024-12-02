using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimpleJSON;

namespace fi {
    class RequestMaker {
        /// <summary>
        /// Helper function that inserts common values to all requests.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="messageType">The type of request this is.</param>
        /// <param name="message">Optional message to log.</param>
        static public void insertHeader(JSONObject request, EMessageType messageType, string message = "") {
            request["ClientID"] = ServerConnection.ConnectionID;
            request["MessageType"] = (int)messageType;
            request["Message"] = message;
        }

        /// <summary>
        /// Makes a request to authenticate, providing the password of the
        /// server.
        /// </summary>
        /// <param name="password">The password of the server.</param>
        /// <param name="message">Optional message to log.</param>
        /// <returns>The request object.</returns>
        static public JSONObject makeAuthenticationRequest(string password, string message = "") {
            JSONObject request = new JSONObject();
            RequestMaker.insertHeader(request, EMessageType.AUTHENTICATION, message);
            request["Password"] = password;
            return request;
        }

        /// <summary>
        /// Makes an application request.
        /// </summary>
        /// <param name="actionType">The type of application request.</param>
        /// <param name="moduleName">The name of the module, where it applies.</param>
        /// <param name="moduleID">The ID of the module, where it applies.</param>
        /// <param name="message">Optional message to log.</param>
        /// <returns>The request object.</returns>
        static public JSONObject makeApplicationRequest(EApplicationRequest actionType, string moduleName, string moduleID, string message = "") {
            JSONObject applicationParams = new JSONObject();
            applicationParams["ActionType"] = (int)actionType;
            applicationParams["ModuleName"] = moduleName;
            applicationParams["ModuleID"] = moduleID;

            JSONObject request = new JSONObject();
            RequestMaker.insertHeader(request, EMessageType.APPLICATION, message);
            request["ApplicationParams"] = applicationParams;

            return request;
        }

        /// <summary>
        /// Makes an application request to log the given log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">Optional message to log.</param>
        /// <returns></returns>
        static public JSONObject makeServerLogRequest(string log, string message = "") {
            JSONObject applicationParams = new JSONObject();
            applicationParams["ActionType"] = (int)EApplicationRequest.LOG;
            applicationParams["Log"] = log;

            JSONObject request = new JSONObject();
            RequestMaker.insertHeader(request, EMessageType.APPLICATION, message);
            request["ApplicationParams"] = applicationParams;

            return request;
        }

        /**** Module Requests *****/
        // TODO: At the moment, only module interaction requests are available.
        // Need to implement other interactions.

        /// <summary>
        /// Creates a request to subscribe to the given module.
        /// </summary>
        /// <param name="moduleID">The ID of the module being subscribed to.</param>
        /// <param name="message">Optional message that can be logged.</param>
        /// <returns>The request object.</returns>
        static public JSONObject makeModuleSubscriptionRequest(string moduleID, string message = "") {
            JSONObject moduleParams = new JSONObject();
            moduleParams["ModuleID"] = moduleID;
            moduleParams["RequestID"] = (int)EModuleRequest.SUBSCRIBE_TO_MODULE;

            JSONObject request = new JSONObject();
            RequestMaker.insertHeader(request, EMessageType.MODULE, message);
            request["ModuleParams"] = moduleParams;
            return request;
        }

        /// <summary>
        /// Creates a request to unsubscribe from the given module.
        /// </summary>
        /// <param name="moduleID">The ID of the module being unsubscribed from.</param>
        /// <param name="message">Optional message that can be logged.</param>
        /// <returns>The request object.</returns>
        static public JSONObject makeModuleUnsubscriptionRequest(string moduleID, string message = "") {
            JSONObject moduleParams = new JSONObject();
            moduleParams["ModuleID"] = moduleID;
            moduleParams["RequestID"] = (int)EModuleRequest.UNSUBSCRIBE_TO_MODULE;

            JSONObject request = new JSONObject();
            RequestMaker.insertHeader(request, EMessageType.MODULE, message);
            request["ModuleParams"] = moduleParams;
            return request;
        }

        /// <summary>
        /// Creates a request that modifies the interaction of the given ID by the given value.
        /// </summary>
        /// <param name="moduleID">The ID of the module this interaction belongs to.</param>
        /// <param name="moduleInteractionID">The ID of the module interaction.</param>
        /// <param name="moduleInteractionValue">The new value of the module interaction.</param>
        /// <param name="message">Optional message that can be logged.</param>
        /// <returns></returns>
        static public JSONObject makeModuleInteractionRequest(string moduleID, string moduleInteractionID, JSONNode moduleInteractionValue, string message = "") {
            JSONObject moduleParams = new JSONObject();
            moduleParams["ModuleID"] = moduleID;
            moduleParams["RequestID"] = (int)EModuleRequest.MODULE_INTERACTION;
            moduleParams["InteractionID"] = moduleInteractionID;
            moduleParams["InteractionValue"] = moduleInteractionValue;

            JSONObject request = new JSONObject();
            RequestMaker.insertHeader(request, EMessageType.MODULE, message);
            request["ModuleParams"] = moduleParams;
            return request;
        }

        /**** Data Requests ****/
        /// <summary>
        /// Requests the image with the given ID.
        /// </summary>
        /// <param name="dataID">The ID of the image.</param>
        /// <param name="message">Optional message to log.</param>
        /// <returns>The request object.</returns>
        static public JSONObject makeImageDataRequest(string dataID, string message = "") {
            JSONObject dataParams = new JSONObject();
            dataParams["DataType"] = (int)EDataType.IMAGE;
            dataParams["DataID"] = dataID;

            JSONObject request = new JSONObject();
            RequestMaker.insertHeader(request, EMessageType.DATA, message);
            request["DataParams"] = dataParams;
            return request;
        }

        /// <summary>
        /// Requests the model with the given ID.
        /// </summary>
        /// <param name="dataID">The ID of the model.</param>
        /// <param name="message">Optional message to log.</param>
        /// <returns>The request object.</returns>
        static public JSONObject makeModelDataRequest(string dataID, string message = "") {
            JSONObject dataParams = new JSONObject();
            dataParams["DataType"] = (int)EDataType.MODEL;
            dataParams["DataID"] = dataID;

            JSONObject request = new JSONObject();
            RequestMaker.insertHeader(request, EMessageType.DATA, message);
            request["DataParams"] = dataParams;
            return request;
        }

        /// <summary>
        /// Requests an image slice from a Study.
        /// </summary>
        /// <param name="studyID">The ID of the study.</param>
        /// <param name="sliceIndex">The slice index.</param>
        /// <param name="orientation">The slice's orientation.</param>
        /// <param name="seriesIndex">The series in which the slice belongs to.</param>
        /// <param name="message">Optional message to log.</param>
        /// <returns>The request object.</returns>
        static public JSONObject makeStudyImageSliceDataRequest(string studyID, int sliceIndex, ESliceOrientation orientation, int seriesIndex, string message = "") {
            JSONObject dataParams = new JSONObject();
            dataParams["DataType"] = (int)EDataType.STUDY;
            dataParams["DataID"] = studyID;
            dataParams["SliceIndex"] = sliceIndex;
            dataParams["SliceOrientation"] = (int)orientation;
            dataParams["SeriesIndex"] = seriesIndex;

            JSONObject request = new JSONObject();
            RequestMaker.insertHeader(request, EMessageType.DATA, message);
            request["DataParams"] = dataParams;
            return request;
        }
    }
}