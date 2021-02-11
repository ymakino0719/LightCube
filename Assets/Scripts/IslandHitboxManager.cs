using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandHitboxManager : MonoBehaviour
{
    // 一つ上の親オブジェクト
    GameObject parent;
    // 一つ下の子オブジェクト（方角ごとのHitBox）
    GameObject hB;

    void Awake()
    {
        // 一つ上の親オブジェクトを取得する
        parent = transform.parent.gameObject;
        // 一つ下の子オブジェクト（方角ごとのHitBox）を取得する
        hB = transform.GetChild(0).gameObject;
        // 開幕は子オブジェクトを非アクティブ化しておく
        hB.SetActive(false);
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Playerの向きとHalfwayPointの向きを確認し、正しい方角であるかを確認する
            float angle = Vector3.Angle(collision.gameObject.transform.up, transform.up);

            // 正しい方角である場合、以下の処理を実行する
            if (angle <= 45)
            {
                // 一度Islandの全ての方角ごとのHitBoxを非アクティブ化させる
                DeactivateAllDirections();

                // このスクリプトがアタッチされているオブジェクトの子オブジェクトのHitBoxのみ有効にする
                hB.SetActive(true);
            }
        }
    }
    void DeactivateAllDirections()
    {
        // 親オブジェクトから見て二階層下の、全ての方角のHitBoxを一度非アクティブにする
        foreach (Transform c1Tra in parent.transform)
        {
            c1Tra.GetChild(0).gameObject.SetActive(false);
        }
    }
}
