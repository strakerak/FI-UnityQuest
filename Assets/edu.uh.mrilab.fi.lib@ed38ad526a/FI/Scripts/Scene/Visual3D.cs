using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fi {
    public class Visual3D : MonoBehaviour {
        public EObjectType ObjectType { get; protected set; }

        /// <summary>
        /// The visibility state of this object.
        /// </summary>
        bool visible;
        public bool Visible {
            get {
                return visible;
            }
            set {
                visible = value;
                gameObject.SetActive(visible);
            }
        }

        /// <summary>
        /// The parent SceneObject
        /// </summary>
        Visual3D parentVisual;
        public Visual3D ParentVisual {
            get {
                return parentVisual;
            } set {
                if (value == parentVisual) {
                    return;
                }
                parentVisual = value;
                if (parentVisual == null) {
                    transform.SetParent(null);
                } else {
                    transform.SetParent(parentVisual.transform);
                }
            }
        }

        /// <summary>
        /// Resets the position, orientation, and scale of the object.
        /// </summary>
        public void resetTransform() {
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localEulerAngles = new Vector3(0, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }

        /// <summary>
        ///  Sets the position, orientation, and scale of the object.
        /// </summary>
        /// <param name="array">The 4x4 transformation matrix.</param>
        public void setTransform(float[] array) {
            if (array == null || array.Length != 16) {
                return;
            }

            Matrix4x4 m = arrayToMatrix(array);
            setTransform(m);
        }

        /// <summary>
        /// Sets the position, orientation, and scale of the object.
        /// </summary>
        /// <param name="matrix">The 4x4 transformation matrix.</param>
        public void setTransform(Matrix4x4 matrix) {
            Vector3 pos = matrix.GetColumn(3);
            pos.x = pos.x * -1;
            Quaternion rot = matrix.rotation;
            Vector3 scl = new Vector3(
                matrix.GetColumn(0).magnitude,
                matrix.GetColumn(1).magnitude,
                matrix.GetColumn(2).magnitude
                );

            Vector3 angles = rot.eulerAngles;
            angles.y = -1 * angles.y;
            angles.z = -1 * angles.z;

            Debug.Log(string.Format("Rotation Values for {0}: ({1}, {2}, {3}, {4})", this.name, rot.w, rot.x, rot.y, rot.z));

            transform.localPosition = pos;
            transform.localEulerAngles = angles;
            transform.localScale = scl;
        }

        /// <summary>
        /// Convert a 16 value array to a Matrix4x4
        /// </summary>
        /// <param name="array">The array with 16 entries.</param>
        /// <returns>The constructed Matrix4x4. An empty Matrix4x4 is given array is invalid.</returns>
        static public Matrix4x4 arrayToMatrix(float[] array) {
            if (array == null || array.Length != 16) {
                return new Matrix4x4();
            }

            Vector4 column0 = new Vector4();
            Vector4 column1 = new Vector4();
            Vector4 column2 = new Vector4();
            Vector4 column3 = new Vector4();
            for (int i = 0; i < 4; i++) {
                column0[i] = array[i * 4];
                column1[i] = array[i * 4 + 1];
                column2[i] = array[i * 4 + 2];
                column3[i] = array[i * 4 + 3];
            }
            Matrix4x4 matrix = new Matrix4x4(column0, column1, column2, column3);
            return matrix;
        }
    }
}