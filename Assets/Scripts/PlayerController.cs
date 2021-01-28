using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // PlayerのRigidbody
    Rigidbody rBody;
    // 仮想重力の係数
    float gravity = 12.0f;

	// Yagikun3D
	GameObject yagikun;

	//Animator animator;
	// 着地しているかどうか（CheckIsGroundからの判定結果）
	bool isGround = false;
	// 1つ前のフレームで着地していたかどうか
	bool lastGround = false;
	// ジャンプの高さ
	float jumpH = 9.5f;
	// アイテムを持っているか
	bool holding = false;
	// 移動スピード
	float moveS = 5.0f;
	// 平面方向（XとZ方向）に対する移動上限速度
	float moveLimit_XZ = 2.0f;
	// 上下方向（Y方向）に対する移動上限速度
	float moveLimit_Y = 3.0f;

	// 1フレーム前にキャラクターがいた場所
	Vector3 latestPos;
	// Playerが動いているかどうかの閾値
	float minimumSpeed = 0.01f;

	// 操作可能な状態かどうか
	bool control = false;

	// Yagikun3DにアタッチされているAnimationControllerの取得
	AnimationController aC;
	// ClearJudgement
	ClearJudgement cJ;
	// CameraControl
	CameraControll cC;

	// 近くにあるitem
	GameObject nearestItem;
	// 近くのitem探索で使用するコライダーのリスト
	Collider[] colList;
	// プレイヤーの原点位置からの探索範囲
	float searchDis = 1.0f;

	// ゲームクリア判定
	bool gameOver = false;
	// ゲームクリア判定: 遷移01
	bool gameOver01 = false;
	// ゲームクリア判定: 遷移01 での使用する回転方向先
	Quaternion g01_Rot;

	// 振り向き時間（StarLight）
	float timeSL = 0.0f;
	// 振り向き速度（StarLight）
	float rotSpeedSL = 0.02f;

	// プレイヤーが今いる（直前にいた）Face
	GameObject currentFace = null;

	// プレイヤーが直前に通過した橋の中間地点
	GameObject midpoint = null;
	// ゲートの出口が浮島かどうか
	bool island = false;
	// 橋を通過し、カメラの回転が必要な場合
	bool throughGate = false;
	// カメラの振りむき時間
	float timeBr = 0.0f;
	// カメラの振りむき速度
	float rotSpeedBr = 0.2f;

	// 縦横移動
	float hor, ver;
	// ジャンプ
	bool jump;
	// プレイヤー基準のローカルベクトル
	Vector3 locVel;

	// 衛星カメラへの切り替え禁止時間（追尾カメラに戻してからすぐには戻せないように）
	bool prohibitCamSwitching = false;

	// Start is called before the first frame update
	void Awake()
    {
        rBody = GetComponent<Rigidbody>();
		// Playerの直下の一番上に配置しているYagikun3DにアタッチされているAnimationControllerを取得する
		yagikun = transform.GetChild(0).gameObject;
		aC = yagikun.GetComponent<AnimationController>();
		// ClearJudgementの取得
		cJ = GameObject.Find("GameDirector").GetComponent<ClearJudgement>();
		cC = GameObject.Find("Camera").GetComponent<CameraControll>();
	}
	void Start()
	{
		latestPos = transform.position;
		// 開幕n秒間の操作を止める
		StartCoroutine("OpeningControlStopCoroutine");
	}

	// Update is called once per frame
	void Update()
    {
		if(control) // 次の面に移動中でないとき（通常時）、その他キャラクターの操作を受け付けないとき
        {
			// Playerをキー操作で動かす
			/////////////////////////////////
			//////////// Moving /////////////
			/////////////////////////////////

			hor = Input.GetAxis("Horizontal");
			ver = Input.GetAxis("Vertical");

			/////////////////////////////////
			//////////// Jumping ////////////
			/////////////////////////////////
			jump = false;
			// 着地しているときジャンプを可能にする
			if (isGround) jump = Input.GetButtonDown("Jump");

			/////////////////////////////////
			/////// PickUp or PutDown ///////
			/////////////////////////////////
			
			bool pick = false;
			// 静止状態かつ着地しているときpickを可能にする
			if (rBody.velocity.sqrMagnitude < minimumSpeed && isGround) pick = Input.GetButtonDown("Pick");

			/////////////////////////////////
			///////// Satellite Cam /////////
			/////////////////////////////////

			bool satelliteCam = false;
			// 静止状態、着地しているときかつ切り替えを禁止していないとき、satelliteCamを可能にする
			if (rBody.velocity.sqrMagnitude < minimumSpeed && isGround && !prohibitCamSwitching) satelliteCam = Input.GetButtonDown("SatelliteCam");

			/////////////////////////////////
			/////////// Functions ///////////
			/////////////////////////////////

			// 静止状態で衛星カメラへの切り替えがあった場合、優先的に処理する（他処理は無視する）
			bool turnOnSCM = false;
			if (satelliteCam) turnOnSCM = TurnOnSatelliteCamMode();

			// 衛星カメラへ切り替える場合、他処理は無視する
			if (turnOnSCM) return;

			// アイテムを持っていないときにpickの入力があった場合のみ、アイテムを拾うかどうか判定する
			if (pick && !holding) JudgePickUpItems(ref pick);

			// プレイヤーのリギッドボディのローカル速度locVelを取得
			locVel = transform.InverseTransformDirection(rBody.velocity);

			aC.MoveAnimation(jump, pick, holding, isGround, lastGround, locVel);
			MoveCharacter(hor, ver, jump, locVel);

			// lastGround（直前に着地していたか）の更新
			lastGround = (isGround) ? true : false;
		}
		else // 次の面に移動処理中のとき（EdgeInformationで処理しているとき）
		{

        }
	}
	IEnumerator OpeningControlStopCoroutine()
	{
		// 開幕n秒間の操作を止める
		yield return new WaitForSeconds(2.0f);

		control = true;

		PausedUI pUI = GameObject.Find("UIDirector").GetComponent<PausedUI>();
		if(pUI.FirstStage) pUI.HowToPlay();
	}

	void FixedUpdate()
    {
		if(control)
        {
			// 仮想重力をかけ続ける
			rBody.AddForce(-transform.up * gravity);
		}

		if (gameOver01)
        {
			if (cJ.GameOver01)
			{
				// yagikun3DをStarLightの方向にゆっくり回転させる
				LookAtStarLightSmoothly_Processing();
			}
			else
            {
				// 回転の強制終了、gameOver02に移行させる
				LookAtStarLightSmoothly_End();
			}
		}

		if(throughGate)
        {
			ThroughGateRotation();
        }
	}

	bool TurnOnSatelliteCamMode()
    {
		// カメラのサテライトモードをオンにする
		cC.Satellite = true;

		// プレイヤーの操作を無効にする
		control = false;

		// 以下の処理の終了用
		bool tOSCM = true;
		return tOSCM;
	}
	void MoveCharacter(float hor, float ver, bool jump, Vector3 locVel)
	{
		///////////////////////
		//////// move /////////
		///////////////////////

		// locVel.z：カメラから見てプレイヤーの左右
		if ((hor > float.Epsilon && locVel.z < moveLimit_XZ) || (hor < float.Epsilon && locVel.z > -moveLimit_XZ))
		{
			// locVel.z：プレイヤーの左右移動
			rBody.AddForce(transform.forward * hor * moveS);
		}

		// locVel.x：カメラから見てプレイヤーの奥行
		if ((ver > float.Epsilon && locVel.x < moveLimit_XZ) || (ver < float.Epsilon && locVel.x > -moveLimit_XZ))
		{
			// locVel.x：プレイヤーの奥行移動
			rBody.AddForce(-transform.right * ver * moveS);
		}

		///////////////////////
		////// movelimit //////
		///////////////////////

		// XとZ軸方向に移動制限をかける
		if (Mathf.Abs(locVel.x) >= moveLimit_XZ) locVel.x = (locVel.x >= 0) ? moveLimit_XZ : -moveLimit_XZ;
		if (Mathf.Abs(locVel.z) >= moveLimit_XZ) locVel.z = (locVel.z >= 0) ? moveLimit_XZ : -moveLimit_XZ;

		// Y軸方向は下降中にのみ制限をかける（ジャンプに制限をかけないようにするため）
		if (locVel.y <= -moveLimit_Y)  locVel.y = -moveLimit_Y;

		rBody.velocity = transform.TransformDirection(locVel);

		///////////////////////
		//////// jump /////////
		///////////////////////

		if (isGround && jump)
		{
			rBody.velocity += transform.up * jumpH;
		}

		///////////////////////
		/////// rotate ////////
		///////////////////////

		// プレイヤーのリギッドボディのローカル速度locVel2を再取得
		Vector3 locVel2 = transform.InverseTransformDirection(rBody.velocity);

		// プレイヤーの速度の向きに方向転換する（X、Z軸方向のみ、Y軸は無効）
		if (Mathf.Abs(locVel2.x) >= minimumSpeed || Mathf.Abs(locVel2.z) >= minimumSpeed)
        {
			Vector3 looking = new Vector3(locVel2.x, 0, locVel2.z);
			yagikun.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(looking), transform.up); // 向きを変更する
		}
	}

	void JudgePickUpItems(ref bool pick)
    {
		// 近くにitemがあるか探索
		nearestItem = FindItemsNearby();

		if (nearestItem == null) // 近くにitemがない場合、pickは無効、終了する
		{
			pick = false;
			return;
		}
		else // ある場合、nearestItemを更新する
        {
			aC.NearestItem = nearestItem;
        }

		// itemの方を振り向く
		TurnTowardTheItem(nearestItem);
	}
	GameObject FindItemsNearby()
    {
		// プレイヤーの原点位置を中心にオブジェクト探索
		colList = Physics.OverlapSphere(transform.position, searchDis);

		// 近くのゲームオブジェクト
		GameObject nItem = null;

		for (int i = 0; i < colList.Length; i++)
		{
			if (colList[i].gameObject.CompareTag("Item")) // Itemのタグが付いたオブジェクトのみを対象とする
			{
				if(nItem == null) // 近くのオブジェクトが１つのみだった場合ここの処理で終わる
                {
					// nItemには衝突したオブジェクトの１つ上の親オブジェクトを代入する
					nItem = colList[i].transform.parent.gameObject;
				}
				else // 近くのオブジェクトが複数ある場合、一番距離が近いitemを特定、nItemを更新する
				{
					float disA = (transform.position - colList[i].transform.parent.gameObject.transform.position).sqrMagnitude;
					float disB = (transform.position - nItem.transform.position).sqrMagnitude;

					if(disA < disB)
                    {
						nItem = colList[i].transform.parent.gameObject;
					}
				}
			}
		}

		return nItem;
	}
	void TurnTowardTheItem(GameObject nearestItem)
    {
		// プレイヤーとitemの座標差を調べる
		Vector3 diff = nearestItem.transform.position - transform.position;

		// プレイヤーを基準としたローカル座標系locPosに変換
		Vector3 locPos = transform.InverseTransformDirection(diff);
		
		// プレイヤーを基準としたローカル座標系の内、Y軸の回転は行わないため、Y要素のみ無効とする
		Vector3 looking = new Vector3(locPos.x, 0, locPos.z);

		// yagikun3Dをオブジェクトのある位置に水平に方向転換する
		yagikun.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(looking), transform.up);
	}

	public void LookAtStarLightSmoothly_Beginning()
    {
		// プレイヤーから見たClearLightのベクトルを調べる
		Vector3 dir = GameObject.Find("ClearLight").transform.position - transform.position;

		// プレイヤーを基準としたローカル座標系locPosに変換
		Vector3 locPos = transform.InverseTransformDirection(dir);

		// プレイヤーを基準としたローカル座標系の内、Y軸の回転は行わないため、Y要素のみ無効とする
		Vector3 looking = new Vector3(locPos.x, 0, locPos.z);

		// yagikun3DをClearLightに向かって水平に方向転換させたときの回転を求める
		g01_Rot = Quaternion.LookRotation(transform.TransformDirection(looking), transform.up);

		gameOver01 = true;
	}
	void LookAtStarLightSmoothly_Processing()
	{
		// 時間
		timeSL += rotSpeedSL * Time.deltaTime;

		yagikun.transform.rotation = Quaternion.Lerp(yagikun.transform.rotation, g01_Rot, timeSL);

		if(timeSL >= 1.0f)
        {
			// 時間を超過したら回転終了
			LookAtStarLightSmoothly_End();
		}
	}
	void LookAtStarLightSmoothly_End()
	{
		yagikun.transform.rotation = g01_Rot;
		TurnTheOtherWay();

		timeSL = 0.0f;
		gameOver01 = false;
	}

	public void TurnTheOtherWay()
	{
		// yagikun3Dを180度回転させる
		yagikun.transform.Rotate(0, 180, 0);
	}
	void ThroughGateRotation()
	{
		// 時間
		timeBr += rotSpeedBr * Time.deltaTime;

		transform.rotation = Quaternion.Lerp(transform.rotation, midpoint.transform.rotation, timeBr);

		// Playerの前方方向の向きとmidpointの前方方向の向きの角度を確認する（前方はx軸のためtransform.rightとなる）
		float angle = Vector3.Angle(transform.right, midpoint.transform.right);

		// timeBr が1を超過またはangleが0.1度未満になった場合処理を終了させる
		if (timeBr >= 1.0f || angle <= 0.1f)
		{
			transform.rotation = midpoint.transform.rotation;

			timeBr = 0.0f;
			midpoint = null;
			throughGate = false;
		}
	}
	public bool IsGround
	{
		set { isGround = value; }
		get { return isGround; }
	}
	public bool Control
	{
		set { control = value; }
		get { return control; }
	}
	public bool Holding
	{
		set { holding = value; }
		get { return holding; }
	}
	public bool GameOver
	{
		set { gameOver = value; }
		get { return gameOver; }
	}
	public GameObject CurrentFace
	{
		set { currentFace = value; }
		get { return currentFace; }
	}
	public GameObject Midpoint
	{
		set { midpoint = value; }
		get { return midpoint; }
	}
	public bool Island
	{
		set { island = value; }
		get { return island; }
	}
	public bool ThroughGate
	{
		set { throughGate = value; }
		get { return throughGate; }
	}
	public bool ProhibitCamSwitching
	{
		set { prohibitCamSwitching = value; }
		get { return prohibitCamSwitching; }
	}
}
