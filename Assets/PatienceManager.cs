using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ConversationManager))]
public class PatienceManager : MonoBehaviour
{
    private ConversationManager conversationManager;

    public GameObject patiencePrefab;
    public int maxPatience = 10;
    public int intialPatience = 5;
    public int currentPatience = 5;
    
    private List<GameObject> iconsObjects = new List<GameObject>();
    private float iconsSpacing = 1f; // Adjust this value based on your layout

    void Start()
    {
        conversationManager = Utils.GetComponent<ConversationManager>(gameObject);

        InstantiatePatience();
    }

    void InstantiatePatience()
    {
        for (int i = 0; i < maxPatience; i++)
        {
            GameObject patienceBar = GameObject.Find("PatienceBar");
            if (patienceBar == null)
            {
                Debug.LogError($"Cannot find patience bar with name {patienceBar}");
            }
            Vector3 position = patienceBar.transform.position;

            GameObject patience = Instantiate(patiencePrefab, new Vector3(position.x + i * iconsSpacing, position.y, position.z), Quaternion.identity, patienceBar.transform);

            iconsObjects.Add(patience);
        }

        UpdatePatienceVisibility(intialPatience);
    }

    void UpdatePatienceVisibility(int patienceCount)
    {
        for (int i = 0; i < iconsObjects.Count; i++)
        {
            iconsObjects[i].SetActive(i < currentPatience);
        }
        if(currentPatience == 0) //TODO remove later
        {
           Application.Quit();
        }
    }

    // Call this method to update the patience based on the current state
    public void UpdatePatience(int variation)
    {
        currentPatience = Mathf.Clamp(currentPatience + variation, 0, maxPatience);
        UpdatePatienceVisibility(currentPatience);
    }
}
