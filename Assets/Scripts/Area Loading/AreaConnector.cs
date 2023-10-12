using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AreaConnector : MonoBehaviour
{
    public Animator animator;

    public static int levelToLoad;

    private int loadingScreenIndex = 1;
    private bool fromLoading;   
    public void ActivateLevelLoad(int LevelLoad, bool isFromLoading)
    {
        fromLoading = isFromLoading;
        FadeToArea(LevelLoad);
    }

    public void FadeToArea (int levelIndex)
    {
        levelToLoad = levelIndex;
        
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        if (fromLoading)
        {
            Debug.Log("from loading screen");
            Debug.Log(levelToLoad);
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            Debug.Log("Loading from Level");
            SceneManager.LoadScene(loadingScreenIndex);
        }
    }

}
