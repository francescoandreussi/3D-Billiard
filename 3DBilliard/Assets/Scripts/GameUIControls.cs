using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIControls : MonoBehaviour
{
    public void Replay()
    {
        GameLogic.replay = true;
    }

    public void BackToMenu()
    {
        SaveData.SaveToJSON(GameLogic.score, GameLogic.shots, (int)GameLogic.gameTime);

        SceneManager.LoadScene("MenuScene");
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("GameScene");
        GameLogic.score = 0;
        GameLogic.shots = 0;
        GameLogic.gameTime = 0;
    }
}
