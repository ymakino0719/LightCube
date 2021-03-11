using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlanet : MonoBehaviour
{
    // 子オブジェクト
    GameObject child;
    // 回転角度（乱数）
    float angle;
    // 回転角度（固定化時）
    public float constAngle = 0.05f;
    // 子オブジェクトの回転速度
    float speedY;
    // 円の半径
    public float radius = 10.0f;
    // 回転方向
    public bool direction = true;
    // 位置のランダム化
    public bool randomPos = true;
    // 回転角度のランダム化
    public bool randomAngle = true;
    void Awake()
    {
        if(randomPos) transform.position = Random.onUnitSphere * radius;
        child = transform.GetChild(0).gameObject;
    }
    void Start()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position, transform.up);
        child.transform.rotation = Random.rotation;

        angle = (randomAngle) ? Random.Range(0.03f, 0.07f) : constAngle;
        if (!direction) angle *= -1;

        speedY = Random.Range(0.5f, 1.0f);
    }
    void FixedUpdate()
    {
        transform.RotateAround(Vector3.zero, transform.up, angle);
        child.transform.rotation *= Quaternion.Euler(0, speedY, 0);
    }
}
