using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberRotation : MonoBehaviour
{
    Transform cameraTransform;

    void Start()
    {
        cameraTransform = GameObject.Find("Main Camera").transform;
    }

    void Update()
    {
        Vector3 lookAwayPosition = transform.position * 2 - cameraTransform.position;
        transform.LookAt(lookAwayPosition);
    }
}
