using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using SimpleJSON;

namespace fi {
    public class AssemblyVisual : Visual3D {

        public ModelVisual ModelPrefab;
        public GameObject MovingBoxPrefab;

        public string DataName { get; private set; }
        Dictionary<string, ModelVisual> Parts = new Dictionary<string, ModelVisual>();

        private void Awake() {
            this.ObjectType = EObjectType.ASSEMBLY;
        }

        public void executePartUpdates(VisualInformation[] partsInfo) {
            List<VisualInformation> parentChanges = new List<VisualInformation>();
            foreach (VisualInformation partInfo in partsInfo) {
                switch (partInfo.ResponseID) {
                    case EModuleResponse.ADD_VISUAL:
                        this.executeAddPart(partInfo);
                        parentChanges.Add(partInfo);
                        break;
                    case EModuleResponse.REFRESH_VISUAL:
                        this.executeRefreshPart(partInfo);
                        parentChanges.Add(partInfo);
                        break;
                    case EModuleResponse.REMOVE_VISUAL:
                        this.executeRemovePart(partInfo);
                        break;
                    case EModuleResponse.DATA_CHANGE:
                        this.executeDataChange(partInfo);
                        break;
                    case EModuleResponse.HIDE_VISUAL:
                        this.executeHidePart(partInfo);
                        break;
                    case EModuleResponse.TRANSFORM_VISUAL:
                        this.executeTransformPart(partInfo);
                        break;
                    case EModuleResponse.PARENT_CHANGE:
                        partInfo.ID = partInfo.VisualJson["ID"];
                        partInfo.ParentID = partInfo.VisualJson["ParentID"];
                        parentChanges.Add(partInfo);
                        break;
                    case EModuleResponse.SET_VISUAL_OPACITY:
                        this.executeSetPartOpacity(partInfo);
                        break;
                    case EModuleResponse.SET_COLOR:
                        this.executeSetColor(partInfo);
                        break;
                    default:
                        Debug.LogWarning("Received an unknown object action.");
                        break;
                }
            }

            // Ensure parenting was set as order of Visual updates may not come in order.
            foreach (VisualInformation partInfo in parentChanges) {
                if (Parts.ContainsKey(partInfo.ID) && Parts.ContainsKey(partInfo.ParentID)) {
                    if (Parts[partInfo.ID].ParentVisual != Parts[partInfo.ParentID]) {
                        Parts[partInfo.ID].ParentVisual = Parts[partInfo.ParentID];
                        Parts[partInfo.ID].setTransform(partInfo.Transformation);
                    }
                }
            }
        }

        Visual3D executeAddPart(VisualInformation partInfo) {
            partInfo.ID = partInfo.VisualJson["ID"];
            if (Parts.ContainsKey(partInfo.ID)) {
                Debug.LogWarning(string.Format("Failed to add part=[{0}] because another part with that ID already exists.",
                    partInfo.ID));
                return null;
            }

            partInfo.TypeInt = partInfo.VisualJson["Type"].AsInt;
            if (partInfo.Type != EObjectType.MODEL) {
                Debug.LogWarning(string.Format("Failed to add part=[{0}] because the part is not a model", partInfo.ID));
                return null;
            }

            ModelVisual part = Instantiate(ModelPrefab, transform);
            part.name = partInfo.ID;
            Parts.Add(partInfo.ID, part);

            partInfo.DataID = partInfo.VisualJson["DataID"];
            this.setModelDataAsync(partInfo.DataID, part);
            this.setModelDataBoxAsync(partInfo.DataID, part);

            partInfo.Visible = partInfo.VisualJson["Visible"].AsBool;
            part.Visible = partInfo.Visible;

            partInfo.Color = new float[3];
            JSONArray colorJson = partInfo.VisualJson["Color"].AsArray;
            if (colorJson.Count == 3) {
                partInfo.Color[0] = colorJson[0].AsFloat;
                partInfo.Color[1] = colorJson[1].AsFloat;
                partInfo.Color[2] = colorJson[2].AsFloat;
            }
            partInfo.Opacity = partInfo.VisualJson["Opacity"].AsFloat;
            part.setColor(partInfo.Color, partInfo.Opacity);

            partInfo.ParentID = partInfo.VisualJson["ParentID"];
            if (Parts.ContainsKey(partInfo.ParentID)) {
                part.ParentVisual = Parts[partInfo.ParentID];
            }

            JSONArray transformation = partInfo.VisualJson["Transformation"].AsArray;
            if (transformation.Count == 16) {
                partInfo.Transformation = new float[16];
                for (int i = 0; i < 16; i++) {
                    partInfo.Transformation[i] = transformation[i].AsFloat;
                }
                part.setTransform(partInfo.Transformation);
            }

            return part;
        }

