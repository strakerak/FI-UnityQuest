using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using SimpleJSON;
using UnityEngine.Video;

namespace fi {
    public class Scene : MonoBehaviour {
        // Scales all the objects by this number evenly in all three directions
        public float scale;

        /// <summary>
        /// The Visual prefabs.
        /// </summary>
        public ImageVisual ImageSlicePrefab;
        public ModelVisual ModelPrefab;
        public AssemblyVisual AssemblyPrefab;

        //public Manipulation DeviceManipulationPrefab;
        //private Manipulation _deviceManipulation;
        //public GameObject MovingBoxPrefab;
        //public GameObject ImageSiceSliderPrefab;

        string moduleID;
        public string ModuleID {
            get {
                return moduleID;
            }
            set {
                moduleID = value;
                //if (ModuleMenu) {
                //    ModuleMenu.name = string.Format("{0} Menu", ModuleID);
                //    ModuleMenu.ModuleID = ModuleID;
                //}
            }
        }

        //ModuleMenu ModuleMenu;

        Dictionary<string, EObjectType> VisualTypes = new Dictionary<string, EObjectType>();
        Dictionary<string, Visual3D> Visuals = new Dictionary<string, Visual3D>();
        Dictionary<string, ImageVisual> imageSlices = new Dictionary<string, ImageVisual>();
        Dictionary<string, ModelVisual> models = new Dictionary<string, ModelVisual>();
        Dictionary<string, AssemblyVisual> assemblies = new Dictionary<string, AssemblyVisual>();

        /// <summary>
        /// Used to store module type responses.
        /// </summary>
        ConcurrentQueue<ModuleResponse> ModuleResponses { get; set; } = new ConcurrentQueue<ModuleResponse>();

        // Use this for initialization
        void Start() {
            Vector3 sceneScale = new Vector3(
                transform.localScale.x * scale,
                transform.localScale.y * scale,
                transform.localScale.z * scale
                );
            transform.localScale = sceneScale;

            //_deviceManipulation = DeviceManipulationPrefab.GetComponent<Manipulation>();
        }

        /// <summary>
        /// When a new module response is received, enqueue it to be executed.
        /// </summary>
        /// <param name="moduleResponse">The module response.</param>
        public void onNewModuleMessage(ModuleResponse moduleResponse) {
            if (!moduleResponse.ModuleInfo.ModuleID.Equals(ModuleID)) {
                return;
            }
            ModuleResponses.Enqueue(moduleResponse);
        }

        /// <summary>
        /// Goes through the given visual updates and applies them.
        /// </summary>
        /// <param name="visualUpdates">The visual updates.</param>
        public void executeVisualUpdates(VisualInformation[] visualUpdates) {
            List<VisualInformation> parentChanges = new List<VisualInformation>();
            for (int i = 0; i < visualUpdates.Length; i++) {
                VisualInformation visual = visualUpdates[i];

                visual.ResponseIDInt = visual.VisualJson["ResponseID"].AsInt;
                switch (visual.ResponseID) {
                    case EModuleResponse.ADD_VISUAL:
                        this.executeAddVisual(visual);
                        parentChanges.Add(visual);
                        break;
                    case EModuleResponse.REFRESH_VISUAL:
                        this.executeRefreshVisual(visual);
                        parentChanges.Add(visual);
                        break;
                    case EModuleResponse.REMOVE_VISUAL:
                        this.executeRemoveVisual(visual);
                        break;
                    case EModuleResponse.DATA_CHANGE:
                        this.executeDataChange(visual);
                        break;
                    case EModuleResponse.HIDE_VISUAL:
                        this.executeHideVisual(visual);
                        break;
                    case EModuleResponse.TRANSFORM_VISUAL:
                        this.executeTransformVisual(visual);
                        break;
                    case EModuleResponse.PARENT_CHANGE:
                        visual.ID = visual.VisualJson["ID"];
                        visual.ParentID = visual.VisualJson["ParentID"];
                        parentChanges.Add(visual);
                        break;
                    case EModuleResponse.SET_VISUAL_OPACITY:
                        this.executeSetVisualOpacity(visual);
                        break;
                    case EModuleResponse.SET_SLICE:
                        this.executeSetSlice(visual);
                        break;
                    case EModuleResponse.SET_COLOR:
                        this.executeSetColor(visual);
                        break;
                    default:
                        Debug.LogWarning("Received an unknown object action.");
                        break;
                }
            }

            // Ensure parenting was set as order of Visual updates may not come in order.
            foreach (VisualInformation visual in parentChanges) {
                if (Visuals.ContainsKey(visual.ID) && Visuals.ContainsKey(visual.ParentID)) {
                    if (Visuals[visual.ID].ParentVisual != Visuals[visual.ParentID]) {
                        Visuals[visual.ID].ParentVisual = Visuals[visual.ParentID];
                        Visuals[visual.ID].setTransform(visual.Transformation);
                    }
                }
            }
        }
        public void updateBoxCollider() {
            var spaceCollider = this.gameObject.GetComponent<BoxCollider>();

            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            var bounds = new Bounds(this.transform.position, spaceCollider.size);
            foreach (var meshFilter in meshFilters) {
                bounds.Encapsulate(meshFilter.mesh.bounds);
            }

            var c = spaceCollider.center;
            spaceCollider.size = bounds.size;
        }

