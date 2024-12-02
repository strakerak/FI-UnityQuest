using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimpleJSON;

namespace fi {
    public class MessageParser {
        static public Response parseResponse(JSONObject responseJSON) {
            Response response;
            int responseType = responseJSON["MessageType"].AsInt;
            switch (responseType) {
                case (int)EMessageType.AUTHENTICATION:
                    response = parseAuthenticationResponse(responseJSON);
                    break;
                case (int)EMessageType.APPLICATION:
                    response = parseApplicationResponse(responseJSON);
                    break;
                case (int)EMessageType.MODULE:
                    response = parseModuleResponse(responseJSON);
                    break;
                case (int)EMessageType.DATA:
                    response = parseDataResponse(responseJSON);
                    break;
                default:
                    response = new Response();
                    response.RawMessage = responseJSON;
                    response.MessageType = EMessageType.ERROR;
                    response.MessageTypeInt = (int)EMessageType.ERROR;
                    break;
            }

            return response;
        }

        static public AuthenticationResponse parseAuthenticationResponse(JSONObject responseJSON) {
            AuthenticationResponse response = new AuthenticationResponse();
            response.RawMessage = responseJSON;

            response.MessageTypeInt = responseJSON["MessageType"].AsInt;
            if (response.MessageType != EMessageType.AUTHENTICATION) {
                response.ParseError = true;
                response.ParseErrorMessage = "Failed to parse authentication response - response is not of authentication type";
                return response;
            }

            response.ResponseStatus = responseJSON["ResponseStatus"].AsInt;
            response.Message = responseJSON["Message"];
            response.ClientID = responseJSON["ClientID"];

            return response;
        }

        static public ApplicationResponse parseApplicationResponse(JSONObject responseJSON) {
            ApplicationResponse response = new ApplicationResponse();
            response.RawMessage = responseJSON;

            response.MessageTypeInt = responseJSON["MessageType"].AsInt;
            if (response.MessageType != EMessageType.APPLICATION) {
                response.ParseError = true;
                response.ParseErrorMessage = "Failed to parse application response - response is not of application type";
                return response;
            }

            response.ResponseStatus = responseJSON["ResponseStatus"].AsInt;
            response.Message = responseJSON["Message"];

            JSONArray availableModules = responseJSON["AvailableModules"].AsArray;
            JSONArray activeModules = responseJSON["ActiveModules"].AsArray;
            response.AvailableModules = new AvailableModule[availableModules.Count];
            response.ActiveModules = new ActiveModule[activeModules.Count];

            for (int i = 0; i < availableModules.Count; i++) {
                AvailableModule module = new AvailableModule();
                module.Name = availableModules[i].AsObject["Name"];
                module.Acronym = availableModules[i].AsObject["Acronym"];

                response.AvailableModules[i] = module;
            }

            for (int i = 0; i < activeModules.Count; i++) {
                JSONObject activeModule = activeModules[i].AsObject;
                response.ActiveModules[i] = new ActiveModule();
                response.ActiveModules[i].Name = activeModule["Name"];
                response.ActiveModules[i].ID = activeModule["ID"];
            }

            return response;
        }

