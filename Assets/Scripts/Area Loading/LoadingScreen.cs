using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [Header("Scene Loading")]
    [SerializeField] AreaConnector areaConnector;
    [SerializeField] float loadTime;
    private int levelLoadTo = AreaConnector.levelToLoad;

    [Header("Area name setup")]
    [SerializeField] TMP_Text displayText;

    private List<string> areaName = new List<string>();

    private void Start()
    {
        areaName.Add("Loading Main Menu");
        areaName.Add("Loading Screen Loop");
        areaName.Add("Entering Area 1...");
        areaName.Add("Entering Area 2...");
        try
        {
            displayText.text = areaName[levelLoadTo];
        }
        catch
        {
            displayText.text = "Error: No level Desc";
        }
        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        yield return new WaitForSeconds(loadTime);
        areaConnector.ActivateLevelLoad(levelLoadTo,true);
    }









}
