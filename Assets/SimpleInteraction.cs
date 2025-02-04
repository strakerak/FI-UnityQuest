using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLMUnity;
using UnityEngine.UI;
using TMPro;

public class SimpleInteraction : MonoBehaviour
{

    public LLMCharacter llm;
    public InputField playerText;
    public Text AIText;
    // Start is called before the first frame update
    void Start()
    {
        playerText.onSubmit.AddListener(onInputFieldSubmit);
        playerText.Select();
    }


    void onInputFieldSubmit(string message)
    {
        playerText.interactable = false;
        AIText.text = "...";
        _ = llm.Chat(message, SetAIText, AIReplyComplete);
    }

    public void SetAIText(string text)
    {

        AIText.text = text; 
    }

    public void AIReplyComplete()
    {
        playerText.interactable = true;
        playerText.Select();
        playerText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
