using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateBehavior : MonoBehaviour
{
    // 一つ上の親オブジェクト
    GameObject parent;
    // 一つ下の子オブジェクト（方角ごとのHitBox）
    GameObject hB;
    // Start is called before the first frame update
    void Awake()
    {
        // 一つ上の親オブジェクトを取得する
        parent = transform.parent.gameObject;
        // 一つ下の子オブジェクト（方角ごとのHitBox）を取得する
        hB = transform.GetChild(0).gameObject;
        // HitBoxの当たり判定を非アクティブにしておく
        hB.SetActive(false);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // ４方角全てのHitBoxを一度非アクティブにする
            Deactivate4Directions();
            // この方角のHitBoxのみアクティブにする
            hB.SetActive(true);
        }
    }
    void Deactivate4Directions()
    {
        // 親オブジェクトから見て４方角全てのHitBoxを一度非アクティブにする
        foreach (Transform c1Tra in parent.transform)
        {
            c1Tra.GetChild(0).gameObject.SetActive(false);
        }
    }
}
