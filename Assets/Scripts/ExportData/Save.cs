using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Save : MonoBehaviour
{

    [SerializeField] string map;
    public int currentScore = 0;

    public string currentParticipant; // Participants will enter their participant code here

    [HideInInspector] 
    public float currentExploration; // This will be hidden from participants 





    [Header("Required Scrpits")]
    [SerializeField] Interaction interaction;
    [SerializeField] CellMapping cellMapping;
    [SerializeField] SpawningSystem spawningSystem;


    private void Update()
    {
        currentScore = interaction.hiveStored;

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Saved!");
            currentExploration = cellMapping.explorationCheck();
            SaveFile();
        }
    }

    public void SaveFile()
    {
        GameData data = new GameData(currentScore, currentParticipant, currentExploration,spawningSystem.systemTypeRunning.ToString(), map);

        string saveData = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/save " + currentParticipant + data.systemTypeRun + data.mapTypeRun + ".json", saveData);

        string csvFormatting = string.Format("{0},{1},{2},{3},{4}\n", currentParticipant, data.systemTypeRun, data.mapTypeRun, data.score, data.explorationPercentage);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/save " + currentParticipant + data.systemTypeRun + data.mapTypeRun + ".csv", csvFormatting);

        // C:\Users\RH246018\AppData\LocalLow\DefaultCompany\RH246018-Dissertation <-- Default location 


        /*
        string destination = Application.persistentDataPath + "/save" + currentParticipant + ".dat";
        Debug.Log(destination);
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData data = new GameData(currentScore, currentParticipant, currentExploration);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
        */
    }
}
