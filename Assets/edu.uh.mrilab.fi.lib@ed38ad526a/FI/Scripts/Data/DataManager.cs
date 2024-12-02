using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace fi {
    // TODO: Do ImageDataPendingRequest
    class SlicePendingRequest {
        public string dataName;
        public List<TaskCompletionSource<SliceData>> promises = new List<TaskCompletionSource<SliceData>>();
    }

    class StudySlicePendingRequestID {
        public string studyName;
        public int sliceIndex;
        public int sliceOrientation;
        public int seriesIndex;

        public float modelX = 69f;
        public float modelY = 69f;
        public float modelZ = 69f;

        public override bool Equals(object that) {
            StudySlicePendingRequestID other = that as StudySlicePendingRequestID;
            if (this == that) {
                return true;
            } else if (other != null) {
                return studyName.Equals(other.studyName) && sliceIndex == other.sliceIndex &&
                    sliceOrientation == other.sliceOrientation && seriesIndex == other.seriesIndex;
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            return string.Format("{0}|{1}|{2}|{3}", studyName, sliceIndex,
                sliceOrientation, seriesIndex).GetHashCode();
        }
    }

    class ModelPendingRequest {
        public string dataName;
        public List<TaskCompletionSource<ModelData>> promises = new List<TaskCompletionSource<ModelData>>();
    }

    class DataManager : MonoBehaviour {
        /// <summary>
        /// This object can only exist once. This is the singleton of it.
        /// </summary>
        static DataManager INSTANCE;

        /// <summary>
        /// List of requests that need study data.
        /// </summary>
        Dictionary<StudySlicePendingRequestID, SlicePendingRequest> StudySlicePendingRequests { get; set; } = new Dictionary<StudySlicePendingRequestID, SlicePendingRequest>();

        /// <summary>
        /// List of requests that need model data.
        /// </summary>
        Dictionary<string, ModelPendingRequest> ModelPendingRequests { get; set; } = new Dictionary<string, ModelPendingRequest>();

        /// <summary>
        /// Used to store data type responses.
        /// </summary>
        ConcurrentQueue<DataResponse> DataResponses { get; set; } = new ConcurrentQueue<DataResponse>();

        /// <summary>
        /// Hash table where known studies are stored.
        /// </summary>
        Dictionary<string, StudyData> studies = new Dictionary<string, StudyData>();

        /// <summary>
        /// Hash table where known models are stored.
        /// </summary>
        Dictionary<string, ModelData> models = new Dictionary<string, ModelData>();

        /// <summary>
        /// Sets the singleton of this class.
        /// </summary>
        private void Awake() {
            if (INSTANCE != null) {
                Destroy(gameObject);
            } else {
                INSTANCE = this;
                GameObject.DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Handles any data responses that have been received.
        /// </summary>
        private void Update() {
            if (!DataResponses.IsEmpty) {
                DataResponse response;
                DataResponses.TryDequeue(out response);
                if (response != null) {
                    this.handleDataResponse(response);
                }
            }
        }

        /// <summary>
        /// When a new data response is received, enqueue it to be executed.
        /// </summary>
        /// <param name="applicationResponse">The data response.</param>
        public void onNewDataResponse(DataResponse response) {
            DataResponses.Enqueue(response);
        }

        /// <summary>
        /// Handles that given data response.
        /// </summary>
        /// <param name="data">The data response.</param>
        private void handleDataResponse(DataResponse data) {
            if (data == null) {
                return;
            }
            Debug.Log(string.Format("Received data for data={0}", data.DataID));

            //TODO: Do Images later
            if (data.DataType == EDataType.STUDY) {
                StudyData study;
                if (studies.ContainsKey(data.DataID)) {
                    study = studies[data.DataID];
                } else {
                    Debug.Log(string.Format("Creating new study with SeriesCount={0}", data.SeriesCount));
                    study = new StudyData(data.DataID, data.SeriesCount, data.Dimensions, data.Spacing);
                    if (data.Cacheable) {
                        studies.Add(study.StudyName, study);
                    }
                }

                study.setSliceData(data.SliceIndex, data.SliceOrientation, data.SeriesIndex, data.Payload);

                StudySlicePendingRequestID studySlicePendingRequestID = new StudySlicePendingRequestID();
                studySlicePendingRequestID.studyName = study.StudyName;
                studySlicePendingRequestID.sliceIndex = data.SliceIndex;
                studySlicePendingRequestID.sliceOrientation = data.SliceOrientationInt;
                studySlicePendingRequestID.seriesIndex = data.SeriesIndex;
                studySlicePendingRequestID.modelX = 65f;
                studySlicePendingRequestID.modelY = 65f;
                studySlicePendingRequestID.modelZ = 65f;

                Debug.Log(string.Format("Checking for StudySliceDataRequest promises with ID: Name={0}, Index={1}, Orientation={2}, Series={3}, X={4}, y={5}, z={6}", studySlicePendingRequestID.studyName,
                studySlicePendingRequestID.sliceIndex, studySlicePendingRequestID.sliceOrientation, studySlicePendingRequestID.seriesIndex, studySlicePendingRequestID.modelX, studySlicePendingRequestID.modelY, studySlicePendingRequestID.modelZ));


                if (StudySlicePendingRequests.ContainsKey(studySlicePendingRequestID)) {
                    SlicePendingRequest sliceRequest = StudySlicePendingRequests[studySlicePendingRequestID];
                    StudySlicePendingRequests.Remove(studySlicePendingRequestID);
                    Debug.Log(string.Format("Completing {0} promises for slice data", sliceRequest.promises.Count));
                    foreach (TaskCompletionSource<SliceData> promise in sliceRequest.promises) {
                        promise.SetResult(study.getSliceData(data.SliceIndex, data.SliceOrientation, data.SeriesIndex));
                    }
                } else {
                    Debug.Log("No ID found!");
                }

            } else if (data.DataType == EDataType.MODEL) {
                ModelData modelData;
                if (models.ContainsKey(data.DataID)) {
                    modelData = models[data.DataID];
                    modelData.setMeshData(data);
                } else {
                    modelData = new ModelData(data);
                    if (data.Cacheable) {
                        models.Add(modelData.DataName, modelData);
                    }
                }

                if (ModelPendingRequests.ContainsKey(data.DataID)) {
                    ModelPendingRequest request = ModelPendingRequests[data.DataID];
                    ModelPendingRequests.Remove(data.DataID);
                    Debug.Log(string.Format("Completing {0} promises for model data.", request.promises.Count));
                    foreach (TaskCompletionSource<ModelData> promise in request.promises) {
                        promise.SetResult(modelData);
                    }
                } else {
                    Debug.Log("There was no promises for this data.");
                }
            }
        }

        static public TaskCompletionSource<ModelData> getModelData(string modelDataName) {
            TaskCompletionSource<ModelData> modelDataPromise = new TaskCompletionSource<ModelData>();
            if (INSTANCE.models.ContainsKey(modelDataName)) {
                modelDataPromise.SetResult(INSTANCE.models[modelDataName]);
            } else {
                if (INSTANCE.ModelPendingRequests.ContainsKey(modelDataName)) {
                    INSTANCE.ModelPendingRequests[modelDataName].promises.Add(modelDataPromise);
                } else {
                    ModelPendingRequest newModelPendingRequest = new ModelPendingRequest();
                    newModelPendingRequest.dataName = modelDataName;
                    newModelPendingRequest.promises.Add(modelDataPromise);

                    INSTANCE.ModelPendingRequests.Add(modelDataName, newModelPendingRequest);

                    SimpleJSON.JSONObject modelDataRequest = RequestMaker.makeModelDataRequest(modelDataName);
                    ServerConnection.sendMessage(modelDataRequest);
                }
            }
            return modelDataPromise;
        }

        /// <summary>
        /// Gets a SliceData that matches the given parameters. Null if indices are out of range or data does not exist.
        /// </summary>
        /// <param name="studyName"></param>
        /// <param name="sliceIndex"></param>
        /// <param name="sliceOrientation"></param>
        /// <param name="seriesIndex"></param>
        /// <returns></returns>
        static public TaskCompletionSource<SliceData> getStudySliceData(string studyName, int sliceIndex, ESliceOrientation sliceOrientation, int seriesIndex) {
            TaskCompletionSource<SliceData> sliceDataPromise = new TaskCompletionSource<SliceData>();
            Debug.Log(string.Format("Fetching data for study={0}, slice={1}, orientation={2}, series={3}", studyName, sliceIndex, (int)sliceOrientation, seriesIndex));

            if (INSTANCE.studies.ContainsKey(studyName)) {
                Debug.Log("Study is already managed");
                StudyData studyData = INSTANCE.studies[studyName];
                if (studyData.isSliceInRange(sliceIndex, sliceOrientation, seriesIndex)) {
                    SliceData sliceData = studyData.getSliceData(sliceIndex, sliceOrientation, seriesIndex);
                    if (sliceData != null) {
                        Debug.Log("SliceData is already cached. Serving it.");
                        sliceDataPromise.SetResult(sliceData);
                    } else {
                        Debug.Log("SliceData is not cached. Requesting it.");
                        INSTANCE.makeStudySliceDataRequest(studyName, sliceIndex, sliceOrientation, seriesIndex, sliceDataPromise);
                    }
                } else {
                    Debug.Log("SliceData requested is out of range.");
                    sliceDataPromise.SetResult(null);
                }
            } else {
                Debug.Log("No study with that name was found. Requesting new data.");
                INSTANCE.makeStudySliceDataRequest(studyName, sliceIndex, sliceOrientation, seriesIndex, sliceDataPromise);
            }

            return sliceDataPromise;
        }

        /// <summary>
        /// Sends a request to the server to get a slice of data with the given information. 
        /// If this data was asked for before, this request is instead added to a list of requests waiting for the same data.
        /// </summary>
        /// <param name="studyName">Name of the study.</param>
        /// <param name="sliceIndex">Index of the wanted slice.</param>
        /// <param name="sliceOrientation">Orientation of the wanted slice.</param>
        /// <param name="seriesIndex">Series where the wanted slice is.</param>
        /// <param name="promise">The promise to resolve when the data is received.</param>
        private void makeStudySliceDataRequest(string studyName, int sliceIndex, ESliceOrientation sliceOrientation, int seriesIndex, TaskCompletionSource<SliceData> promise) {
            StudySlicePendingRequestID studyPendingRequestID = new StudySlicePendingRequestID();
            studyPendingRequestID.studyName = studyName;
            studyPendingRequestID.sliceIndex = sliceIndex;
            studyPendingRequestID.sliceOrientation = (int)sliceOrientation;
            studyPendingRequestID.seriesIndex = seriesIndex;
            studyPendingRequestID.modelX = 66f;
            studyPendingRequestID.modelY = 66f;
            studyPendingRequestID.modelZ = 66f;

            Debug.Log(string.Format("Checking for StudySliceDataRequest with ID: Name={0}, Index={1}, Orientation={2}, Series={3}, X={4}, y={5}, z={6}", studyPendingRequestID.studyName,
                studyPendingRequestID.sliceIndex, studyPendingRequestID.sliceOrientation, studyPendingRequestID.seriesIndex, studyPendingRequestID.modelX, studyPendingRequestID.modelY, studyPendingRequestID.modelZ));

            if (INSTANCE.StudySlicePendingRequests.ContainsKey(studyPendingRequestID)) {
                Debug.Log("Request already found. Queueing a promise");
                INSTANCE.StudySlicePendingRequests[studyPendingRequestID].promises.Add(promise);
            } else {
                Debug.Log("No previous request found. Creating new request and queueing promise");
                SlicePendingRequest slicePendingRequest = new SlicePendingRequest();
                slicePendingRequest.dataName = studyName;
                slicePendingRequest.promises.Add(promise);

                INSTANCE.StudySlicePendingRequests.Add(studyPendingRequestID, slicePendingRequest);

                SimpleJSON.JSONObject sliceDataRequest =
                    RequestMaker.makeStudyImageSliceDataRequest(studyName, sliceIndex, sliceOrientation, seriesIndex);
                ServerConnection.sendMessage(sliceDataRequest);
            }
        }
    }
}