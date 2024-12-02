using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour

{
    public Text fpsText;
    public float deltaTime;

    // Start is called before the first frame update
    void Start()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
