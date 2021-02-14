using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreWriter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string jsonScoreData = System.IO.File.ReadAllText(Application.persistentDataPath + "/FinalScoreData.json");
        SaveData.ScoreData finalGameData = JsonUtility.FromJson<SaveData.ScoreData>(jsonScoreData);
        TMPro.TextMeshProUGUI[] textBoxes = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        textBoxes[1].text = "- Time Elapsed: " + finalGameData.finalTimeElapsed + "s";
        textBoxes[2].text = "- Shots taken: " + finalGameData.finalShotsTaken;
        textBoxes[3].text = "- Final Score: " + finalGameData.finalScore;
    }
}
