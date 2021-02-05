using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleStar : MonoBehaviour
{
    Rigidbody rBody;

    // 移動乱数の下限値
    float moveRandMin = -20.0f;
    // 移動乱数の上限値
    float moveRandMax = 20.0f;

    // 回転乱数の下限値
    float rotRandMin = -2.0f;
    // 回転乱数の上限値
    float rotRandMax = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();

        ChangeVelocity();
        ChangeAngularVelocity();
    }
    void OnCollisionEnter(Collision other)
    {
        ChangeVelocity();
        ChangeAngularVelocity();
    }
    void ChangeVelocity()
    {
        rBody.velocity = new Vector3(Random.Range(moveRandMin, moveRandMax), Random.Range(moveRandMin, moveRandMax), Random.Range(moveRandMin, moveRandMax));
    }

    void ChangeAngularVelocity()
    {
        rBody.angularVelocity = new Vector3(Random.Range(rotRandMin, rotRandMax), Random.Range(rotRandMin, rotRandMax), Random.Range(rotRandMin, rotRandMax));
    }
}