        static public ModuleResponse parseModuleResponse(JSONObject responseJSON) {
            ModuleResponse response = new ModuleResponse();
            response.RawMessage = responseJSON;

            response.MessageTypeInt = responseJSON["MessageType"].AsInt;
            if (response.MessageType != EMessageType.MODULE) {
                response.ParseError = true;
                response.ParseErrorMessage = "Failed to parse module response - response is not of module type";
                Debug.LogError("Message of unknonwn type was received as Module message");
                return response;
            }

            response.ResponseStatus = responseJSON["ResponseStatus"].AsInt;
            response.Message = responseJSON["Message"];

            JSONObject modInfoJson = responseJSON["ModuleInfo"].AsObject;
            ModuleInformation modInfo = new ModuleInformation();
            response.ModuleInfo = modInfo;

            modInfo.ModuleID = modInfoJson["ModuleID"];
            modInfo.SceneID = modInfoJson["SceneID"];

            JSONArray visualsInfo = modInfoJson["VisualsInfo"].AsArray;
            modInfo.Visuals = new VisualInformation[visualsInfo.Count];
            for (int i = 0; i < visualsInfo.Count; i++) {
                modInfo.Visuals[i] = new VisualInformation();
                modInfo.Visuals[i].VisualJson = visualsInfo[i].AsObject;
            }

            JSONArray interactionsInfo = modInfoJson["ModuleInteractions"].AsArray;
            modInfo.ModuleInteractions = new ModuleInteraction[interactionsInfo.Count];
            for (int i = 0; i < interactionsInfo.Count; i++) {
                modInfo.ModuleInteractions[i] = new ModuleInteraction();
                modInfo.ModuleInteractions[i].InteractionJson = interactionsInfo[i].AsObject;
            }

            return response;
        }

        static void parseObjectInformation(JSONObject objectInformationJSON, VisualInformation objectInformation) {
            objectInformation.ID = objectInformationJSON["ID"];
            objectInformation.TypeInt = objectInformationJSON["Type"].AsInt;
            objectInformation.Visible = objectInformationJSON["Visible"].AsBool;
            objectInformation.Opacity = objectInformationJSON["Opacity"].AsFloat;

            JSONArray transformation = objectInformationJSON["Transformation"].AsArray;
            if (transformation.Count == 16) {
                objectInformation.Transformation = new float[16];
                for (int i = 0; i < 16; i++) {
                    objectInformation.Transformation[i] = transformation[i].AsFloat;
                }
            }

            objectInformation.ParentID = objectInformationJSON["ParentID"];
            objectInformation.DataID = objectInformationJSON["DataID"];
            objectInformation.DataName = objectInformationJSON["DataName"];
            objectInformation.DataTypeInt = objectInformationJSON["DataType"].AsInt;

            switch (objectInformation.Type) {
                case EObjectType.MODEL:
                    JSONArray color = objectInformationJSON["Color"].AsArray;
                    if (color.Count == 3) {
                        objectInformation.Color = new float[3];
                        objectInformation.Color[0] = color[0];
                        objectInformation.Color[1] = color[1];
                        objectInformation.Color[2] = color[2];
                    }
                    break;
                case EObjectType.STUDY_IMAGE_SLICE:
                    objectInformation.SliceIndex = objectInformationJSON["SliceIndex"].AsInt;
                    objectInformation.SliceOrientationInt = objectInformationJSON["SliceOrientation"].AsInt;
                    objectInformation.SeriesIndex = objectInformationJSON["SeriesIndex"].AsInt;
                    break;
                case EObjectType.ASSEMBLY:
                    objectInformation.AssemblyPartCount = objectInformationJSON["AssemblyPartCount"].AsInt;

                    JSONArray assemblyPartsInfo = objectInformationJSON["AssemblyPartsInfo"].AsArray;
                    objectInformation.AssemblyPartsInfo = new VisualInformation[assemblyPartsInfo.Count];
                    for (int i = 0; i < assemblyPartsInfo.Count; i++) {
                        VisualInformation partInformation = new VisualInformation();
                        MessageParser.parseObjectInformation(assemblyPartsInfo[i].AsObject, partInformation);
                        objectInformation.AssemblyPartsInfo[i] = partInformation;
                    }
                    break;
                case EObjectType.UNKNOWN:
                    Debug.LogWarning("Failed to parse object information. Object type is unknown.");
                    break;
            }
        }

