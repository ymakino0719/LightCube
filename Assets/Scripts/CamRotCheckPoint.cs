using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotCheckPoint : MonoBehaviour
{
    // Player
    GameObject player;
    // PlayerController
    PlayerController pC;

    // Start is called before the first frame update
    void Awake()
    {
        // Playerの取得
        player = GameObject.Find("Player");
        // PlayerControllerの取得
        pC = player.GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Playerの向きとチェックポイントの向きを確認し、正しい方角であるかを確認する
            bool trueDir = CheckPlayerAndCheckPointDirection();

            // 中間地点を経由していて、チェックポイントの向きと異なる場合、カメラを回転する（なお回転処理中である場合は無効とする）
            if (trueDir && pC.Halfway && !pC.ThroughGate) CheckThroughGate();
        }
    }
    bool CheckPlayerAndCheckPointDirection()
    {
        // Playerの上方向の向きとゲートの上方向の向きの角度を確認する
        float angle = Vector3.Angle(player.transform.up, transform.up);

        // 差が45度以下の場合、正しい方角と判断する
        bool tD = (angle <= 45) ? true : false;

        return tD;
    }
    void CheckThroughGate()
    {
        // 差が45度以下の場合（橋を通ったが元の面に帰ってきた場合）は、カメラの回転を実行しない
        // Playerの前方方向の向きとmidpointの前方方向の向きの角度を確認する（前方はx軸のためtransform.rightとなる）
        float angle = Vector3.Angle(player.transform.right, transform.right);
        if (angle > 45)
        {
            pC.RotRef = this.gameObject;
            pC.ThroughGate = true;
        }

        pC.Halfway = false;
    }
}
