using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleStar : MonoBehaviour
{
    Rigidbody rBody;

    // 乱数の下限値
    float randMin = -10.0f;
    // 乱数の上限値
    float randMax = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        rBody.velocity = new Vector3(Random.Range(randMin, randMax), Random.Range(randMin, randMax), Random.Range(randMin, randMax));
    }
}
