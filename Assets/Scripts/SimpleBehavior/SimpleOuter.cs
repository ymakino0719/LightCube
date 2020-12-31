﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOuter : MonoBehaviour
{
    Quaternion rot;
    // Start is called before the first frame update
    void Start()
    {
        rot = Quaternion.AngleAxis(0.01f, transform.up);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion q = this.transform.rotation;
        this.transform.rotation = q * rot;
    }
}