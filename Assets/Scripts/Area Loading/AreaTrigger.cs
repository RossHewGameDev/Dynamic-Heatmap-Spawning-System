using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaTrigger : MonoBehaviour
{
    [SerializeField] AreaConnector areaConnector;
    [SerializeField] int sceneToLoad;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("False");
            areaConnector.ActivateLevelLoad(sceneToLoad,false);
        }
    }

}
