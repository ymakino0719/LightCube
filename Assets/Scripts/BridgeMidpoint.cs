using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeMidpoint : MonoBehaviour
{
    // 浮島かどうかの設定
    public bool island = false;
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController pC = collision.gameObject.GetComponent<PlayerController>();
            pC.Midpoint = this.gameObject;
            // island情報を渡す（浮島側がゲートの出口の場合、回転を実行しないようにする）
            pC.Island = island;
        }
    }
}