        static public DataResponse parseDataResponse(JSONObject responseJSON) {
            DataResponse response = new DataResponse();
            response.RawMessage = responseJSON;

            response.MessageTypeInt = responseJSON["MessageType"].AsInt;
            if (response.MessageType != EMessageType.DATA) {
                response.ParseError = true;
                response.ParseErrorMessage = "Failed to parse data response - response is not of data type";
                return response;
            }

            response.ResponseStatus = responseJSON["ResponseStatus"].AsInt;
            response.Message = responseJSON["Message"];

            JSONObject data = responseJSON["Data"].AsObject;

            response.DataID = data["DataID"];
            response.DataName = data["DataName"];
            response.DataTypeInt = data["DataType"].AsInt;
            response.Cacheable = data["Cacheable"].AsBool;
            response.DataFormat = data["DataFormat"].AsInt;
            if (response.DataType == EDataType.IMAGE ||
                response.DataType == EDataType.STUDY) {
                response.SliceIndex = data["SliceIndex"].AsInt;
                response.SliceOrientationInt = data["SliceOrientation"].AsInt;

                JSONArray dimensions = data["Dimensions"].AsArray;
                JSONArray origin = data["Origin"].AsArray;
                JSONArray spacing = data["Spacing"].AsArray;

                if (dimensions.Count == 3) {
                    response.Dimensions = new int[3];
                    response.Dimensions[0] = dimensions[0].AsInt;
                    response.Dimensions[1] = dimensions[1].AsInt;
                    response.Dimensions[2] = dimensions[2].AsInt;
                }

                if (origin.Count == 3) {
                    response.Origin = new float[3];
                    response.Origin[0] = origin[0].AsInt;
                    response.Origin[1] = origin[1].AsInt;
                    response.Origin[2] = origin[2].AsInt;
                }

                if (spacing.Count == 3) {
                    response.Spacing = new float[3];
                    response.Spacing[0] = spacing[0].AsFloat;
                    response.Spacing[1] = spacing[1].AsFloat;
                    response.Spacing[2] = spacing[2].AsFloat;
                }

                //JSONArray values = data["Values"].AsArray;
                //response.Values = new Color[values.Count];
                //for (int i = 0; i < values.Count; i++) {
                //    float colorValue = values[i].AsFloat;
                //    Color pixel = new Color(colorValue, colorValue, colorValue);

                //    // If the orientation is XY, the x-pixels have to be mirrored across
                //    if (response.SliceOrientation == ESliceOrientation.XY) {
                //        int x = i % response.Dimensions[0];
                //        int y = i / response.Dimensions[0];
                //        x = response.Dimensions[0] - x - 1;

                //        int fixedPosition = response.Dimensions[0] * y + x;
                //        response.Values[fixedPosition] = pixel;
                //    } else {
                //        response.Values[i] = pixel;
                //    }
                //}

            } else if (response.DataType == EDataType.MODEL) {
                Debug.Log("Parsing ModelData response");
                response.PayloadPointsLength = data["PayloadPointsLength"].AsInt;
                response.PayloadTrianglesLength = data["PayloadTrianglesLength"].AsInt;
                response.PayloadTextureCoordinatesLength = data["PayloadTextureCoordinatesLength"].AsInt;

                JSONArray texturesJson = data["Textures"].AsArray;
                response.Textures = new TextureMetadata[texturesJson.Count];
                for (int i = 0; i < texturesJson.Count; i++) {
                    JSONObject textureJson = texturesJson[i].AsObject;

                    response.Textures[i] = new TextureMetadata();
                    response.Textures[i].TextureName = textureJson["TextureName"];
                    response.Textures[i].DimensionU = textureJson["DimensionU"].AsInt;
                    response.Textures[i].DimensionV = textureJson["DimensionV"].AsInt;
                    response.Textures[i].PayloadLength = textureJson["PayloadTextureLength"].AsInt;
                }

                //response.Points = new float[points.Count];
                //response.TriangleIndices = new int[triangleIndices.Count];

                //for (int i = 0; i < points.Count; i++) {
                //    response.Points[i] = points[i].AsFloat;
                //}

                //for (int i = 0; i < triangleIndices.Count; i++) {
                //    response.TriangleIndices[i] = triangleIndices[i].AsInt;
                //}
            }

            if (response.DataType == EDataType.STUDY) {
                response.SeriesIndex = data["SeriesIndex"].AsInt;
                response.SeriesCount = data["SeriesCount"].AsInt;
            }

            return response;
        }
    }
}