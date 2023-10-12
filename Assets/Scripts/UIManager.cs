using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] Interaction interaction;

    [SerializeField] TMP_Text gatheredPlants;
    [SerializeField] TMP_Text hivedPlants;

    // Update is called once per frame
    void Update()
    {
        gatheredPlants.text = interaction.plantsCollected.ToString();
        hivedPlants.text = interaction.hiveStored.ToString();
    }
}
