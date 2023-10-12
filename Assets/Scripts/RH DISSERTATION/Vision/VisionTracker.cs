using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionTracker : MonoBehaviour
{

    [SerializeField] Camera _camera;
    [SerializeField] CellMapping _cellMapping;
    private int layer_mask;
    private int resolution;
    private void Awake()
    {
        layer_mask = LayerMask.GetMask("EnviromentNonInteractable");
    }

    private void FixedUpdate()
    {
        trackingupdate();
    }

    private void trackingupdate()
    {
        //Basic Edges
        //RayFromCamera(0, 0.3f);
        //RayFromCamera(0, -0.3f);
        //RayFromCamera(0.7f, 0);
        //RayFromCamera(-0.7f, 0);
        //Basic Edges
        /*
        for (float x = 0f; x <= 0.7; x += 0.1f)
        {
            for (float y = 0f; y <= 0.3; y += 0.1f)
            {
                SphereFromCamera(x, y);
                SphereFromCamera(x, -y);
                SphereFromCamera(-x, y);
                SphereFromCamera(-x, -y);
            }
        }
        */
        /*
        for (float x = 0; x < 0.1f; x += 0.01f)
        {
            for (float y = 0; y < 0.1f; y += 0.01f)
            {
                RayFromCamera(x, y);
                RayFromCamera(-x, y);
                RayFromCamera(x, -y);
                RayFromCamera(-x, -y);
            }
        }
        */

    }

    private void RayFromCamera(float xRayOffset, float yRayOffset)
    {
        RaycastHit hit;
        if (_camera != null)
        {
            if (Physics.Raycast(_camera.transform.position, _camera.transform.TransformDirection(new Vector3(xRayOffset, yRayOffset, 1)), out hit, 50, layer_mask))
            {
                Debug.DrawRay(_camera.transform.position, _camera.transform.TransformDirection(new Vector3(xRayOffset, yRayOffset, 1)) * hit.distance, Color.yellow);
            }
            else
            {
                Debug.DrawRay(_camera.transform.position, _camera.transform.TransformDirection(new Vector3(xRayOffset, yRayOffset, 1)) * 50, Color.yellow);
            }
        }
        else
        {
            Debug.LogError("No Camera In Vision Tracker");
        }
    }

    private void SphereFromCamera(float xRayOffset, float yRayOffset)
    {
        RaycastHit hit;
        if (_camera != null)
        {
            if (Physics.SphereCast(_camera.transform.position,1, _camera.transform.TransformDirection(new Vector3(xRayOffset, yRayOffset, 1)), out hit, 50, layer_mask))
            {
                Debug.DrawRay(_camera.transform.position, _camera.transform.TransformDirection(new Vector3(xRayOffset, yRayOffset, 1)) * hit.distance, Color.yellow);
            }
            else
            {
                Debug.DrawRay(_camera.transform.position, _camera.transform.TransformDirection(new Vector3(xRayOffset, yRayOffset, 1)) * 50, Color.yellow);
            }
        }
        else
        {
            Debug.LogError("No Camera In Vision Tracker");
        }
    }




}
