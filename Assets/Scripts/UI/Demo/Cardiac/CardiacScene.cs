using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

namespace fi
{
    public class CardiacScene : MonoBehaviour
    {
        public ImageVisual ImageSlicePrefab;
        public ModelVisual ModelPrefab;

        ImageVisual imageSlice;
        ModelVisual modelEpi;
        ModelVisual modelEndo;

        /// <summary>
        /// TextAsset Contains all the individual slices with its phases
        /// Both Meshes corresponds to the Different Phases
        /// </summary>
        public TextAsset[] textAsset;
        public Mesh[] epiMesh;
        public Mesh[] endoMesh;

        /// <summary>
        /// Materials for Both Mesh
        /// </summary>
        public Material epiMat;
        public Material endoMat;

        float scale = 0.75f;

        /// <summary>
        /// Dimensions of the slices : 10 slices of size [192x256]  
        /// </summary>
        int[] Dimensions = { 192, 256, 10 };
        private readonly float[] spacing = { 0.027f, 0.027f, 0.027f };

        /// <summary>
        /// Dictionary that stores the individual Phase (25) and all its slices (array of 10 slices)
        /// Key is Phase number (0-24): Size 25
        /// </summary>
        Dictionary<int, int[]> sliceDataValues = new Dictionary<int, int[]>();

        /// <summary>
        /// Dictionary to store Final Color Values corresponding to each Phase and Slice (25x10)
        /// Each key is combination of (Phase, Slice)
        /// </summary>
        Dictionary<Tuple<int, int>, Color[]> sliceColorValues = new Dictionary<Tuple<int, int>, Color[]>();

        static bool animateCheck = false;

        /// <summary>
        /// Current Slice and Phase Value
        /// </summary>
        int Phase;
        int Slice;

        // Start is called before the first frame update
        void Start()
        {
            Vector3 sceneScale = new Vector3(
                transform.localScale.x * scale,
                transform.localScale.y * scale,
                transform.localScale.z * scale
                );
            transform.localScale = sceneScale;

            //Create Endocardium
            modelEndo = Instantiate(ModelPrefab, transform);
            modelEndo.GetComponent<MeshFilter>().mesh = endoMesh[0]; //this used to be modelEndo.insideoutModel.GetComponent<MeshFilter>().mesh = endoMesh[0] -- change that back and comment it out if it wasn't supposed to be there cause idk why Hossein Removed it. 
            //modelEndo.insideoutModel.AddComponent<BoxCollider>();

            //modelEndo.insideoutModel.GetComponent<Renderer>().material = endoMat;
            modelEndo.GetComponent<Renderer>().material = endoMat;
            //modelEndo.GetComponent<MeshFilter>().mesh = endoMesh[Phase];
            modelEndo.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
            modelEndo.gameObject.name = "Endocardium";

            //Create Epicardium
            modelEpi = Instantiate(ModelPrefab, transform);
            modelEpi.GetComponent<MeshFilter>().mesh = epiMesh[0]; //this used to be modelEpi.insideoutModel.GetComponent<MeshFilter>().mesh = epiMesh[0] -- change that back and comment it out if it wasn't supposed to be there cause idk why Hossein Removed it. 
            //modelEpi.insideoutModel.AddComponent<BoxCollider>();
            //modelEpi.insideoutModel.GetComponent<Renderer>().material = epiMat;
            //modelEpi.GetComponent<MeshFilter>().mesh = epiMesh[Phase];
            modelEpi.GetComponent<Renderer>().material = epiMat;
            modelEpi.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
            modelEpi.transform.localPosition = modelEndo.transform.localPosition;
            modelEpi.gameObject.name = "Epicardium";

            //Instantiate Slice Visual
            imageSlice = Instantiate(ImageSlicePrefab, transform);
            imageSlice.transform.Rotate(0f, 0f, -47f);
            imageSlice.orientationPivot.gameObject.transform.Rotate(90.0f, 0f, 0f);
            imageSlice.gameObject.name = "StudySlice";
            imageSlice.transform.localPosition = new Vector3(modelEndo.transform.localPosition.x - 0.368f, modelEndo.transform.localPosition.y + 0.336f, modelEndo.transform.localPosition.z + 0.431f);

            setValues(1, 1);
            dataChange(Slice);
        }

        /// <summary>
        /// Set Values for Current Phase and Slice.
        /// </summary>
        /// <param name="p"> Int Value of Current Phase </param> 
        /// <param name="s"> Int Value of Current Slice </param>  
        void setValues(int p, int s)
        {
            if (Phase != p)
                Phase = p;

            if (Slice != s)
                Slice = s;
        }

        /// <summary>
        /// Read Slice Data.
        /// </summary>
        /// <param name="idx"> Slice Value (0-9)</param>
        void readData(int idx)
        {
            string text = textAsset[idx].text;
            int[] values = Array.ConvertAll(text.Split(','), int.Parse);
            sliceDataValues.Add(idx, values);
        }