        void executeAddVisual(VisualInformation visualInfo) {
            visualInfo.ID = visualInfo.VisualJson["ID"];

            if (Visuals.ContainsKey(visualInfo.ID)) {
                Debug.LogWarning(string.Format("Cannot add visual=[{0}] to scene because its ID is already taken", visualInfo.ID));
                return;
            }

            visualInfo.TypeInt = visualInfo.VisualJson["Type"].AsInt;
            Visual3D visual;

            switch (visualInfo.Type) {
                case EObjectType.STUDY_IMAGE_SLICE:
                    ImageVisual newImageSlice = Instantiate(ImageSlicePrefab, transform);
                    imageSlices.Add(visualInfo.ID, newImageSlice);

                    visualInfo.DataID = visualInfo.VisualJson["DataID"];
                    visualInfo.SliceIndex = visualInfo.VisualJson["SliceIndex"].AsInt;
                    visualInfo.SliceOrientationInt = visualInfo.VisualJson["SliceOrientation"].AsInt;
                    visualInfo.SeriesIndex = visualInfo.VisualJson["SeriesIndex"].AsInt;
                    this.setStudyImageSliceDataAsync(visualInfo.DataID, visualInfo.SliceIndex, visualInfo.SliceOrientation, visualInfo.SeriesIndex, newImageSlice);

                    visual = newImageSlice;
                    break;
                case EObjectType.MODEL:
                    ModelVisual newModel = Instantiate(ModelPrefab, transform);
                    models.Add(visualInfo.ID, newModel);

                    visualInfo.Color = new float[3];
                    JSONArray colorJson = visualInfo.VisualJson["Color"].AsArray;
                    if (colorJson.Count == 3) {
                        visualInfo.Color[0] = colorJson[0].AsFloat;
                        visualInfo.Color[1] = colorJson[1].AsFloat;
                        visualInfo.Color[2] = colorJson[2].AsFloat;
                    }
                    visualInfo.Opacity = visualInfo.VisualJson["Opacity"].AsFloat;
                    // TODO: Only Models' opacity can be changed, generalize to other visuals.
                    newModel.setColor(visualInfo.Color, visualInfo.Opacity);

                    visualInfo.DataID = visualInfo.VisualJson["DataID"];
                    this.setModelDataAsync(visualInfo.DataID, newModel);

                    visual = newModel;
                    break;
                case EObjectType.ASSEMBLY:
                    AssemblyVisual newAssembly = Instantiate(AssemblyPrefab, transform);
                    assemblies.Add(visualInfo.ID, newAssembly);

                    JSONArray partsInfoJson = visualInfo.VisualJson["AssemblyPartsInfo"].AsArray;
                    VisualInformation[] partsInfo = new VisualInformation[partsInfoJson.Count];
                    for (int i = 0; i < partsInfo.Length; i++) {
                        partsInfo[i] = new VisualInformation();
                        partsInfo[i].VisualJson = partsInfoJson[i].AsObject;
                        partsInfo[i].ResponseIDInt = visualInfo.VisualJson["AssemblyAction"].AsInt;
                    }
                    newAssembly.executePartUpdates(partsInfo);

                    visual = newAssembly;
                    break;
                default:
                    Debug.LogWarning(string.Format("Failed to add visual=[{0}] because its type is unknown", visualInfo.ID));
                    return;
            }
            visual.name = visualInfo.ID;
            visual.Visible = visualInfo.Visible;
            // TODO: Gotta set opacity of Visual

            visualInfo.ParentID = visualInfo.VisualJson["ParentID"];
            if (Visuals.ContainsKey(visualInfo.ParentID)) {
                visual.ParentVisual = Visuals[visualInfo.ParentID];
            }

            JSONArray transformation = visualInfo.VisualJson["Transformation"].AsArray;
            if (transformation.Count == 16) {
                visualInfo.Transformation = new float[16];
                for (int i = 0; i < 16; i++) {
                    visualInfo.Transformation[i] = transformation[i].AsFloat;
                }
                visual.setTransform(visualInfo.Transformation);
            }

            VisualTypes.Add(visualInfo.ID, visual.ObjectType);
            Visuals.Add(visualInfo.ID, visual);
        }

