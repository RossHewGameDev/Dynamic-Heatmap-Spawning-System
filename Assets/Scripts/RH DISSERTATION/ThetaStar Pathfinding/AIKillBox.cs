using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A simple script that deletes the AI when a player gets too close to it (capturing it)
/// </summary>
public class AIKillBox : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }

}
