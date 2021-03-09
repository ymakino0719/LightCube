using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlanet : MonoBehaviour
{
    // 子オブジェクト
    GameObject child;
    // 回転角度
    float angle = 0.05f;
    // 回転速度（乱数）
    float speedY;
    void Awake()
    {
        child = transform.GetChild(0).gameObject;
    }
    void Start()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position, transform.up);
        child.transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        speedY = Random.Range(0.5f, 1.0f);
    }
    void FixedUpdate()
    {
        transform.RotateAround(Vector3.zero, transform.up, angle);
        child.transform.rotation *= Quaternion.Euler(0, speedY, 0);
    }
}
