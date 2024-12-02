using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fi {
    public class StudyData {
        public string StudyName { get; private set; }

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
                    Debug.Log(string.Format("Failed to set dimensions for {0} because dimension data was missing.", this.StudyName));
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
                    Debug.Log(string.Format("Failed to set spacing for {0} because spacing data was missing", this.StudyName));
                }
            }
        }

        List<ImageData> series = new List<ImageData>();

        public StudyData(string studyName, int seriesCount, int[] dims, float[] spacing) {
            StudyName = studyName;
            if (seriesCount > 0) {
                series.Capacity = seriesCount;
                for (int i = 0; i < seriesCount; i++) {
                    series.Add(null);
                }
            }
            Dimensions = dims;
            Spacing = spacing;
        }

        public void setSliceData(int sliceIndex, ESliceOrientation sliceOrientation, int seriesIndex, byte[] bytes) {
            if (seriesIndex < 0 || seriesIndex >= series.Count) {
                Debug.Log("Failed to set slice data because series index is out of range.");
                return;
            }

            ImageData imageData = series[seriesIndex];
            if (imageData == null) {
                imageData = new ImageData(this.StudyName, this.Dimensions, this.Spacing);
                series[seriesIndex] = imageData;
            }

            Color[] values = new Color[bytes.Length / sizeof(float)];
            for (int i = 0; i < values.Length; i++) {
                float value = System.BitConverter.ToSingle(bytes, i * sizeof(float));
                Color pixel = new Color(value, value, value);

                // If the orientation is XY, the x-pixels have to be mirrored across
                if (sliceOrientation == ESliceOrientation.XY) {
                    int x = i % Dimensions[0];
                    int y = i / Dimensions[0];
                    x = Dimensions[0] - x - 1;

                    int fixedPosition = Dimensions[0] * y + x;
                    values[fixedPosition] = pixel;
                } else {
                    values[i] = pixel;
                }
            }

            imageData.setSlice(sliceIndex, sliceOrientation, values);
        }

        public ImageData getSeriesData(int seriesIndex) {
            if (seriesIndex < 0 || seriesIndex >= series.Count) {
                return null;
            } else {
                return series[seriesIndex];
            }
        }

        public bool isSeriesInRange(int seriesIndex) {
            if (seriesIndex < 0 || seriesIndex >= series.Count) {
                return false;
            } else {
                return true;
            }
        }

        public SliceData getSliceData(int sliceIndex, ESliceOrientation sliceOrientation, int seriesIndex) {
            if (!this.isSeriesInRange(seriesIndex)) {
                return null;
            }

            ImageData imageData = series[seriesIndex];
            if (imageData == null) {
                return null;
            } else {
                return imageData.getSliceData(sliceIndex, sliceOrientation);
            }
        }

        public bool isSliceInRange(int sliceIndex, ESliceOrientation sliceOrientation, int seriesIndex) {
            if (!this.isSeriesInRange(seriesIndex)) {
                return false;
            }

            switch (sliceOrientation) {
                case ESliceOrientation.XY: {
                    if (sliceIndex >= Dimensions[2]) {
                        return false;
                    } else {
                        return true;
                    }
                }
                case ESliceOrientation.YZ: {
                    if (sliceIndex >= Dimensions[0]) {
                        return false;
                    } else {
                        return true;
                    }
                }
                case ESliceOrientation.XZ: {
                    if (sliceIndex >= Dimensions[1]) {
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