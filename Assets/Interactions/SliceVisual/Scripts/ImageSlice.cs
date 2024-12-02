using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSlice
{
    public GameObject _myImage;

    public ImageSlice(GameObject slice_container, bool visible)
    {
        _myImage = GetNewSlice(slice_container.transform, visible);
    }

    public void SetImage(Texture2D texture)
    {
        _myImage.GetComponent<MeshRenderer>().material.mainTexture = texture;
    }

    public void TransformToObject(Matrix4x4 matrix)
    {

        _myImage.transform.localPosition = matrix.GetColumn(3);
        _myImage.transform.rotation =
            Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
        _myImage.transform.localScale = new Vector3(
                                       matrix.GetColumn(0).magnitude,
                                       matrix.GetColumn(1).magnitude,
                                       matrix.GetColumn(2).magnitude);

        _myImage.GetComponent<MeshFilter>().mesh.RecalculateBounds();
    }


    private GameObject GetNewSlice(Transform parent, bool visible)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.localPosition = Vector3.zero;
        quad.transform.localScale = Vector3.one;

        quad.AddComponent<MeshFilter>();
        quad.AddComponent<MeshCollider>();
        quad.AddComponent<MeshRenderer>();
        quad.transform.SetParent(parent);
        return quad;
    }
}
