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
    public float range01 = 60.0f;
    public float range02 = 20.0f;

    // 回転速度
    float rotSpeed01 = 0.1f;
    float rotSpeed02 = 0.175f;

    // 乱数値（開始タイミングをずらす）
    float rand;

    void Awake()
    {
        rotParts02 = transform.GetChild(0).gameObject;
        initialY01 = transform.localEulerAngles.y;
        initialZ02 = rotParts02.transform.localEulerAngles.z;
    }
    void Start()
    {
        rand = Random.Range(0.0f, 10.0f);
    }
    void FixedUpdate()
    {
        RotateParts01();
        RotateParts02();
    }
    void RotateParts01()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, initialY01 + Mathf.Sin((rand + Time.time) * rotSpeed01 * Mathf.PI) * range01, transform.localEulerAngles.z);
    }
    void RotateParts02()
    {
        rotParts02.transform.localEulerAngles = new Vector3(rotParts02.transform.localEulerAngles.x, rotParts02.transform.localEulerAngles.y, initialZ02 + Mathf.Sin((rand + Time.time) * rotSpeed02 * Mathf.PI) * range02);
    }
}
