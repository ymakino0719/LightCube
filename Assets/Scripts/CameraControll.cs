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
    // FaceInformation
    FaceInformation face;
    // EdgeInformation
    EdgeInformation edge;

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
    // 追跡ロボットが現在属する面または辺のGameObject
    GameObject currentGao;
    // 追跡ロボットが次に移動する面または辺のGameObject
    GameObject nextGao;
    // サテライトモードでカメラが回転中かどうか
    bool rolling = false;
    // 縦横の操作入力受付
    bool vertical = false;
    bool horizontal = false;
    // ロボットの移動距離（※大きすぎないように！）
    float moveDis = 5.0f;
    // サテライトカメラを移動させるときの時間（0～1）
    private float time = 0;
    // サテライトカメラを移動させるときの時間の調整パラメータ
    private float timeCoef = 2.0f;

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
            if (satellite) SatelliteCamera();
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

    void SatelliteCamera()
    {
        // 初期カメラ位置への移動
        if (!openingMove)
        {
            OpeningSatelliteCamMovement();
        }

        if (!rolling) SatelliteCamMovement_Input();
        else SatelliteCamMovement_Rotate();

        // サテライトモードの終了条件
        bool endCondition = EndConditionOfSatelliteMode();
        if (endCondition) return;
    }
    void OpeningSatelliteCamMovement()
    {
        // 初期位置へのカメラの移動
        currentGao = player.GetComponent<PlayerController>().CurrentFace;
        transform.position = currentGao.transform.GetChild(0).transform.position;

        // カメラの視点: 原点位置の方を向く（上方向はプレイヤーと合わせる）
        Quaternion rotation = Quaternion.LookRotation(Vector3.zero - transform.position, player.transform.up);
        transform.rotation = rotation;

        UpdateRobotInfo_First();

        openingMove = true;
    }

    void UpdateRobotInfo_First()
    {
        // ロボットの初期位置
        robot.transform.position = currentGao.transform.position;
        // ロボットの初期方向（上方向はプレイヤーと合わせる）
        Quaternion rotation = Quaternion.LookRotation(Vector3.zero - robot.transform.position, player.transform.up);
        robot.transform.rotation = rotation;
    }
    void SatelliteCamMovement_Input()
    {
        // 操作入力
        // vertical 方向に奇数回回転している状態では horizontal 方向の入力を受け付けない
        float hor = (vertical) ? 0 : Input.GetAxis("Horizontal");
        int horA = ZeroOne(hor);

        // horizontal 方向に奇数回回転している状態では vertical 方向の入力を受け付けない
        float ver = (horizontal) ? 0 : Input.GetAxis("Vertical");
        int verA = ZeroOne(ver);

        if (Mathf.Abs(hor) > float.Epsilon)
        {
            // robotによる探索（水平移動）
            robot.transform.position += transform.right * moveDis * horA;

            // 最短距離探索
            SearchShortestDistance();

            // ロボットの位置を元に戻す
            robot.transform.position -= transform.right * moveDis * horA;

            // ロボットの移動
            UpdateRobotInfo();

            // horizontal の入力を切り替える
            horizontal = !horizontal;

            // 回転処理に移行する
            rolling = true;
        }
        else if (Mathf.Abs(ver) > float.Epsilon)
        {
            // robotによる探索（上下移動）
            robot.transform.position += transform.up * moveDis * verA;

            // 最短距離探索
            SearchShortestDistance();

            // ロボットの位置を元に戻す
            robot.transform.position -= transform.up * moveDis * verA;

            // ロボットの移動
            UpdateRobotInfo();

            // vertical の入力を切り替える
            vertical = !vertical;

            // 回転処理に移行する
            rolling = true;
        }
    }

    int ZeroOne(float num)
    {
        int a = 0;
        if (num > float.Epsilon) a = 1;
        else if (num < float.Epsilon) a = -1;

        return a;
    }

    void SearchShortestDistance()
    {
        // 直前までカメラが面していたのは面か辺か
        if (!vertical && !horizontal) // 面の時
        {
            face = currentGao.GetComponent<FaceInformation>();
            float? dis = null;

            for (int i = 0; i < 4; i++)
            {
                // robotが探索移動した後の座標と最も距離が近い辺（のGameObject）を割り出す
                float tempDis = (face.edge[i].transform.position - robot.transform.position).sqrMagnitude;
                if (dis == null || tempDis < dis)
                {
                    dis = tempDis;
                    nextGao = face.edge[i];
                }
            }
        }
        else // 辺の時
        {
            edge = currentGao.GetComponent<EdgeInformation>();
            float? dis = null;

            for (int i = 0; i < 2; i++)
            {
                // robotが探索移動した後の座標と最も距離が近い面（のGameObject）を割り出す
                float tempDis = (edge.face[i].transform.position - robot.transform.position).sqrMagnitude;
                if (dis == null || tempDis < dis)
                {
                    dis = tempDis;
                    nextGao = edge.face[i];
                }
            }
        }
    }
    void UpdateRobotInfo()
    {
        // 移動先までの角度差を計算する
        float angle = Vector3.Angle(currentGao.transform.position - Vector3.zero, nextGao.transform.position - Vector3.zero);
        // 回転軸を割り出す
        // currentGaoとnextGaoのどちらかは必ず辺のオブジェクトで、回転軸を持つ
        edge = (currentGao.GetComponent<EdgeInformation>() != null) ? currentGao.GetComponent<EdgeInformation>() : nextGao.GetComponent<EdgeInformation>();
        Vector3 axis = edge.vertex[0].transform.position - edge.vertex[1].transform.position; // 回転軸

        // angle度でrobotのRotateAroundを行う
        robot.transform.RotateAround(Vector3.zero, axis, angle);
        // angle度で回転後の位置及び角度を記録する
        Vector3 nextPos01 = robot.transform.position;
        Vector3 nextAngle01 = robot.transform.eulerAngles;

        // -2 * angle度でrobotのRotateAroundを行う（逆方向に回転させる）
        robot.transform.RotateAround(Vector3.zero, axis, -2 * angle);
        // -angle度で回転後の位置及び角度を記録する
        Vector3 nextPos02 = robot.transform.position;
        Vector3 nextAngle02 = robot.transform.eulerAngles;

        // angle度でrobotのRotateAroundを行う（初期位置に戻す）
        robot.transform.RotateAround(Vector3.zero, axis, angle);

        // 上で調べたnextPosとnextGaoが原点と織りなす角度の差を求め、より小さい方を正しい回転方向として採用する
        float angle01 = Vector3.Angle(nextGao.transform.position - Vector3.zero, nextPos01 - Vector3.zero);
        float angle02 = Vector3.Angle(nextGao.transform.position - Vector3.zero, nextPos02 - Vector3.zero);
        Vector3 nextAngle = (angle01 < angle02) ? nextAngle01 : nextAngle02;

        // ロボットの位置
        robot.transform.position = nextGao.transform.position;
        // ロボットの方向
        robot.transform.eulerAngles = nextAngle;
    }
    void SatelliteCamMovement_Rotate()
    {
        // timeに時間を加算する
        time += timeCoef * Time.deltaTime;

        Vector3 currentCamPos = currentGao.transform.GetChild(0).transform.position;
        Vector3 nextCamPos = nextGao.transform.GetChild(0).transform.position;

        // サテライトカメラをゆっくり移動させる
        transform.position = Vector3.Slerp(currentCamPos, nextCamPos, time);

        // サテライトカメラの方向は常に原点位置を向くようにする（上方向はロボットの上方向に合わせる）
        Quaternion rotation = Quaternion.LookRotation(Vector3.zero - transform.position, robot.transform.up);
        transform.rotation = rotation;

        if (time >= 1)
        {
            transform.position = nextCamPos;
            currentGao = nextGao;
            time = 0;
            rolling = false;
        }
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
