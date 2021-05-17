using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelBallBehavior : MonoBehaviour
{
    // PlayerのGameObject
    GameObject player;
    // BallのRigidbody
    Rigidbody rBody;
    // 仮想重力の係数
    float gravity = 6.0f;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        rBody = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        // 仮想重力をかけ続ける
        rBody.AddForce(-player.transform.up * gravity);
    }
}
