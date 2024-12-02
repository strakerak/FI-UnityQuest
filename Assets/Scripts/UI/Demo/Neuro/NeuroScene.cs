using System.Collections.Generic;
using UnityEngine;
using System;

namespace fi
{
    public class NeuroScene : MonoBehaviour
    {
        public ImageVisual ImageSlicePrefab;
        public ModelVisual ModelPrefab;
        //public AssemblyVisual AssemblyPrefab;

        ImageVisual imageSlice;
        ModelVisual model;

        Dictionary<int, Color[]> sliceColorValues = new Dictionary<int, Color[]>();

        public TextAsset textAsset;
        public Mesh modelmesh;

        float scale = 0.75f;

        int[] Dimensions = { 144, 206, 166 };
        private readonly float[] spacing = { 0.002f, 0.002f, 0.002f };

        int[] all_textdata;

        // Start is called before the first frame update
        void Start()
        {
            Vector3 sceneScale = new Vector3(
                transform.localScale.x * scale,
                transform.localScale.y * scale,
                transform.localScale.z * scale
                );
            transform.localScale = sceneScale;

            // Read All Slice Data from Text File
            //Debug.Log("Read File");
            string text = textAsset.text;
            all_textdata = Array.ConvertAll(text.Split(','), int.Parse);
            //Debug.Log("Read File End");

            //Instantiate Vessel Visual
            model = Instantiate(ModelPrefab, transform);
            //model.insideoutModel.GetComponent<MeshFilter>().mesh = modelmesh;
            //model.insideoutModel.AddComponent<BoxCollider>();
            //model.insideoutModel.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            model.transform.localScale = new Vector3(0.006f, 0.006f, 0.006f);
            //model.transform.localPosition = new Vector3(imageSlice.transform.localPosition.x - 0.48f, imageSlice.transform.localPosition.y - 0.27f, imageSlice.transform.localPosition.z - 0.045f);

            //Instantiate Slice Visual
            imageSlice = Instantiate(ImageSlicePrefab, transform);
            imageSlice.orientationPivot.gameObject.transform.Rotate(0f, 0f, 180f);
            imageSlice.transform.localPosition = new Vector3(model.transform.localPosition.x + 0.48f, model.transform.localPosition.y + 0.27f, model.transform.localPosition.z + 0.045f);
            dataChange(1);

        }

        public void dataChange(int index)
        {
            if (index > 0 && index <= Dimensions[2])
            {
                imageSlice = this.transform.Find("SliceVisual(Clone)").GetComponent<ImageVisual>();
                int[] arr_slice = new int[144 * 206];
                Color[] values = new Color[Dimensions[0] * Dimensions[1]];

                //Check if Already Parsed
                if (sliceColorValues.ContainsKey(index))
                {
                    values = sliceColorValues[index];
                }
                else
                {
                    int pos = (index - 1) * (Dimensions[0] * Dimensions[1]);
                    //Debug.Log(string.Format("Copy Data for {0}", index));
                    Array.Copy(all_textdata, pos, arr_slice, 0, Dimensions[0] * Dimensions[1]);

                    //Debug.Log(string.Format("Index: {0} Pos: {1}", index, pos));

                    getColor(ESliceOrientation.XY, arr_slice, values);
                    //Debug.Log(string.Format("GetColor {0}", index));
                    sliceColorValues.Add(index, values);
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

                //Debug.Log(string.Format("Action Complete {0}", index));
            }
            else
                Debug.Log(string.Format("Incorrect Slice Requested {0} Max Slice: {1}", index, Dimensions[2]));
        }

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
                }
                else
                {
                    values[i] = pixel;
                }
            }
        }
    }
}
