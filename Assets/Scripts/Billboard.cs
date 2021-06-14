using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform canvas;

    // Update is called once per frame
    void Update()
    {
        canvas.forward = Camera.main.transform.forward;

    }
}