        void executeRefreshVisual(VisualInformation visualInfo) {
            visualInfo.ID = visualInfo.VisualJson["ID"];

            if (!Visuals.ContainsKey(visualInfo.ID)) {
                Debug.LogWarning(string.Format("Cannot refresh visual=[{0}] in scene because visual was not found.", visualInfo.ID));
                return;
            }

            visualInfo.TypeInt = visualInfo.VisualJson["Type"].AsInt;
            Visual3D visual;

            switch (visualInfo.Type) {
                case EObjectType.STUDY_IMAGE_SLICE:
                    ImageVisual slice = imageSlices[visualInfo.ID];

                    visualInfo.DataID = visualInfo.VisualJson["DataID"];
                    visualInfo.SliceIndex = visualInfo.VisualJson["SliceIndex"].AsInt;
                    visualInfo.SliceOrientationInt = visualInfo.VisualJson["SliceOrientation"].AsInt;
                    visualInfo.SeriesIndex = visualInfo.VisualJson["SeriesIndex"].AsInt;
                    this.setStudyImageSliceDataAsync(visualInfo.DataID, visualInfo.SliceIndex, visualInfo.SliceOrientation, visualInfo.SeriesIndex, slice);

                    visual = slice;
                    break;
                case EObjectType.MODEL:
                    ModelVisual model = models[visualInfo.ID];

                    visualInfo.Color = new float[3];
                    JSONArray colorJson = visualInfo.VisualJson["Color"].AsArray;
                    if (colorJson.Count == 3) {
                        visualInfo.Color[0] = colorJson[0].AsFloat;
                        visualInfo.Color[1] = colorJson[1].AsFloat;
                        visualInfo.Color[2] = colorJson[2].AsFloat;
                    }
                    visualInfo.Opacity = visualInfo.VisualJson["Opacity"].AsFloat;
                    // TODO: Only Models' opacity can be changed, generalize to other visuals.
                    model.setColor(visualInfo.Color, visualInfo.Opacity);

                    visualInfo.DataID = visualInfo.VisualJson["DataID"];
                    this.setModelDataAsync(visualInfo.DataID, model);

                    visual = model;
                    break;
                case EObjectType.ASSEMBLY:
                    AssemblyVisual assembly = assemblies[visualInfo.ID];
                    visual = assembly;
                    break;
                default:
                    Debug.LogWarning(string.Format("Failed to refresh visual=[{0}] because its type is unknown.", visualInfo.ID));
                    return;
            }
            visual.Visible = visualInfo.Visible;
            // TODO: Gotta set opacity of Visual

            visualInfo.ParentID = visualInfo.VisualJson["ParentID"];
            if (Visuals.ContainsKey(visualInfo.ParentID)) {
                visual.ParentVisual = Visuals[visualInfo.ParentID];
            }

            JSONArray transformation = visualInfo.VisualJson["Transformation"].AsArray;
            if (transformation.Count == 16) {
                visualInfo.Transformation = new float[16];
                for (int i = 0; i < 16; i++) {
                    visualInfo.Transformation[i] = transformation[i].AsFloat;
                }
                visual.setTransform(visualInfo.Transformation);
            }
        }

