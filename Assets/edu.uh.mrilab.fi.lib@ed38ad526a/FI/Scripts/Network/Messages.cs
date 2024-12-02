using System;
using SimpleJSON;
using UnityEngine;

namespace fi {
    // General format for a Message
    public class Message
    {
        public bool HasPayload { get; set; } = false;
        public string Info { get; set; } = "";
        public byte[] Paylaod { get; set; } = new byte[0];
    }

    /**** Response Data Structures ****/

    // All responses have the following
    public class Response
    {
        // Response related information
        public SimpleJSON.JSONObject RawMessage { get; set; }
        public bool ParseError { get; set; } = false;
        public string ParseErrorMessage { get; set; } = "";

        // Response Fields
        public int ResponseStatus { get; set; } = 0;
        public EMessageType MessageType { get; set; } = EMessageType.ERROR;
        public int MessageTypeInt {
            get {
                return (int)MessageType;
            }
            set {
                if (Enum.IsDefined(typeof(EMessageType), value)) {
                    MessageType = (EMessageType)value;
                } else {
                    MessageType = EMessageType.ERROR;
                }
            }
        }
        public string Message { get; set; } = "";
        public byte[] Payload { get; set; } = new byte[0];
    }

    // Responses relating to authentication
    public class AuthenticationResponse : Response
    {
        public string ClientID { get; set; }

        public AuthenticationResponse() {
            MessageType = EMessageType.AUTHENTICATION;
        }
    }

    // Responses about the overall application 
    public class ApplicationResponse : Response
    {
        public AvailableModule[] AvailableModules { get; set; }
        public ActiveModule[] ActiveModules { get; set; }

        public ApplicationResponse() {
            MessageType = EMessageType.APPLICATION;
        }
    }

    public class AvailableModule
    {
        public string Name { get; set; }
        public string Acronym { get; set; }
    }

    public class ActiveModule
    {
        public string Name { get; set; }
        public string ID { get; set; }
    }

    // Responses about the module
    public class ModuleResponse : Response
    {
        public ModuleInformation ModuleInfo { get; set; }

        public ModuleResponse() {
            MessageType = EMessageType.MODULE;
        }
    }

    public class ModuleInformation
    {
        public string ModuleID { get; set; } = "";
        public string SceneID { get; set; } = "";
        public VisualInformation[] Visuals { get; set; }
        public ModuleInteraction[] ModuleInteractions { get; set; }
    }

    public class ModuleInteraction
    {
        public JSONObject InteractionJson { get; set; }
        public EModuleResponse ResponseID { get; set; } = EModuleResponse.UNKNOWN;
        public int ResponseIDInt {
            get {
                return (int)ResponseID;
            }
            set {
                if (Enum.IsDefined(typeof(EModuleResponse), value)) {
                    ResponseID = (EModuleResponse)value;
                } else {
                    ResponseID = EModuleResponse.UNKNOWN;
                }
            }
        }
        public string ID { get; set; } = "";
        public EModuleInteraction Type { get; set; } = EModuleInteraction.UNKNOWN;
        public int TypeInt {
            get {
                return (int)Type;
            }
            set {
                if (Enum.IsDefined(typeof(EModuleInteraction), value)) {
                    Type = (EModuleInteraction)value;
                } else {
                    Type = EModuleInteraction.UNKNOWN;
                }
            }
        }
        public EModuleInteractionConstraint Constraint { get; set; } = EModuleInteractionConstraint.UNKNOWN;
        public int ConstraintInt {
            get {
                return (int)Constraint;
            }
            set {
                if (Enum.IsDefined(typeof(EModuleInteractionConstraint), value)) {
                    Constraint = (EModuleInteractionConstraint)value;
                } else {
                    Constraint = EModuleInteractionConstraint.UNKNOWN;
                }
            }
        }
        public bool UpdateConstraint { get; set; }
        public JSONNode Value { get; set; } = 0;
        public JSONNode MinValue { get; set; } = 0;        // int or double, convert according to ActionType
        public JSONNode MaxValue { get; set; } = 0;        // int or double, convert according to ActionType
        public JSONArray ValueList { get; set; }           // strings
        public string info { get; set; } = "";
    }

    public class VisualInformation
    {
        public JSONObject VisualJson { get; set; }
        public EModuleResponse ResponseID { get; set; } = EModuleResponse.UNKNOWN;
        public int ResponseIDInt {
            get {
                return (int)ResponseID;
            }
            set {
                if (Enum.IsDefined(typeof(EModuleResponse), value)) {
                    ResponseID = (EModuleResponse)value;
                } else {
                    ResponseID = EModuleResponse.UNKNOWN;
                }
            }
        }
        public string ID { get; set; } = "";
        public EObjectType Type { get; set; } = EObjectType.UNKNOWN;
        public int TypeInt {
            get {
                return (int)Type;
            }
            set {
                if (Enum.IsDefined(typeof(EObjectType), value)) {
                    Type = (EObjectType)value;
                } else {
                    Type = EObjectType.UNKNOWN;
                }
            }
        }
        public bool Visible { get; set; } = true;
        public float Opacity { get; set; } = 0;
        public float[] Transformation { get; set; }
        public string ParentID { get; set; } = "";
        public string DataID { get; set; } = "";
        public string DataName { get; set; } = "";
        public EDataType DataType { get; set; } = EDataType.UNKNOWN;
        public int DataTypeInt {
            get {
                return (int)DataType;
            }
            set {
                if (Enum.IsDefined(typeof(EDataType), value)) {
                    DataType = (EDataType)value;
                } else {
                    DataType = EDataType.UNKNOWN;
                }
            }
        }
        public float[] Color { get; set; }
        public int SeriesIndex { get; set; } = 0;
        public ESliceOrientation SliceOrientation { get; set; } = ESliceOrientation.UNKNOWN;
        public int SliceOrientationInt {
            get {
                return (int)SliceOrientation;
            }
            set {
                if (Enum.IsDefined(typeof(ESliceOrientation), value)) {
                    SliceOrientation = (ESliceOrientation)value;
                } else {
                    SliceOrientation = ESliceOrientation.UNKNOWN;
                }
            }
        }
        public int SliceIndex { get; set; } = 0;
        public int AssemblyPartCount { get; set; } = 0;
        public VisualInformation[] AssemblyPartsInfo { get; set; }
    }