        /// <summary>
        /// Function to Change Slice Data Based on Index and Phase.
        /// </summary>
        /// <param name="index"> Index for the currect Phase</param>
        public void dataChange(int index)
        {
            if (index < 0 || index > Dimensions[2])
            {
                Debug.Log(string.Format("Incorrect Slice Requested {0} Max Slice: {1}", index, Dimensions[2]));
                return;
            }

            imageSlice = this.transform.Find("StudySlice").GetComponent<ImageVisual>();
            int[] arr_slice = new int[256 * 192];
            Color[] values = new Color[Dimensions[0] * Dimensions[1]];

            //Check if Already Parsed
            Tuple<int, int> key = new Tuple<int, int>(Phase, index);
            if (sliceColorValues.ContainsKey(key))
                values = sliceColorValues[key];
            else
            {
                int pos = (index - 1) * (Dimensions[0] * Dimensions[1]);
                //Debug.Log(string.Format("Copy Data for {0}", index));

                //Check if Data Read and Loaded
                if (!sliceDataValues.ContainsKey(Phase - 1))
                    readData(Phase - 1);

                Array.Copy(sliceDataValues[Phase - 1], pos, arr_slice, 0, Dimensions[0] * Dimensions[1]);

                //Debug.Log(string.Format("Index: {0} Pos: {1}", index, pos));

                getColor(ESliceOrientation.XY, arr_slice, values);

                //Debug.Log(string.Format("GetColor {0}", index));
                sliceColorValues.Add(key, values);
            }

            Texture2D texture = new Texture2D(Dimensions[0], Dimensions[1], TextureFormat.RGBAFloat, false);
            texture.SetPixels(values);
            texture.Apply();
            imageSlice.backTexture.GetComponent<Renderer>().material.mainTexture = texture;
            imageSlice.sliceTranslation.GetComponent<Renderer>().material.mainTexture = texture;

            Vector3 slicePosition = imageSlice.ImageSlicePosition;

            imageSlice.sliceTranslation.transform.localPosition = new Vector3(
               slicePosition.x,
               slicePosition.y,
               +spacing[2] * index);

            setValues(Phase, index);
            //Debug.Log(string.Format("Action Complete {0}", index));
        }

        /// <summary>
        /// Calculate Color Values Based on the Slice data values.
        /// </summary>
        /// <param name="sliceOrientation"> Slice Orientation XY </param>
        /// <param name="slice"> Slice Index Value </param>
        /// <param name="values"> Return Value: Color Values of Slice </param>
        void getColor(ESliceOrientation sliceOrientation, int[] slice, Color[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                float value = ((float)(slice[i] / 255.0));
                Color pixel = new Color(value, value, value);

                // If the orientation is XY, the x-pixels have to be mirrored across
                if (sliceOrientation == ESliceOrientation.XY)
                {
                    int x = i % Dimensions[0];
                    int y = i / Dimensions[0];
                    x = Dimensions[0] - x - 1;

                    int fixedPosition = Dimensions[0] * y + x;
                    values[fixedPosition] = pixel;

                    //Debug.Log("XY Orientation");
                }
                else
                {
                    //Debug.Log("Non-XY Dimension");
                    values[i] = pixel;
                }
            }
        }

        /// <summary>
        /// Function to Change Slice Data Based on new Phase Value.
        /// </summary>
        /// <param name="phase"> New Phase value for Current Slice Index </param>
        public void phaseChange(int phase)
        {
            Debug.Log(phase);
            if (phase <= 0 || phase > 25)
            {
                Debug.Log(string.Format("Incorrect Phase Requested {0} Max Value: 25", phase));
                return;
            }

            modelEpi.GetComponent<MeshFilter>().mesh = epiMesh[phase - 1]; //this used to be modelEpi.insideoutModel.GetComponent<MeshFilter>().mesh = epiMesh[phase-1] -- change that back and comment it out if it wasn't supposed to be there cause idk why Hossein Removed it. 
            //modelEpi.insideoutModel.GetComponent<Renderer>().material = epiMat;
            modelEpi.GetComponent<Renderer>().material = epiMat;

            modelEndo.GetComponent<MeshFilter>().mesh = endoMesh[phase - 1]; //this used to be modelEndo.insideoutModel.GetComponent<MeshFilter>().mesh = endoMesh[phase-1] -- change that back and comment it out if it wasn't supposed to be there cause idk why Hossein Removed it. 
            //modelEndo.insideoutModel.GetComponent<Renderer>().material = endoMat;
            modelEndo.GetComponent<Renderer>().material = endoMat;

            setValues(phase, Slice);
            dataChange(Slice);

        }

        /// <summary>
        /// Function to simulate Animation of Beating Heart.
        /// </summary>
        /// <param name="val"></param>
        public void changeAnimation(bool val)
        {
            if (animateCheck != val)
            {
                animateCheck = val;
                StartCoroutine(Animate());
            }
        }

        IEnumerator Animate()
        {
            int i = 1;
            while (animateCheck)
            {
                phaseChange(i);
                i++;

                if (i == 26)
                    i = 0;

                yield return new WaitForSeconds(.1f);
            }
        }

        /// <summary>
        /// Show/Hide Surface of mesh based on UI toggle.
        /// </summary>
        /// <param name="val"></param>
        public void surface(bool val)
        {
            modelEndo.gameObject.SetActive(val);
            modelEpi.gameObject.SetActive(val);
        }
    }
}
