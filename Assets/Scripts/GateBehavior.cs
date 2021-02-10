using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateBehavior : MonoBehaviour
{
    // 一つ上の親オブジェクト
    GameObject parent;
    // 一つ下の子オブジェクト（方角ごとのHitBox）
    GameObject hB;
    // Player
    GameObject player;
    // PlayerController
    PlayerController pC;

    // ゲートドア
    BoxCollider[] door;
    // Start is called before the first frame update
    void Awake()
    {
        // 一つ上の親オブジェクトを取得する
        parent = transform.parent.gameObject;
        // 一つ下の子オブジェクト（方角ごとのHitBox）を取得する
        hB = transform.GetChild(0).gameObject;
        // 全てのゲートドアを取得
        door = GetComponents<BoxCollider>();
        // Playerの取得
        player = GameObject.Find("Player");
        // PlayerControllerの取得
        pC = player.GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Playerの向きとゲートの向きを確認し、Playerがゲートを通る正しい方角であるかを確認する
            bool trueGate = CheckPlayerAndGateDirection();

            // ゲートの処理
            ActivateProcess(trueGate);
        }
    }

    bool CheckPlayerAndGateDirection()
    {
        // Playerの上方向の向きとゲートの上方向の向きの角度を確認する
        float angle = Vector3.Angle(player.transform.up, transform.up);
        //Debug.Log("angle: " + angle);

        // 差が45度以下の場合、正しい方角と判断する
        bool tG = (angle <= 45) ? true : false;

        return tG;
    }

    void ActivateProcess(bool tG)
    {
        if (tG) // Playerがゲートを通る正しい方角である場合、その方角のHitBoxのみをアクティブにする
        {
            // 全ての方角のHitBoxを一度非アクティブにする
            DeactivateAllDirections();
            // この方角のHitBoxのみアクティブにする
            hB.SetActive(true);
            // 全てのゲートドアを開ける
            for (int i = 0; i < door.Length; i++) door[i].enabled = false;
        }
        else
        {
            // 全てのゲートドアを閉める（正しい方向でない場合、プレイヤーが侵入できないようにする）
            for (int i = 0; i < door.Length; i++) door[i].enabled = true;
        }
    }
    void DeactivateAllDirections()
    {
        // 親オブジェクトから見て全ての方角のHitBoxを一度非アクティブにする
        foreach (Transform c1Tra in parent.transform)
        {
            c1Tra.GetChild(0).gameObject.SetActive(false);
        }
    }
}