        void executeRemoveVisual(VisualInformation visualInfo) {
            visualInfo.ID = visualInfo.VisualJson["ID"];

            if (!Visuals.ContainsKey(visualInfo.ID)) {
                Debug.LogWarning(string.Format("Cannot remove visual=[{0}] from scene because visual was not found.", visualInfo.ID));
                return;
            }

            visualInfo.TypeInt = visualInfo.VisualJson["Type"].AsInt;

            switch (visualInfo.Type) {
                case EObjectType.STUDY_IMAGE_SLICE:
                    imageSlices.Remove(visualInfo.ID);
                    break;
                case EObjectType.MODEL:
                    models.Remove(visualInfo.ID);
                    break;
                case EObjectType.ASSEMBLY:
                    assemblies.Remove(visualInfo.ID);
                    break;
            }
            Visual3D obj = Visuals[visualInfo.ID];
            Visuals.Remove(visualInfo.ID);
            VisualTypes.Remove(visualInfo.ID);
            Destroy(obj);
        }

        void executeDataChange(VisualInformation visualInfo) {
            visualInfo.ID = visualInfo.VisualJson["ID"];

            if (!Visuals.ContainsKey(visualInfo.ID)) {
                Debug.LogWarning(string.Format("Cannot change data of visual=[{0}] because visual was not found.", visualInfo.ID));
                return;
            }

            visualInfo.TypeInt = visualInfo.VisualJson["Type"].AsInt;
            switch (visualInfo.Type) {
                case EObjectType.STUDY_IMAGE_SLICE:
                    ImageVisual slice = imageSlices[visualInfo.ID];
                    visualInfo.DataID = visualInfo.VisualJson["DataID"];
                    visualInfo.SliceIndex = visualInfo.VisualJson["SliceIndex"].AsInt;
                    visualInfo.SliceOrientationInt = visualInfo.VisualJson["SliceOrientation"].AsInt;
                    visualInfo.SeriesIndex = visualInfo.VisualJson["SeriesIndex"].AsInt;
                    this.setStudyImageSliceDataAsync(visualInfo.DataID, visualInfo.SliceIndex, visualInfo.SliceOrientation, visualInfo.SeriesIndex, slice);
                    break;
                case EObjectType.MODEL:
                    ModelVisual model = models[visualInfo.ID];
                    visualInfo.DataID = visualInfo.VisualJson["DataID"];
                    this.setModelDataAsync(visualInfo.DataID, model);
                    break;
                case EObjectType.ASSEMBLY:
                    AssemblyVisual assembly = assemblies[visualInfo.ID];
                    JSONArray partsInfoJson = visualInfo.VisualJson["AssemblyPartsInfo"].AsArray;
                    VisualInformation[] partsInfo = new VisualInformation[partsInfoJson.Count];
                    for (int i = 0; i < partsInfo.Length; i++) {
                        partsInfo[i] = new VisualInformation();
                        partsInfo[i].VisualJson = partsInfoJson[i].AsObject;
                        partsInfo[i].ResponseIDInt = visualInfo.VisualJson["AssemblyAction"].AsInt;
                    }
                    assembly.executePartUpdates(partsInfo);
                    break;
                default:
                    Debug.LogWarning(string.Format("Failed to change data of visual=[{0}] because its type is unknown.", visualInfo.ID));
                    break;
            }
        }

        void executeHideVisual(VisualInformation visualInfo) {
            visualInfo.ID = visualInfo.VisualJson["ID"];

            if (!Visuals.ContainsKey(visualInfo.ID)) {
                Debug.LogWarning(string.Format("Cannot change visibility of visual=[{0}] because visual was not found.", visualInfo.ID));
                return;
            }

            Visual3D visual = Visuals[visualInfo.ID];
            visualInfo.Visible = visualInfo.VisualJson["Visible"].AsBool;
            visual.Visible = visualInfo.Visible;
        }

