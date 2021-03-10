using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlanet : MonoBehaviour
{
    // 子オブジェクト
    GameObject child;
    // 回転角度
    float angle;
    // 回転速度（乱数）
    float speedY;
    // 円の半径
    public float radius = 10.0f;
    // 回転方向
    public bool direction = true;
    void Awake()
    {
        transform.position = Random.onUnitSphere * radius;
        child = transform.GetChild(0).gameObject;
    }
    void Start()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position, transform.up);
        child.transform.rotation = Random.rotation;

        angle = Random.Range(0.03f, 0.07f);
        if (!direction) angle *= -1;

        speedY = Random.Range(0.5f, 1.0f);
    }
    void FixedUpdate()
    {
        transform.RotateAround(Vector3.zero, transform.up, angle);
        child.transform.rotation *= Quaternion.Euler(0, speedY, 0);
    }
}
