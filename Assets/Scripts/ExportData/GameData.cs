using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int score;
    public string participantNumber;
    public float explorationPercentage;
    public string systemTypeRun;
    public string mapTypeRun;

    /// <summary>
    /// holds all data to be saved
    /// </summary>
    /// <param name="scoreInt"></param>
    /// <param name="participantStr"></param>
    /// <param name="explorationFlt"></param>
    /// <param name="systemTypeStr"></param>
    /// <param name="mapTypeStr"></param>
    public GameData(int scoreInt, string participantStr, float explorationFlt, string systemTypeStr, string mapTypeStr)
    {
        score = scoreInt;
        participantNumber = participantStr;
        explorationPercentage = explorationFlt;
        systemTypeRun = systemTypeStr;
        mapTypeRun = mapTypeStr;

    }
}