        void executeTransformVisual(VisualInformation visualInfo) {
            visualInfo.ID = visualInfo.VisualJson["ID"];

            if (!Visuals.ContainsKey(visualInfo.ID)) {
                Debug.LogWarning(string.Format("Cannot transform visual=[{0}] because visual was not found.", visualInfo.ID));
                return;
            }
            Visual3D visual = Visuals[visualInfo.ID];

            JSONArray transformation = visualInfo.VisualJson["Transformation"].AsArray;
            if (transformation.Count == 16) {
                visualInfo.Transformation = new float[16];
                for (int i = 0; i < 16; i++) {
                    visualInfo.Transformation[i] = transformation[i].AsFloat;
                }
                visual.setTransform(visualInfo.Transformation);
            }
        }

        void executeSetVisualOpacity(VisualInformation visualInfo) {
            // TODO: At the moment, only models are applied opacity
            visualInfo.ID = visualInfo.VisualJson["ID"];
            if (!models.ContainsKey(visualInfo.ID)) {
                Debug.LogWarning(string.Format("Cannot change opacity of visual=[{0}] because visual was not found. Only Models are compatible with opacity at the moment.", visualInfo.ID));
                return;
            }

            ModelVisual model = models[visualInfo.ID];
            visualInfo.Opacity = visualInfo.VisualJson["Opacity"].AsFloat;
            model.setOpacity(visualInfo.Opacity);
        }

        void executeSetSlice(VisualInformation visualInfo) {
            visualInfo.ID = visualInfo.VisualJson["ID"];
            if (!imageSlices.ContainsKey(visualInfo.ID)) {
                Debug.LogWarning(string.Format("Cannot change slice of Slice=[{0}] because slice was not found.", visualInfo.ID));
                return;
            }

            ImageVisual slice = imageSlices[visualInfo.ID];
            //_deviceManipulation.UpdateSlice(visualInfo.ID, slice);

            visualInfo.DataID = visualInfo.VisualJson["DataID"];
            visualInfo.SliceIndex = visualInfo.VisualJson["SliceIndex"].AsInt;
            visualInfo.SliceOrientationInt = visualInfo.VisualJson["SliceOrientation"].AsInt;
            visualInfo.SeriesIndex = visualInfo.VisualJson["SeriesIndex"].AsInt;
            this.setStudyImageSliceDataAsync(visualInfo.DataID, visualInfo.SliceIndex, visualInfo.SliceOrientation, visualInfo.SeriesIndex, slice);
        }

        void executeSetColor(VisualInformation visualInfo) {
            visualInfo.ID = visualInfo.VisualJson["ID"];
            if (!models.ContainsKey(visualInfo.ID)) {
                Debug.LogWarning(string.Format("Cannot change color of Model=[{0}] because model was not found.", visualInfo.ID));
                return;
            }
            ModelVisual model = models[visualInfo.ID];

            JSONArray color = visualInfo.VisualJson["Color"].AsArray;
            if (color.Count == 3) {
                model.setColor(color[0].AsFloat, color[1].AsFloat, color[2].AsFloat);
            }
        }

        async void setStudyImageSliceDataAsync(string studyName, int sliceIndex, ESliceOrientation sliceOrientation, int seriesIndex, ImageVisual imageSlice) {
            Debug.Log(string.Format("Requesting data={0} for object={1}", studyName, imageSlice.name));
            TaskCompletionSource<SliceData> promise = DataManager.getStudySliceData(studyName, sliceIndex, sliceOrientation, seriesIndex);
            await promise.Task;
            SliceData sliceData = promise.Task.Result;
            imageSlice.setSliceData(sliceData, sliceIndex, sliceOrientation, seriesIndex);
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
            //GameObject newObject = Instantiate(MovingBoxPrefab, model.transform.parent);
            //newObject.GetComponent<HLMovingBox>().AddObject(model);
        }
    }
}