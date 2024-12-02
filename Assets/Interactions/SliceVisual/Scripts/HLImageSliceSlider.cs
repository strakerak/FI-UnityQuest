using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HLImageSliceSlider : fi.Interaction
{
    public GameObject slice_container_prefab; //NearInteractionGrabbable component
    public Slider ps_slider_x;
    public Slider ps_slider_y;
    public Slider ps_slider_z;

    private int[] dimensions;

    /*
     * GameObject sliceTransformation = should have the center location of the slice 
     * ImageVisual slice = has all infomration to set parameters for the slider 
     */

    void Awake()
    {
        //ps_slider_x.onValueChanged.AddListener(this.onValueChanged);
        //ps_slider_y.onValueChanged.AddListener(this.onValueChanged);
        //ps_slider_z.onValueChanged.AddListener(this.onValueChanged);
        dimensions = new int[3];
    }


    public void AddSlice(GameObject sliceTransformation, fi.ImageVisual imageSlice)
    {
        int index = (int)imageSlice.SliceOrientation;
        GameObject newContainer = Instantiate(slice_container_prefab);
        SetSliderParameters(imageSlice.Spacing[2] * imageSlice.Dimensions[2]);

        ImageSlice image = new ImageSlice(newContainer, true);
        image.TransformToObject(sliceTransformation.transform.localToWorldMatrix);
        newContainer.transform.SetParent(ps_slider_y.transform);





        //sliceTransformation.transform.SetParent(newContainer.transform);


    }

    public void onValueUpdated(PointerEventData value)
    {/*
        int newValue = (int)SliceUtility.map(value.NewValue, 0, 1, 0, 25); //25 is dimension 
        int oldValue = (int)SliceUtility.map(value.OldValue, 0, 1, 0, 25); //25 is dimension 

        if (oldValue == newValue)
            return;

        string interactionID = value.slider.ToString();
        ServerConnection.sendMessage(RequestMaker.makeModuleInteractionRequest(ModuleID, value.Slider.ToString(), newValue));*/
    }
    private void SetSliderParameters(float SlideEndDistance)
    {
        ps_slider_y.minValue = 0f;
        ps_slider_y.maxValue = SlideEndDistance;
    }
}
