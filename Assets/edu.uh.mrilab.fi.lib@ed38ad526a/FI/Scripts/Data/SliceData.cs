using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fi {
    public class SliceData {
        /// <summary>
        /// The name of the data.
        /// </summary>
        public string DataName { get; private set; }

        /// <summary>
        /// The texture itself.
        /// </summary>
        public Texture2D Texture { get; private set; }

        private int[] dimensions = { 0, 0, 0 };
        public int[] Dimensions {
            get {
                return dimensions;
            }
            set {
                if (value != null && value.Length == 3) {
                    dimensions[0] = value[0];
                    dimensions[1] = value[1];
                    dimensions[2] = value[2];
                }
            }
        }

        private readonly float[] spacing = { 0, 0, 0 };
        public float[] Spacing {
            get {
                return spacing;
            }
            set {
                if (value != null && value.Length == 3) {
                    spacing[0] = value[0];
                    spacing[1] = value[1];
                    spacing[2] = value[2];
                } else {
                    Debug.Log(string.Format("Failed to set spacing for {0} because spacing data was missing", this.DataName));
                }
            }
        }

        /// <summary>
        /// Constructs a SliceData object.
        /// </summary>
        /// <param name="dataName">The name of the data.</param>
        /// <param name="dimX">The x-dimension of the texture.</param>
        /// <param name="dimY">The y-dimension of the texture.</param>
        /// <param name="values">The values that make up the texture.</param>
        public SliceData(string dataName, int dimX, int dimY, Color[] values) {
            DataName = dataName;
            this.setTextureData(dimX, dimY, values);
        }

        public void setTextureData(int dimX, int dimY, Color[] values) {
            if (values == null) {
                Debug.Log("Failed to set texture data. Given data is null");
                return;
            } else if (dimX <= 0 || dimY <= 0) {
                Debug.Log("Failed to set texture data. Given dimensions are incorrect.");
                return;
            } else if (dimX * dimY != values.Length) {
                Debug.Log("Failed to set texture data. Given dimensions do not match given data.");
                return;
            }

            Texture2D texture = new Texture2D(dimX, dimY, TextureFormat.RGBAFloat, false);
            texture.SetPixels(values);

            texture.Apply();
            Texture = texture;
        }
    }
}
