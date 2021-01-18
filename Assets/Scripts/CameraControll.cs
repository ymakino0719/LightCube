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
    // 回転時間: 遷移01
    float rotTime = 2.0f;
    // 移動時間: 遷移01
    float moveTime = 3.0f;
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

    // サテライトモード
    bool satellite = false;
    // サテライトモードの開幕移動判定
    bool openingMove = false;
    // カウンター
    int counter = 0;
    // サテライトモード用の追跡ロボット（位置（現在属する面または辺の位置）及び方向（原点位置を正面とする）を記録させる）
    GameObject robot;

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
        // 追跡用ロボットの取得
        robot = GameObject.Find("TrackingRobot");
    }

    // Update is called once per frame
    void Update()
    {
        if (!cJ.GameOver01 && !cJ.GameOver02)
        {
            if (satellite) SatelliteCamMovement();
            else ChasingCamera();
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

    void SatelliteCamMovement()
    {
        // 初期カメラ位置への移動
        if (!openingMove)
        {
            OpeningCamMovement();
        }

        // サテライトモードの終了条件
        bool endCondition = EndConditionOfSatelliteMode();
        if (endCondition) return;
    }
    void OpeningCamMovement()
    {
        // 初期位置へのカメラの移動
        GameObject gao = player.GetComponent<PlayerController>().CurrentFace;
        transform.position = gao.transform.GetChild(0).transform.position;

        // カメラの視点: 原点位置の方を向く
        Quaternion rotation = Quaternion.LookRotation(Vector3.zero - transform.position);
        transform.rotation = rotation;

        UpdateRobotInfo_First(gao.transform.position);

        openingMove = true;
    }

    void UpdateRobotInfo_First(Vector3 pos)
    {
        // ロボットの初期位置
        robot.transform.position = pos;
        // ロボットの初期方向
        Quaternion rotation = Quaternion.LookRotation(Vector3.zero - robot.transform.position, player.transform.up);
        robot.transform.rotation = rotation;
    }
    bool EndConditionOfSatelliteMode()
    {
        bool eC = false;
        // サテライトモード時に対応するボタンをもう一度押された場合、サテライトモードを終了する
        bool satelliteCam = Input.GetButtonDown("SatelliteCam");
        // 二重に押される処理になるらしいので、counterを追加した。counterが２になった場合終了処理を行う
        if (satelliteCam) counter++;
        if(counter == 2)
        {
            counter = 0;

            ChasingCamera(); // 追尾カメラに戻す
            player.GetComponent<PlayerController>().Control = true;
            openingMove = false;
            satellite = false;
            eC = true;
        }

        return eC;
    }
    void ChasingCamera()
    {
        // カメラの移動
        transform.position = camPos.transform.position;
        // カメラの視点: playerの方を向く（playerの頭が上になるように）
        Quaternion rotation = Quaternion.LookRotation(player.transform.position - camPos.transform.position, player.transform.up);
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
    public bool Satellite
    {
        set { satellite = value; }
        get { return satellite; }
    }
}
