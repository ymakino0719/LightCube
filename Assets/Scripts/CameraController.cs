using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Camera
    Camera cam;
    // PlayerのGameObject
    GameObject player;
    // PlayerController
    PlayerController pC;
    // Playerのスキン
    SkinnedMeshRenderer pSkin;
    // FacePos
    GameObject facePos;
    // CamPosのGameObject
    GameObject camPos;
    // ClearJudgement
    ClearJudgement cJ;
    // FaceInformation
    FaceInformation face;
    // EdgeInformation
    EdgeInformation edge;
    // InsideColorBoxのMeshRenderer
    MeshRenderer insideColorBox;
    // StageUI
    StageUI sUI;
    // SFXPlayer
    SFXPlayer sfx_UI;

    // ClearLight
    GameObject clearLight;
    // 回転時間: 遷移01
    float rotTime = 2.0f;
    // 移動時間: 遷移01
    float moveTime = 3.0f;
    // ターゲットとの距離: 遷移01
    public float targetDis01 = 7.0f;
    // ターゲットとの距離: 遷移02（奥行）
    public float targetDis02_Depth = 4.0f;
    // ターゲットとの距離: 遷移02（高さ）
    public float targetDis02_Height = 2.0f;
    // 接近停止閾値
    float minDis = 0.01f;
    //　現在の移動の速度
    Vector3 moveVelocity;
    //　現在の移動の速度（回転）
    Vector3 rotVelocity;
    // ターゲット位置
    Vector3 targetPos;
    // ターゲットへの角度
    Vector3 targetAngle;

    // カメラのズーム倍率の初期値
    float initialFieldOfView = 60.0f;
    // 開幕処理
    bool openingSequence = true;
    // スクロールの感度
    float scrollSensitivity = 30.0f;
    // カメラ操作入力受付が可能かどうか
    bool camControl = true;

    // サテライトモード
    bool satellite = false;
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
    float moveDis = 1.0f;
    // サテライトカメラを移動させるときの時間（0～1）
    float time = 0;
    // サテライトカメラを移動させるときの時間の調整パラメータ
    float timeCoef = 2.0f;
    // rayの長さ
    public float rayDis = 100.0f;
    // カメラの水平移動を行うための条件
    bool slide = false;

    // 一人称カメラモード
    bool firstPerson = false;
    // マウス感度：SatelliteCamについて
    float mouseSensitivity_S = 0.02f;
    // マウス感度：FirstPersonCamについて
    float mouseSensitivity_FP = 0.2f;
    // BringingPos（Yagikun3Dの手の位置）
    GameObject bringingPos;
    // 直前のマウス座標
    Vector2 lastMousePosition;

    // ゲーム終了判定（クリア条件を満たす最後のブロックが完全にはめ込まれた後にtrueにする）
    bool gameOver = false;

    void Awake()
    {
        // Cameraの取得
        cam = this.GetComponent<Camera>();
        // playerの取得
        player = GameObject.Find("Player");
        // PlayerControllerの取得
        pC = player.GetComponent<PlayerController>();
        // Playerのスキンの取得
        pSkin = GameObject.FindWithTag("PlayerTexture").GetComponent<SkinnedMeshRenderer>();
        // facePosの取得
        facePos = GameObject.Find("FacePos");
        // camPosの取得
        camPos = GameObject.Find("CamPos");
        // ClearLightの取得
        clearLight = GameObject.Find("ClearLight");
        // ClearJudgementの取得
        cJ = GameObject.Find("GameDirector").GetComponent<ClearJudgement>();
        // 追跡用ロボットの取得
        robot = GameObject.Find("TrackingRobot");
        // InsideColorBoxのMeshRendererの取得
        insideColorBox = GameObject.FindWithTag("InsideColorBox").GetComponent<MeshRenderer>();
        // BringingPosの取得
        bringingPos = GameObject.FindWithTag("BringingPos");

        GameObject uiD = GameObject.Find("UIDirector");
        // StageUIの取得
        sUI = uiD.GetComponent<StageUI>();
        // SFXPlayerの取得
        sfx_UI = uiD.GetComponent<SFXPlayer>();
    }

    void Start()
    {
        // 初期値
        cam.fieldOfView = initialFieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            if (satellite) SatelliteCamera();
            else if (firstPerson) FirstPersonCamera();
            else ChasingCamera();
        }
    }
    void SatelliteCamera()
    {
        if (openingSequence)
        {
            // カメラ切り替えボタン入力制限
            StartCoroutine("ProhibitCamSwitchingTime");

            // 初期カメラ位置への移動
            OpeningSatelliteCamMovement();

            openingSequence = false;
        }

        if (rolling) SatelliteCamMovement_Rotate();
        else if(camControl) // 回転中でない時且つカメラの操作が可能な時
        {
            SatelliteCamMovement_Input();
            ZoomInAndOut();
            HorizontalMovementOfViewpoint();

            // サテライトモードの終了条件
            EndConditionOfSatelliteMode();
        }
    }
    void ZoomInAndOut()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float view = cam.fieldOfView - scroll * scrollSensitivity;

        cam.fieldOfView = Mathf.Clamp(view, 20f, 60f);
    }
    void HorizontalMovementOfViewpoint()
    {
        if (Input.GetMouseButtonDown(0)) // 押したとき
        {
            // 押し込んだ時、ステージ上にポインタがあるかどうか
            slide = CheckHorizontalMovementPossibilities();

            // スライド可能な時、マウス座標をlastMousePositionに格納する
            if (slide) lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && slide) // 押しこんでいるとき、且つスライド可能な時
        {
            // 新しいマウス座標と直前のマウス座標との差分を取得
            float x_Mouse = Input.mousePosition.x - lastMousePosition.x;
            float y_Mouse = Input.mousePosition.y - lastMousePosition.y;

            // カメラの水平移動
            transform.position -= transform.right * x_Mouse * mouseSensitivity_S + transform.up * y_Mouse * mouseSensitivity_S;

            // マウス座標をlastMousePositionに格納
            lastMousePosition = Input.mousePosition;
        }
    }
    bool CheckHorizontalMovementPossibilities()
    {
        bool slide = false;
        // カメラからタッチした場所まで光線を作成
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDis);
        // 長さrayDisの光線を作成しオブジェクトを取得
        if (hits.Length > 0)
        {
            for(int i = 0; i < hits.Length; i++)
            {
                if(hits[i].transform.gameObject.CompareTag("OutsideBox")) 
                {
                    // OutsideBoxが１つでもあればtrueを返して処理終了
                    slide = true;
                    break;
                }
            }
        }

        return slide;
    }
    void OpeningSatelliteCamMovement()
    {
        // 初期位置へのカメラの移動
        currentGao = pC.CurrentFace;
        transform.position = currentGao.transform.GetChild(0).transform.position;

        // カメラの視点: 原点位置の方を向く（上方向はプレイヤーと合わせる）
        Quaternion rotation = Quaternion.LookRotation(Vector3.zero - transform.position, player.transform.up);
        transform.rotation = rotation;

        UpdateRobotInfo_First();
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

            // 最近面・辺探索
            FindNearestFaceOrEdge();

            // ロボットの位置を元に戻す
            robot.transform.position -= transform.right * moveDis * horA;

            // ロボットの移動
            UpdateRobotInfo();

            // horizontal の入力を切り替える
            horizontal = !horizontal;

            // fieldOfViewの初期化
            cam.fieldOfView = initialFieldOfView;

            // 回転の効果音を鳴らす
            sfx_UI.PlaySFX(9);

            // 回転処理に移行する
            rolling = true;
        }
        else if (Mathf.Abs(ver) > float.Epsilon)
        {
            // robotによる探索（上下移動）
            robot.transform.position += transform.up * moveDis * verA;

            // 最近面・辺探索
            FindNearestFaceOrEdge();

            // ロボットの位置を元に戻す
            robot.transform.position -= transform.up * moveDis * verA;

            // ロボットの移動
            UpdateRobotInfo();

            // vertical の入力を切り替える
            vertical = !vertical;

            // fieldOfViewの初期化
            cam.fieldOfView = initialFieldOfView;

            // 回転の効果音を鳴らす
            sfx_UI.PlaySFX(10);

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
    void FindNearestFaceOrEdge()
    {
        // 直前までカメラが面していたのは面か辺か
        if (!vertical && !horizontal) // 面の時
        {
            face = currentGao.GetComponent<FaceInformation>();
            float? diff = null;

            for (int i = 0; i < 4; i++)
            {
                // robotの探索移動前後の距離を比較し、最も距離が縮まった辺（のGameObject）を割り出す
                // robotの探索移動前（＝現在いる面）と移動先候補の辺との距離
                float beforeDis = (face.edge[i].transform.position - currentGao.transform.position).magnitude;
                // robotの探索移動後と移動先候補の辺との距離
                float afterDis = (face.edge[i].transform.position - robot.transform.position).magnitude;
                if (diff == null || afterDis - beforeDis < diff)
                {
                    diff = afterDis - beforeDis;
                    nextGao = face.edge[i];
                }
            }
        }
        else // 辺の時
        {
            edge = currentGao.GetComponent<EdgeInformation>();
            float? diff = null;

            for (int i = 0; i < 2; i++)
            {
                // robotの探索移動前後の距離を比較し、最も距離が縮まった面（のGameObject）を割り出す
                // robotの探索移動前（＝現在いる面）と移動先候補の辺との距離
                float beforeDis = (edge.face[i].transform.position - currentGao.transform.position).magnitude;
                // robotの探索移動後と移動先候補の辺との距離
                float afterDis = (edge.face[i].transform.position - robot.transform.position).magnitude;
                if (diff == null || afterDis - beforeDis < diff)
                {
                    diff = afterDis - beforeDis;
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
    void EndConditionOfSatelliteMode()
    {
        // サテライトモード時に対応するボタンをもう一度押された場合、サテライトモードを終了する
        // ただし、カメラ切り替え後の再切替え禁止時間、またはカメラ回転中の入力は無効とする
        bool switchCam = false;
        if (!pC.ProhibitCamSwitching && !rolling) switchCam = Input.GetButtonDown("SwitchCamMode");

        if (switchCam) SwitchSatelliteToFirstPersonCam();
    }
    void SwitchSatelliteToFirstPersonCam()
    {
        //ChasingCamera(); // 追尾カメラに戻す
        // 追尾カメラに戻した後、0.5秒間は衛星カメラ入力を受け付けない
        //StartCoroutine("ProhibitCamSwitchingTime");
        //pC.Control = true;

        // 初期化
        vertical = false;
        horizontal = false;

        openingSequence = true;
        satellite = false;
        cam.fieldOfView = initialFieldOfView;

        // CamTypeUIの切り替え
        sUI.SwitchCamTypeUI_ToFirstPerson();

        // FirstPersonCameraへの移行
        firstPerson = true;
    }
    public void SwitchToFirstPersonCamMode_PressedButton()
    {
        if (!pC.ProhibitCamSwitching && !rolling) SwitchSatelliteToFirstPersonCam();
    }
    void FirstPersonCamera()
    {
        if(openingSequence)
        {
            // カメラ切り替えボタン入力制限
            StartCoroutine("ProhibitCamSwitchingTime");

            // カメラの移動
            transform.position = facePos.transform.position;
            transform.rotation = facePos.transform.rotation;

            // Playerとその周りのオブジェクトの非表示
            FirstPersonCam_HideObjects();

            // 直前のマウス座標の初期化
            lastMousePosition = Input.mousePosition;

            openingSequence = false;
        }

        if(camControl) // カメラの操作が可能な時
        {
            FirstPersonCamMovement();
            ZoomInAndOut();

            // 一人称カメラモードの終了条件
            EndConditionOfFirstPersonMode();
        }
    }

    void FirstPersonCam_HideObjects()
    {
        // Playerのスキンの非表示
        pSkin.enabled = false;

        // InsideColorBoxのMeshRendererの非表示
        insideColorBox.enabled = false;

        // 手に持っているアイテムの非表示
        // ①全てのMeshRendererをOFFにする
        var childMeshRenderers = bringingPos.GetComponentsInChildren<MeshRenderer>();
        foreach (var item in childMeshRenderers) item.GetComponent<MeshRenderer>().enabled = false;

        // ②全てのParticleSystemを停止する
        var childParticleSystems = bringingPos.GetComponentsInChildren<ParticleSystem>();
        foreach (var item in childParticleSystems) item.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);

        // ③全てのLightをOFFにする
        var childLights = bringingPos.GetComponentsInChildren<Light>();
        foreach (var item in childLights) item.GetComponent<Light>().enabled = false;
    }
    void FirstPersonCamMovement()
    {
        // 新しいマウス座標と直前のマウス座標との差分を取得
        float x_Mouse = Input.mousePosition.x - lastMousePosition.x;
        float y_Mouse = Input.mousePosition.y - lastMousePosition.y;

        // マウス座標をlastMousePositionに格納
        lastMousePosition = Input.mousePosition;

        // カメラの正面方向（forward）の決定
        Quaternion local = transform.localRotation;
        Quaternion rot = Quaternion.Euler(-y_Mouse * mouseSensitivity_FP, x_Mouse * mouseSensitivity_FP, 0);
        transform.localRotation = local * rot;

        // 縦方向の角度制限（カメラが垂直方向を向くとぐるぐるするため）
        float angle = Vector3.Angle(player.transform.up, transform.forward);

        if (angle >= 10.0f && angle <= 170.0f) // 10度以上170度以下の場合はそのまま次の処理
        {
            // カメラの上方向（up）の決定
            // プレイヤーのplayer.transform.upからカメラのtransform.forwardに下ろした垂線がカメラの上方向になる
            Vector3 upward = player.transform.up - Vector3.Project(player.transform.up, transform.forward);
            transform.rotation = Quaternion.LookRotation(transform.forward, upward);
        }
        else // 10度以下あるいは170度以上の場合は、回転を元に戻す
        {
            transform.localRotation = local;
        }
    }
    void EndConditionOfFirstPersonMode()
    {
        // 一人称カメラモード時に対応するボタンをもう一度押された場合、一人称カメラモードを終了する
        // ただし、カメラ切り替え後の再切替え禁止時間中の入力は無効とする
        bool switchCam = false;
        if (!pC.ProhibitCamSwitching) switchCam = Input.GetButtonDown("SwitchCamMode");

        if (switchCam) SwitchFirstPersonToNormalCam();
    }
    void SwitchFirstPersonToNormalCam()
    {
        // Playerとその周りのオブジェクトの再表示
        FirstPersonCam_RedisplayObjects();

        ChasingCamera(); // 追尾カメラに戻す
                         // 追尾カメラに戻した後、0.5秒間は衛星カメラ入力を受け付けない
        StartCoroutine("ProhibitCamSwitchingTime");
        pC.Control = true;

        // 初期化
        openingSequence = true;
        cam.fieldOfView = initialFieldOfView;

        // CamTypeUIの切り替え
        sUI.SwitchCamTypeUI_ToNormal();

        // HelpButtonの非表示
        sUI.HideHelpButton();

        // ChasingCameraへの移行
        firstPerson = false;
    }
    public void SwitchToNormalCamMode_PressedButton()
    {
        if (!pC.ProhibitCamSwitching) SwitchFirstPersonToNormalCam();
    }
    void FirstPersonCam_RedisplayObjects()
    {
        // Playerのスキンの再表示
        pSkin.enabled = true;

        // InsideColorBoxのMeshRendererの再表示
        insideColorBox.enabled = true;

        // 手に持っているアイテムの再表示
        // ①全てのMeshRendererをONにする
        var childMeshRenderers = bringingPos.GetComponentsInChildren<MeshRenderer>();
        foreach (var item in childMeshRenderers) item.GetComponent<MeshRenderer>().enabled = true;

        // ②全てのParticleSystemを再開する
        var childParticleSystems = bringingPos.GetComponentsInChildren<ParticleSystem>();
        foreach (var item in childParticleSystems) item.GetComponent<ParticleSystem>().Play(true);

        // ③全てのLightをOFFにする
        var childLights = bringingPos.GetComponentsInChildren<Light>();
        foreach (var item in childLights) item.GetComponent<Light>().enabled = true;
    }
    IEnumerator ProhibitCamSwitchingTime()
    {
        pC.ProhibitCamSwitching = true;

        // カメラ入力受け付け禁止時間
        yield return new WaitForSeconds(0.1f);

        pC.ProhibitCamSwitching = false;
    }
    void ChasingCamera()
    {
        // カメラの移動
        transform.position = camPos.transform.position;
        // カメラの視点: playerの方を向く（playerの頭が上になるように）
        Quaternion rotation = Quaternion.LookRotation(player.transform.position - camPos.transform.position, player.transform.up);
        transform.rotation = rotation;
    }
    public void StartingOperation01()
    {
        // 移動先のtargetPosの取得
        Vector3 vec = (transform.position - clearLight.transform.position).normalized * targetDis01;
        targetPos = clearLight.transform.position + vec;

        // 最初の位置を記録
        Vector3 startPos = transform.position;
        // 最初の回転を記録
        Quaternion startRot = transform.rotation;
        // ターゲット位置に移動させてみる
        transform.position = targetPos;
        // ターゲットの方に向かせてみる
        transform.rotation = Quaternion.LookRotation(clearLight.transform.position - transform.position, transform.up);
        // ターゲットへの角度を取得
        targetAngle = transform.eulerAngles;
        // 最初の回転に戻す
        transform.rotation = startRot;
        // 最初の位置に戻す
        transform.position = startPos;
    }

    public void TurnAroundToStarLight()
    {
        //　カメラの角度をスムーズに動かす
        float xRotate = Mathf.SmoothDampAngle(transform.eulerAngles.x, targetAngle.x, ref rotVelocity.x, rotTime);
        float yRotate = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle.y, ref rotVelocity.y, rotTime);
        float zRotate = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle.z, ref rotVelocity.z, rotTime);
        transform.eulerAngles = new Vector3(xRotate, yRotate, zRotate);
    }

    public void MoveCloserToStarLight()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref moveVelocity, moveTime);

        // 十分距離近づいたとき、カメラを近づける処理を終了させる（併せて回転処理も終了となる）
        float checkDis = (targetPos - transform.position).sqrMagnitude;
        if(checkDis <= minDis)
        {
            cJ.GameOver01 = false;
            cJ.GameOver02 = true;
        }
    }
    public void Move_StarLightBecomesBackground()
    {
        // 移動先のtargetPosの取得及び移動
        Vector3 vec = player.transform.position - clearLight.transform.position;
        Vector3 targetPos = player.transform.position + vec.normalized * targetDis02_Depth + player.transform.up * targetDis02_Height;
        transform.position = targetPos;

        // カメラをターゲットの方に向かせる
        transform.rotation = Quaternion.LookRotation(clearLight.transform.position - transform.position, player.transform.up);
    }
    public bool Satellite
    {
        set { satellite = value; }
        get { return satellite; }
    }
    public bool FirstPerson
    {
        set { firstPerson = value; }
        get { return firstPerson; }
    }
    public bool GameOver
    {
        set { gameOver = value; }
        get { return gameOver; }
    }
    public bool CamControl
    {
        set { camControl = value; }
        get { return camControl; }
    }
}
