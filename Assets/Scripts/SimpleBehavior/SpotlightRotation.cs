using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightRotation : MonoBehaviour
{
    GameObject rotParts02;

    // 初期回転量
    float initialY01;
    float initialZ02;

    // 回転域
    float range01 = 60.0f;
    float range02 = 20.0f;

    // 回転速度
    float rotSpeed01 = 0.1f;
    float rotSpeed02 = 0.2f;

    void Awake()
    {
        rotParts02 = transform.GetChild(0).gameObject;
        initialY01 = transform.eulerAngles.y;
        initialZ02 = transform.eulerAngles.z;
    }
    void FixedUpdate()
    {
        RotateParts01();
        RotateParts02();
    }
    void RotateParts01()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, initialY01 + Mathf.Sin(Time.time * rotSpeed01 * Mathf.PI) * range01, transform.eulerAngles.z);
    }
    void RotateParts02()
    {
        rotParts02.transform.eulerAngles = new Vector3(rotParts02.transform.eulerAngles.x, rotParts02.transform.eulerAngles.y, initialZ02 + Mathf.Sin(Time.time * rotSpeed02 * Mathf.PI) * range02);
    }
}
