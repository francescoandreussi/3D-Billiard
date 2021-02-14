using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    [SerializeField] private static ScoreData _FinalScoreData = new ScoreData();

    [System.Serializable]
    public class ScoreData
    {
        public int finalTimeElapsed;
        public int finalShotsTaken;
        public int finalScore;
    }

    public static void SaveToJSON(int score, int shots, int gameTime)
    {
        _FinalScoreData.finalScore = score;
        _FinalScoreData.finalShotsTaken = shots;
        _FinalScoreData.finalTimeElapsed = gameTime;
        string finalScore = JsonUtility.ToJson(_FinalScoreData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/FinalScoreData.json", finalScore);
    }
}
