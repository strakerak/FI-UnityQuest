using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fi {
    public class ImageVisual : Visual3D {

        public GameObject orientationPivot;
        public GameObject sliceTranslation;
        public GameObject backTexture;

        public Vector3 ImageSlicePosition { get; private set; }

        public SliceData SliceData { get; private set; } = null;

        bool UpdateSlice { get; set; } = false;

        public ESliceOrientation SliceOrientation { get; private set; }
        public int SliceIndex { get; private set; }
        public int SeriesIndex { get; private set; }

        private int[] dimensions = { 0, 0, 0 };
        public int[] Dimensions {
            get {
                return dimensions;
            }
            private set {
                if (value != null && value.Length == 3) {
                    dimensions[0] = value[0];
                    dimensions[1] = value[1];
                    dimensions[2] = value[2];
                }
            }
        }

        private float[] spacing = { 0.0f, 0.0f, 0.0f };
        public float[] Spacing {
            get {
                return spacing;
            }
            private set {
                if (value != null && value.Length == 3) {
                    spacing[0] = value[0];
                    spacing[1] = value[1];
                    spacing[2] = value[2];
                }
            }
        }

        // Same values as spacing above, but these are set relative to the current slice
        public float WidthSpacing { get; private set; }
        public float HeightSpacing { get; private set; }
        public float SliceSpacing { get; private set; }

        // Same as dimensions, but these are set relative to the current slice. Used to define texture dimensions
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Depth { get; private set; }

        private void Awake() {
            this.ObjectType = EObjectType.STUDY_IMAGE_SLICE;
            ImageSlicePosition = sliceTranslation.transform.localPosition;

            // Flip the normals on the back texture
            Mesh backMesh = backTexture.GetComponent<MeshFilter>().mesh;
            backMesh.triangles = backMesh.triangles.Reverse().ToArray();
        }

        // Update is called once per frame
        void Update() {
            if (UpdateSlice) {
                Debug.Log(string.Format("Changing texture of ImageVisual: {0}", this.name));
                this.applyOrientationChange();
                this.applySliceChange();
                orientationPivot.transform.localScale = new Vector3(Width * WidthSpacing, Height * HeightSpacing, 1);

                sliceTranslation.GetComponent<Renderer>().material.mainTexture = SliceData.Texture;
                backTexture.GetComponent<Renderer>().material.mainTexture = SliceData.Texture;
                UpdateSlice = false;
            }
        }

        public void setSliceData(SliceData sliceData, int sliceIndex, ESliceOrientation orientation, int seriesIndex) {
            Debug.Log(string.Format("Setting texture data representing slice={0}, orientation={1}, series={2}", sliceIndex, (int)orientation, seriesIndex));
            if (sliceData == null) {
                Debug.Log("Failed to set texture data because texture is null");
                return;
            }

            SliceData = sliceData;
            Dimensions = sliceData.Dimensions;
            Spacing = sliceData.Spacing;
            SliceIndex = sliceIndex;
            SliceOrientation = orientation;
            this.calculateSliceUnits();
            SeriesIndex = seriesIndex;
            UpdateSlice = true;
        }

        void calculateSliceUnits() {
            switch (SliceOrientation) {
                case ESliceOrientation.XY:
                    Width = Dimensions[0];
                    Height = Dimensions[1];
                    Depth = Dimensions[2];
                    WidthSpacing = Spacing[0];
                    HeightSpacing = Spacing[1];
                    SliceSpacing = Spacing[2];
                    break;
                case ESliceOrientation.YZ:
                    Width = Dimensions[1];
                    Height = Dimensions[2];
                    Depth = Dimensions[0];
                    WidthSpacing = Spacing[1];
                    HeightSpacing = Spacing[2];
                    SliceSpacing = Spacing[0];
                    break;
                case ESliceOrientation.XZ:
                    Width = Dimensions[0];
                    Height = Dimensions[2];
                    Depth = Dimensions[1];
                    WidthSpacing = Spacing[0];
                    HeightSpacing = Spacing[2];
                    SliceSpacing = Spacing[1];
                    break;
                default:
                    Debug.LogWarning(string.Format("Failed to calculate ImageSlice={0} units because orientation is unknown.", this.name));
                    break;
            }
        }

        void applySliceChange() {
            switch (SliceOrientation) {
                case ESliceOrientation.XY:
                    // Change the quad object's position to where it suppose to be according to its index
                    sliceTranslation.transform.localPosition = new Vector3(
                        ImageSlicePosition.x,
                        ImageSlicePosition.y,
                        SliceSpacing * SliceIndex);
                    break;
                case ESliceOrientation.YZ:
                    sliceTranslation.transform.localPosition = new Vector3(
                        ImageSlicePosition.x,
                        ImageSlicePosition.y,
                        -1 * SliceSpacing * SliceIndex);
                    break;
                case ESliceOrientation.XZ:
                    sliceTranslation.transform.localPosition = new Vector3(
                        ImageSlicePosition.x,
                        ImageSlicePosition.y,
                        SliceSpacing * SliceIndex);
                    break;
            }
        }

        void applyOrientationChange() {
            if (SliceOrientation == ESliceOrientation.XY) {
                orientationPivot.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                orientationPivot.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

            } else if (SliceOrientation == ESliceOrientation.YZ) {
                orientationPivot.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 90.0f);
                orientationPivot.transform.localPosition = new Vector3(0.0f, Width * WidthSpacing, 0.0f);

            } else if (SliceOrientation == ESliceOrientation.XZ) {
                orientationPivot.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 0.0f);
                orientationPivot.transform.localPosition = new Vector3(-1 * Width * WidthSpacing, 0.0f, 0.0f);
            }
        }
    }
}