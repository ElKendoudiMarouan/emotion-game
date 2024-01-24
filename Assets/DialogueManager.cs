using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueManager : MonoBehaviour
{
    public CharacterDialogueData characterDialogueData;
    void Start()
    {
        string jsonContent = Resources.Load<TextAsset>("Dialogues/policeman").text;//TODO enhance this
        characterDialogueData = JsonConvert.DeserializeObject<CharacterDialogueData>(jsonContent);
    }


    public DialogueLineData GetRandomDialogueForEotion(EmotionType emotion)
    {
        DialogueLinesByEmotionData lines = characterDialogueData.dialogueLinesByEmotion.Find(cdt => emotion == cdt.emotion);

        return lines?.dialogueLines.RandomElement();
    }
}