        void executeRefreshPart(VisualInformation partInfo) {
            partInfo.ID = partInfo.VisualJson["ID"];
            if (!Parts.ContainsKey(partInfo.ID)) {
                Debug.LogWarning(string.Format("Failed to refresh part=[{0}] because part was not found.", partInfo.ID));
                return;
            }

            ModelVisual part = Parts[partInfo.ID];

            partInfo.DataID = partInfo.VisualJson["DataID"];
            this.setModelDataAsync(partInfo.DataID, part);
            this.setModelDataBoxAsync(partInfo.DataID, part);

            partInfo.Visible = partInfo.VisualJson["Visible"].AsBool;
            part.Visible = partInfo.Visible;

            partInfo.Color = new float[3];
            JSONArray colorJson = partInfo.VisualJson["Color"].AsArray;
            if (colorJson.Count == 3) {
                partInfo.Color[0] = colorJson[0].AsFloat;
                partInfo.Color[1] = colorJson[1].AsFloat;
                partInfo.Color[2] = colorJson[2].AsFloat;
            }
            partInfo.Opacity = partInfo.VisualJson["Opacity"].AsFloat;
            part.setColor(partInfo.Color, partInfo.Opacity);

            partInfo.ParentID = partInfo.VisualJson["ParentID"];
            if (Parts.ContainsKey(partInfo.ParentID)) {
                part.ParentVisual = Parts[partInfo.ParentID];
            }

            JSONArray transformation = partInfo.VisualJson["Transformation"].AsArray;
            if (transformation.Count == 16) {
                partInfo.Transformation = new float[16];
                for (int i = 0; i < 16; i++) {
                    partInfo.Transformation[i] = transformation[i].AsFloat;
                }
                part.setTransform(partInfo.Transformation);
            }
        }

        void executeRemovePart(VisualInformation partInfo) {
            partInfo.ID = partInfo.VisualJson["ID"];
            if (!Parts.ContainsKey(partInfo.ID)) {
                Debug.LogWarning(string.Format("Failed to remove part=[{0}] because part was not found.", partInfo.ID));
                return;
            }

            ModelVisual part = Parts[partInfo.ID];
            Parts.Remove(partInfo.ID);

            part.transform.parent = null;
            Destroy(part);
        }

        void executeDataChange(VisualInformation partInfo) {
            partInfo.ID = partInfo.VisualJson["ID"];
            if (!Parts.ContainsKey(partInfo.ID)) {
                Debug.LogWarning(string.Format("Failed to remove part=[{0}] because part was not found.", partInfo.ID));
                return;
            }

            ModelVisual part = Parts[partInfo.ID];

            partInfo.DataID = partInfo.VisualJson["DataID"];
            this.setModelDataAsync(partInfo.DataID, part);
            this.setModelDataBoxAsync(partInfo.DataID, part);
        }

        void executeHidePart(VisualInformation partInfo) {
            partInfo.ID = partInfo.VisualJson["ID"];
            if (!Parts.ContainsKey(partInfo.ID)) {
                Debug.LogWarning(string.Format("Failed to set visibility of part=[{0}] because part was not found.", partInfo.ID));
                return;
            }

            ModelVisual part = Parts[partInfo.ID];

            partInfo.Visible = partInfo.VisualJson["Visible"].AsBool;
            part.Visible = partInfo.Visible;
        }

        void executeTransformPart(VisualInformation partInfo) {
            partInfo.ID = partInfo.VisualJson["ID"];
            if (!Parts.ContainsKey(partInfo.ID)) {
                Debug.LogWarning(string.Format("Failed to set transform of part=[{0}] because part was not found.", partInfo.ID));
                return;
            }

            ModelVisual part = Parts[partInfo.ID];

            JSONArray transformation = partInfo.VisualJson["Transformation"].AsArray;
            if (transformation.Count == 16) {
                partInfo.Transformation = new float[16];
                for (int i = 0; i < 16; i++) {
                    partInfo.Transformation[i] = transformation[i].AsFloat;
                }
                part.setTransform(partInfo.Transformation);
            }
        }

        void executeSetPartOpacity(VisualInformation partInfo) {
            partInfo.ID = partInfo.VisualJson["ID"];
            if (!Parts.ContainsKey(partInfo.ID)) {
                Debug.LogWarning(string.Format("Failed to set opacity of part=[{0}] because part was not found.", partInfo.ID));
                return;
            }

            ModelVisual part = Parts[partInfo.ID];

            partInfo.Opacity = partInfo.VisualJson["Opacity"].AsFloat;
            part.setOpacity(partInfo.Opacity);
        }

        void executeSetColor(VisualInformation partInfo) {
            partInfo.ID = partInfo.VisualJson["ID"];
            if (!Parts.ContainsKey(partInfo.ID)) {
                Debug.LogWarning(string.Format("Failed to set opacity of part=[{0}] because part was not found.", partInfo.ID));
                return;
            }

            ModelVisual part = Parts[partInfo.ID];

            partInfo.Color = new float[3];
            JSONArray colorJson = partInfo.VisualJson["Color"].AsArray;
            if (colorJson.Count == 3) {
                partInfo.Color[0] = colorJson[0].AsFloat;
                partInfo.Color[1] = colorJson[1].AsFloat;
                partInfo.Color[2] = colorJson[2].AsFloat;
            }
            part.setColor(partInfo.Color);
        }

        public ModelVisual getPart(string partName) {
            if (Parts.ContainsKey(partName)) {
                return Parts[partName];
            }
            return null;
        }

        async void setModelDataAsync(string modelDataName, ModelVisual modelSceneObject) {
            Debug.Log(string.Format("Requesting data={0} for object={1}", modelDataName, modelSceneObject.name));
            TaskCompletionSource<ModelData> promise = DataManager.getModelData(modelDataName);
            await promise.Task;
            ModelData modelData = promise.Task.Result;
            modelSceneObject.ModelData = modelData;
        }

        public async void setModelDataBoxAsync(string modelDataName, ModelVisual modelSceneObject) {
            while (modelSceneObject.ModelData == null) {
                await Task.Yield();
            }

            GameObject model = GameObject.Find(modelDataName);
            GameObject newObject = Instantiate(MovingBoxPrefab, model.transform.parent);
            //newObject.GetComponent<HLMovingBox>().AddObject(model);
        }

    }
}