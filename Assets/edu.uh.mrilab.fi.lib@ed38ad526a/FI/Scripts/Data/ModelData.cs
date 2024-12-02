using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fi {
    public class ModelData {
        /// <summary>
        /// The name of the data.
        /// </summary>
        public string DataName { get; private set; }

        /// <summary>
        /// The mesh itself.
        /// </summary>
        public Mesh Mesh { get; private set; }

        /// <summary>
        /// The list of textures attached to this model data.
        /// </summary>
        public List<Texture2D> Textures { get; private set; } = new List<Texture2D>();

        /// <summary>
        /// The texture coordinates indicies.
        /// </summary>
        public Vector2[] TextureCoordinates { get; private set; }

        /// <summary>
        /// Constructs a ModelData.
        /// </summary>
        /// <param name="dataResponse">The data message containing model data and metadata.</param>
        public ModelData(DataResponse dataResponse) {
            this.setMeshData(dataResponse);
        }

        /// <summary>
        /// Constructs the model data from the data response.
        /// </summary>
        /// <param name="dataResponse">The data message containing model data and metadata.</param>
        public void setMeshData(DataResponse dataResponse) {
            DataName = dataResponse.DataID;
            
            int dataIndex = 0;
            float[] points = new float[dataResponse.PayloadPointsLength / sizeof(float)];
            System.Buffer.BlockCopy(dataResponse.Payload, dataIndex, points, 0, dataResponse.PayloadPointsLength);
            dataIndex += dataResponse.PayloadPointsLength;

            int[] triangleIndices = new int[dataResponse.PayloadTrianglesLength / sizeof(int)];
            System.Buffer.BlockCopy(dataResponse.Payload, dataIndex, triangleIndices, 0, dataResponse.PayloadTrianglesLength);
            dataIndex += dataResponse.PayloadTrianglesLength;

            float[] textureCoordinates = null;
            if (dataResponse.PayloadTextureCoordinatesLength > 0) {
                textureCoordinates = new float[dataResponse.PayloadTextureCoordinatesLength / sizeof(float)];
                System.Buffer.BlockCopy(dataResponse.Payload, dataIndex, textureCoordinates, 0, dataResponse.PayloadTextureCoordinatesLength);
                dataIndex += dataResponse.PayloadTextureCoordinatesLength;
            }

            List<float[]> textures = new List<float[]>();
            if (dataResponse.Textures.Length != 0) {
                for (int i = 0; i < dataResponse.Textures.Length; i++) {
                    TextureMetadata textureMetadata = dataResponse.Textures[i];
                    if (textureMetadata.PayloadLength > 0) {
                        float[] texture = new float[textureMetadata.PayloadLength / sizeof(float)];
                        System.Buffer.BlockCopy(dataResponse.Payload, dataIndex, texture, 0, textureMetadata.PayloadLength);
                        dataIndex += textureMetadata.PayloadLength;
                        textures.Add(texture);
                    }
                }
            }


            this.setMeshData(points, triangleIndices, textureCoordinates, textures, dataResponse.Textures);
        }

        /// <summary>
        /// Changes the Mesh in this object.
        /// </summary>
        /// <param name="points">The vertices of the mesh.</param>
        /// <param name="triangleIndices">The triangle indices that make up the mesh.</param>
        public void setMeshData(float[] points, int[] triangleIndices, float[] textureCoordinates, List<float[]> textures, TextureMetadata[] texturesMetadata) {
            //TODO: For now, only triangular mesh works.
            if (points == null || triangleIndices == null) {
                Debug.Log("Failed to set model data. Given data is null");
                return;
            } else if (points.Length % 3 != 0) {
                Debug.Log("Failed to set model data. Point information is incorrect");
                return;
            } else if (triangleIndices.Length % 3 != 0) {
                Debug.Log("Failed to set model data. Triangle index information is incorrect");
                return;
            }

            Vector3[] vertices = new Vector3[points.Length / 3];
            for (int i = 0; i < vertices.Length; i++) {
                int pointIndex = i * 3;
                Vector3 point = new Vector3(
                    points[pointIndex++] * -1,     // Times -1 because x-axes is backwards from server   
                    points[pointIndex++],
                    points[pointIndex]
                );
                vertices[i] = point;
            }

            Mesh = new Mesh();
            Mesh.vertices = vertices;
            Mesh.triangles = triangleIndices;
            Mesh.RecalculateBounds();
            Mesh.RecalculateNormals();

            // Create the texture coordinates.
            TextureCoordinates = new Vector2[textureCoordinates.Length / 2];
            for (int i = 0; i < TextureCoordinates.Length; i++) {
                int index = i * 2;
                TextureCoordinates[i] = new Vector2(textureCoordinates[index], textureCoordinates[index + 1]);
            }

            // Create the textures.
            for (int t = 0; t < textures.Count; t++) {
                float[] textureData = textures[t];
                Color[] pixels = new Color[textureData.Length / 3];
                for (int i = 0; i < pixels.Length; i++) {
                    int index = i * 3;
                    pixels[i] = new Color(textureData[index], textureData[index + 1], textureData[index + 2]);
                }
                Texture2D texture = new Texture2D(texturesMetadata[t].DimensionU, texturesMetadata[t].DimensionV, TextureFormat.RGBAFloat, false);
                texture.name = texturesMetadata[t].TextureName;
                texture.SetPixels(pixels);
                texture.Apply();
                Textures.Add(texture);
            }

            Debug.Log(string.Format("Data set for Model={0}. Points={1}, Triangles={2}",
                DataName, vertices.Length, triangleIndices.Length));
        }
    }
}