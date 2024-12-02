using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fi {
    public class ImageData {
        public string DataName { get; private set; }

        private readonly int[] dimensions = { 0, 0, 0 };
        public int[] Dimensions {
            get {
                return dimensions;
            }
            private set {
                if (value != null && value.Length == 3) {
                    dimensions[0] = value[0];
                    dimensions[1] = value[1];
                    dimensions[2] = value[2];
                } else {
                    Debug.Log(string.Format("Failed to set dimensions for {0} because dimension data was missing.", this.DataName));
                }
            }
        }

        private readonly float[] spacing = { 0, 0, 0 };
        public float[] Spacing {
            get {
                return spacing;
            }
            private set {
                if (value != null && value.Length == 3) {
                    spacing[0] = value[0];
                    spacing[1] = value[1];
                    spacing[2] = value[2];
                } else {
                    Debug.Log(string.Format("Failed to set spacing for {0} because spacing data was missing", this.DataName));
                }
            }
        }

        List<SliceData> transverses = new List<SliceData>();
        List<SliceData> sagittals = new List<SliceData>();
        List<SliceData> coronals = new List<SliceData>();

        public ImageData(string dataName, int[] dims, float[] spac) {
            DataName = dataName;
            Dimensions = dims;
            Spacing = spac;

            transverses.Capacity = dimensions[2];
            sagittals.Capacity = dimensions[0];
            coronals.Capacity = dimensions[1];

            for (int i = 0; i < dimensions[2]; i++) {
                transverses.Add(null);
            }

            for (int i = 0; i < dimensions[0]; i++) {
                sagittals.Add(null);
            }

            for (int i = 0; i < dimensions[1]; i++) {
                coronals.Add(null);
            }
        }

        public void setSlice(int sliceIndex, ESliceOrientation sliceOrientation, Color[] values) {
            switch (sliceOrientation) {
                case ESliceOrientation.XY:
                    this.setSliceXY(sliceIndex, values);
                    break;
                case ESliceOrientation.YZ:
                    setSliceYZ(sliceIndex, values);
                    break;
                case ESliceOrientation.XZ:
                    setSliceXZ(sliceIndex, values);
                    break;
                default:
                    Debug.Log("Failed to set slice data. Unknown orientation was given.");
                    break;
            }
        }

        void setSliceXY(int sliceIndex, Color[] values) {
            if (sliceIndex < 0 || sliceIndex >= transverses.Count) {
                Debug.Log("Failed to set XY slice. Slice index out of range.");
                return;
            }

            SliceData sliceData = transverses[sliceIndex];
            if (sliceData == null) {
                sliceData = new SliceData(this.DataName, dimensions[0], dimensions[1], values);
                sliceData.Dimensions = this.Dimensions;
                sliceData.Spacing = this.Spacing;
                transverses[sliceIndex] = sliceData;
            } else {
                sliceData.setTextureData(dimensions[0], dimensions[1], values);
            }
        }

        void setSliceYZ(int sliceIndex, Color[] values) {
            if (sliceIndex < 0 || sliceIndex >= sagittals.Count) {
                Debug.Log("Failed to set YZ slice. Slice index out of range.");
                return;
            }

            SliceData sliceData = sagittals[sliceIndex];
            if (sliceData == null) {
                sliceData = new SliceData(this.DataName, dimensions[1], dimensions[2], values);
                sliceData.Dimensions = this.Dimensions;
                sliceData.Spacing = this.Spacing;
                sagittals[sliceIndex] = sliceData;
            } else {
                sliceData.setTextureData(dimensions[1], dimensions[2], values);
            }
        }

        void setSliceXZ(int sliceIndex, Color[] values) {
            if (sliceIndex < 0 || sliceIndex >= coronals.Count) {
                Debug.Log("Failed to set XZ slice. Slice index out of range.");
                return;
            }

            SliceData sliceData = coronals[sliceIndex];
            if (sliceData == null) {
                sliceData = new SliceData(this.DataName, dimensions[0], dimensions[2], values);
                sliceData.Dimensions = this.Dimensions;
                sliceData.Spacing = this.Spacing;
                coronals[sliceIndex] = sliceData;
            } else {
                sliceData.setTextureData(dimensions[0], dimensions[2], values);
            }
        }

        public SliceData getSliceData(int sliceIndex, ESliceOrientation sliceOrientation) {
            switch (sliceOrientation) {
                case ESliceOrientation.XY: {
                    if (sliceIndex >= transverses.Count) {
                        return null;
                    } else {
                        return transverses[sliceIndex];
                    }
                }
                case ESliceOrientation.YZ: {
                    if (sliceIndex >= sagittals.Count) {
                        return null;
                    } else {
                        return sagittals[sliceIndex];
                    }
                }
                case ESliceOrientation.XZ: {
                    if (sliceIndex >= coronals.Count) {
                        return null;
                    } else {
                        return coronals[sliceIndex];
                    }
                }
                default:
                    return null;
            }
        }

        public bool isSliceInRange(int sliceIndex, ESliceOrientation sliceOrientation) {
            if (sliceIndex < 0) {
                return false;
            }
            switch (sliceOrientation) {
                case ESliceOrientation.XY: {
                    if (sliceIndex >= transverses.Count) {
                        return false;
                    } else {
                        return true;
                    }
                }
                case ESliceOrientation.YZ: {
                    if (sliceIndex >= sagittals.Count) {
                        return false;
                    } else {
                        return true;
                    }
                }
                case ESliceOrientation.XZ: {
                    if (sliceIndex >= coronals.Count) {
                        return false;
                    } else {
                        return true;
                    }
                }
                default:
                    return false;
            }
        }
    }
}