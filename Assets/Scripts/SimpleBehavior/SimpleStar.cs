using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleStar : MonoBehaviour
{
    Quaternion rot;
    // Start is called before the first frame update
    void Start()
    {
        rot = Quaternion.AngleAxis(0.3f, transform.forward);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion q = this.transform.rotation;
        this.transform.rotation = q * rot;
    }
}
