using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fi {
    public class ModelVisual : Visual3D {
        /// <summary>
        /// When ModelData is set externally, it is placed here. 
        /// The object's update function then assigns it to be visualized.
        /// </summary>
        ModelData pendingData = null;

        /// <summary>
        /// The data that is currently being displayed.
        /// </summary>
        ModelData modelData = null;

        /// <summary>
        /// the ModelData being visualized. Used to be set externally. 
        /// </summary>
        public ModelData ModelData {
            get {
                if (pendingData != null) {
                    return pendingData;
                } else {
                    return modelData;
                }
            } set {
                pendingData = value;
            }
        }

        /// <summary>
        /// Sets the type of this object to Model.
        /// </summary>
        private void Awake() {
            this.ObjectType = EObjectType.MODEL;
        }

        private void Update() {
            if (pendingData != null) {
                Debug.Log(string.Format("Changing mesh of ModelVisual: {0}", this.name));
                modelData = pendingData;
                pendingData = null;

                this.GetComponent<MeshFilter>().mesh = modelData.Mesh;
                if (modelData.TextureCoordinates.Length > 0) {
                    this.GetComponent<MeshFilter>().mesh.uv = modelData.TextureCoordinates;
                }

                if (ModelData.Textures.Count != 0) {
                    // TODO: Unity complains that multiple materials in the same renderer is bad for performance.
                    // What's the best way to do that?
                    Material[] materials = new Material[ModelData.Textures.Count];
                    for (int i = 0; i < modelData.Textures.Count; i++) {
                        Material materialTexture = new Material(Shader.Find("Mixed Reality Toolkit/Standard"));
                        // TODO: What's the best way to handle this?
                        // Set cull mode to Front (1) face so it renders properly. Other modes: off (0), front (1), and back (2)
                        materialTexture.SetInt("_CullMode", 0);
                        //materialTexture.SetFloat("_Metallic", 1);
                        //materialTexture.SetFloat("_EnableDirectionalLight", 1);
                        materialTexture.SetFloat("_SphericalHarmonics", 1);
                        materialTexture.SetFloat("_Reflections", 1);

                        // TODO: There are here for a project. Remove once textures are properly sort out.
                        if (this.gameObject.name.EndsWith("/mount")) {
                            materialTexture.color = new Color(0.6f, 0.6f, 0.6f);
                        } else if (this.gameObject.name.EndsWith("/switch_")) {
                            materialTexture.color = new Color(0.75f, 0.75f, 0.75f);
                        }

                        materialTexture.mainTexture = modelData.Textures[i];
                        materials[i] = materialTexture;
                    }

                    MeshRenderer renderer = this.GetComponent<MeshRenderer>();
                    renderer.materials = materials;
                }
                
                transform.GetComponentInParent<Scene>().updateBoxCollider();
            }
        }

        /// <summary>
        /// Sets the opacity of the object.
        /// </summary>
        /// <param name="a">The opacity value.</param>
        public void setOpacity(float a) {
            Material trianglesMaterial = GetComponent<MeshRenderer>().materials[0];
            Color modelColor = new Color(trianglesMaterial.color.r, trianglesMaterial.color.g, trianglesMaterial.color.b, a);

            if (trianglesMaterial != null) {
                trianglesMaterial.color = modelColor;
            }
        }

        /// <summary>
        /// Sets the color of the model.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        public void setColor(float r, float g, float b) {
            Material trianglesMaterial = GetComponent<MeshRenderer>().materials[0];
            Color modelColor = new Color(r, g, b, trianglesMaterial.color.a);

            if (trianglesMaterial != null) {
                trianglesMaterial.color = modelColor;
            }
        }

        /// <summary>
        /// Sets the color of the model, including opacity.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The opacity value.</param>
        public void setColor(float r, float g, float b, float a) {
            Material trianglesMaterial = GetComponent<MeshRenderer>().materials[0];
            Color modelColor = new Color(r, g, b, a);

            if (trianglesMaterial != null) {
                trianglesMaterial.color = modelColor;
            }
        }

        /// <summary>
        /// Sets the color of the model.
        /// </summary>
        /// <param name="color">Array of 3 floats, indicating the RGB values.</param>
        public void setColor(float[] color) {
            if (color == null || color.Length != 3) {
                return;
            }
            this.setColor(color[0], color[1], color[2]);
        }

        /// <summary>
        /// Sets the color of the model and opacity.
        /// </summary>
        /// <param name="color">Array of 3 floats, indicating the RGB values.</param>
        /// <param name="a">The opacity value.</param>
        public void setColor(float[] color, float a) {
            if (color == null || color.Length != 3) {
                return;
            }
            this.setColor(color[0], color[1], color[2], a);
        }
    }
}