    public class DataResponse : Response
    {
        public string DataID { get; set; } = "";
        public string DataName { get; set; } = "";
        public EDataType DataType { get; set; } = EDataType.UNKNOWN;
        public int DataTypeInt {
            get {
                return (int)DataType;
            }
            set {
                if (Enum.IsDefined(typeof(EDataType), value)) {
                    DataType = (EDataType)value;
                } else {
                    DataType = EDataType.UNKNOWN;
                }
            }
        }
        public bool Cacheable { get; set; } = true;
        public int DataFormat { get; set; } = 0;  // TODO:
        public int SliceIndex { get; set; } = 0;
        public ESliceOrientation SliceOrientation { get; set; } = ESliceOrientation.UNKNOWN;
        public int SliceOrientationInt {
            get {
                return (int)SliceOrientation;
            }
            set {
                if (Enum.IsDefined(typeof(ESliceOrientation), value)) {
                    SliceOrientation = (ESliceOrientation)value;
                } else {
                    SliceOrientation = ESliceOrientation.UNKNOWN;
                }
            }
        }
        public int SeriesIndex { get; set; } = 0;
        public int SeriesCount { get; set; } = 0;
        public int[] Dimensions { get; set; }
        public float[] Origin { get; set; }
        public float[] Spacing { get; set; }
        public int PayloadPointsLength { get; set; } = 0;
        public int PayloadTrianglesLength { get; set; } = 0;
        public int PayloadTextureCoordinatesLength { get; set; } = 0;
        public TextureMetadata[] Textures { get; set; }
    }

    public class TextureMetadata
    {
        public string TextureName { get; set; } = "";
        public int DimensionU { get; set; } = 0;
        public int DimensionV { get; set; } = 0;
        public int PayloadLength { get; set; } = 0;
    }

    /**** Request Data Structures ****/
    // General request format
    [System.Serializable]
    public class Request
    {
        public string ClientID { get; set; } = ServerConnection.ConnectionID;
        public EMessageType MessageType { get; set; } = EMessageType.ERROR;
        public string Message { get; set; } = "";
    }

    // Authentication request
    [System.Serializable]
    public class AuthenticationRequest : Request
    {
        public string Password { get; set; } = "";

        public AuthenticationRequest() {
            MessageType = EMessageType.AUTHENTICATION;
        }
    }

    // Application request format
    [System.Serializable]
    public class ApplicationRequest : Request
    {
        public ApplicationParameters ApplicationParams { get; set; } = new ApplicationParameters();

        public ApplicationRequest() {
            MessageType = EMessageType.APPLICATION;
        }
    }

    [System.Serializable]
    public class ApplicationParameters
    {
        public EApplicationRequest ActionType { get; set; } = EApplicationRequest.UNKNOWN;
        public string ModuleName = "";
        public string ModuleID = "";
    }

    // Module Requests
    [System.Serializable]
    public class ModuleRequest : Request
    {
        public ModuleParameters ModuleParams { get; set; } = new ModuleParameters();

        public ModuleRequest() {
            MessageType = EMessageType.MODULE;
        }
    }

    [System.Serializable]
    public class ModuleParameters
    {
        public string ModuleID { get; set; } = "";
        public string SceneID { get; set; } = "";
        public EModuleRequest RequestID { get; set; } = EModuleRequest.UNKNOWN;
        public string TargetVisual { get; set; } = "";
        public bool Hidden { get; set; } = false;
        public double[] Translate { get; set; }
        public double[] Rotate { get; set; }
        public int SeriesIndex { get; set; } = 0;
        public ESliceOrientation SliceOrientation { get; set; } = ESliceOrientation.UNKNOWN;
        public int SliceIndex { get; set; } = 0;
        public string InteractionID { get; set; }
        SimpleJSON.JSONNode InteractionValue { get; set; }
    }

    // Data Requests
    public class DataRequest : Request
    {
        public DataParameters DataParams { get; set; } = new DataParameters();

        public DataRequest() {
            MessageType = EMessageType.DATA;
        }
    }

    public class DataParameters : Request
    {
        EDataType DataType { get; set; } = EDataType.UNKNOWN;
        string DataName { get; set; } = "";
        int SliceIndex { get; set; } = 0;
        ESliceOrientation SliceOrientation { get; set; } = ESliceOrientation.UNKNOWN;
        int SeriesIndex { get; set; } = 0;
    }
}