using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    // PlayerのGameObject
    GameObject player;
    // CamPosのGameObject
    GameObject camPos;
    // ClearJudgement
    ClearJudgement cJ;

    // ClearLight
    GameObject clearLight;
    // 回転時間
    float rotTime = 2.0f;
    // 移動時間
    float moveTime = 2.0f;
    // ターゲットとの距離: 遷移01
    float targetDis01 = 7.0f;
    // ターゲットとの距離: 遷移02
    float targetDis02 = 4.0f;
    // 接近停止閾値
    float minDis = 0.01f;
    // 開幕処理
    bool beginning = true;
    //　現在の移動の速度
    Vector3 moveVelocity;
    //　現在の移動の速度（回転）
    Vector3 rotVelocity;
    // ターゲット位置
    Vector3 targetPos;
    // ターゲットへの角度
    Vector3 targetAngle;
    

    void Awake()
    {
        // playerの取得
        player = GameObject.Find("Player");
        // camPosの取得
        camPos = GameObject.Find("CamPos");
        // ClearLightの取得
        clearLight = GameObject.Find("ClearLight");
        // ClearJudgementの取得
        cJ = GameObject.Find("GameDirector").GetComponent<ClearJudgement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!cJ.GameOver01 && !cJ.GameOver02)
        {
            ChasingCamera();
        }
    }

    void FixedUpdate()
    {
        if (cJ.GameOver01)
        {
            if (beginning) StartingOperation01();

            // ゆっくりStarLightの方を見る
            TurnAroundToStarLight();
            // ゆっくりStarLightにカメラを近づける
            MoveCloserToStarLight();
        }

        if (cJ.GameOver02)
        {
            // StarLightが背景になるようにカメラ移動する
            Move_StarLightBecomesBackground();
        }
    }

    void ChasingCamera()
    {
        // カメラの移動
        transform.position = camPos.transform.position;
        // カメラの視点: playerの方を向く（playerの頭が上になるように）
        Quaternion rotation = Quaternion.LookRotation(player.transform.position - camPos.transform.position, camPos.transform.up);
        transform.rotation = rotation;
    }

    void StartingOperation01()
    {
        // 移動先のtargetPosの取得
        Vector3 vec = (transform.position - clearLight.transform.position).normalized * targetDis01;
        targetPos = clearLight.transform.position + vec;

        // 最初の回転を記録
        Quaternion startRot = transform.rotation;
        // ターゲットの方に向かせてみる
        transform.rotation = Quaternion.LookRotation(targetPos - transform.position, transform.up);
        // ターゲットへの角度を取得
        targetAngle = transform.eulerAngles;
        // 最初の回転に戻す
        transform.rotation = startRot;

        beginning = false;
    }

    void TurnAroundToStarLight()
    {
        //　カメラの角度をスムーズに動かす
        float xRotate = Mathf.SmoothDampAngle(transform.eulerAngles.x, targetAngle.x, ref rotVelocity.x, rotTime);
        float yRotate = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle.y, ref rotVelocity.y, rotTime);
        float zRotate = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle.z, ref rotVelocity.z, rotTime);
        transform.eulerAngles = new Vector3(xRotate, yRotate, zRotate);
    }

    void MoveCloserToStarLight()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref moveVelocity, moveTime);

        // 十分距離近づいたとき、カメラを近づける処理を終了させる（併せて回転処理も終了となる）
        float checkDis = (targetPos - transform.position).sqrMagnitude;
        if(checkDis <= minDis)
        {
            beginning = false;
            cJ.GameOver01 = false;
            cJ.GameOver02 = true;
        }
    }
    void Move_StarLightBecomesBackground()
    {
        // 移動先のtargetPosの取得及び移動
        Vector3 vec = player.transform.position - clearLight.transform.position;
        Vector3 targetPos = player.transform.position + vec.normalized * targetDis02 + player.transform.up * 2.0f;
        transform.position = targetPos;

        // カメラをターゲットの方に向かせる
        transform.rotation = Quaternion.LookRotation(clearLight.transform.position - transform.position, player.transform.up);
    }
}
