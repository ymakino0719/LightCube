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
	// 地上ジャンプの高さ
	float jumpH_G = 6.5f;
	// 空中ジャンプの高さ
	float jumpH_A = 6.5f;
	// 最大ジャンプ回数（不変値※残りジャンプ回数と一致させる）
	int jumpNum_Max = 2;
	// 残りジャンプ回数（可変値）
	int jumpNum = 2;
	// 空中ジャンプ時の回転中かどうか
	//bool aerialJumpRot = false;
	// 空中ジャンプ時の回転時間
	//float timeAJ = 0.0f;
	// 空中ジャンプ時の回転速度
	//float rotSpeedAJ = 4.0f;

	// アイテムを持っているか
	bool holding = false;
	// 移動スピード
	float moveS = 15.0f;
	// 平面方向（XとZ方向）に対する移動上限速度
	float moveLimit_XZ = 2.0f;
	// 上下方向（Y方向）に対する移動上限速度
	float moveLimit_Y = 3.0f;
	// キャラクターが静止しているかどうか
	bool stopping = true;

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
	CameraController cC;

	// 近くにあるitem, LockBlock
	GameObject nearestItem, nearestLock;
	// 近くのitem探索で使用するコライダーのリスト
	Collider[] colList;
	// プレイヤーの原点位置からの探索範囲（キーブロック、ロックブロックの探索）
	float searchDis_Key = 1.0f;
	float searchDis_Lock = 0.35f;

	// ゲームクリア判定: 遷移01 での使用する回転方向先
	Quaternion g01_Rot;

	// 振り向き時間（StarLight）
	float timeSL = 0.0f;
	// 振り向き速度（StarLight）
	float rotSpeedSL = 0.02f;

	// プレイヤーが今いる（直前にいた）Face
	GameObject currentFace = null;

	// プレイヤーが中間地点を経由したかどうか
	bool halfway = false;
	// 中間地点を経由したプレイヤーを回転させるための基準となるオブジェクト
	GameObject rotRef;

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

	// ゲーム終了判定（クリア条件を満たす最後のブロックが接触した時点でプレイヤーのコントロールを無効にする）
	bool gameOver = false;

	// Start is called before the first frame update
	void Awake()
    {
        rBody = GetComponent<Rigidbody>();
		// Playerの直下の一番上に配置しているYagikun3DにアタッチされているAnimationControllerを取得する
		yagikun = transform.GetChild(0).gameObject;
		aC = yagikun.GetComponent<AnimationController>();
		// ClearJudgementの取得
		cJ = GameObject.Find("GameDirector").GetComponent<ClearJudgement>();
		cC = GameObject.Find("Camera").GetComponent<CameraController>();
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
			
			jump = Input.GetButtonDown("Jump");

			/////////////////////////////////
			///////// JudgeStopping /////////
			/////////////////////////////////

			// キャラクターが静止状態かつ着地しているとき
			stopping = (rBody.velocity.sqrMagnitude < minimumSpeed && isGround) ? true : false;

			/////////////////////////////////
			/////// PickUp or PutDown ///////
			/////////////////////////////////

			bool pick = false;
			// 静止状態かつ着地しているときpickを可能にする
			if (stopping) pick = Input.GetButtonDown("Pick");

			/////////////////////////////////
			///////// Satellite Cam /////////
			/////////////////////////////////

			bool satelliteCam = false;
			// 静止、着地状態かつカメラの切り替えを禁止していないとき、satelliteCamを可能にする
			if (stopping && !prohibitCamSwitching) satelliteCam = Input.GetButtonDown("SatelliteCam");

			/////////////////////////////////
			/////// First Person Cam ////////
			/////////////////////////////////

			bool firstPersonCam = false;
			// 静止、着地状態かつカメラの切り替えを禁止していないとき、satelliteCamを可能にする
			if (stopping && !prohibitCamSwitching) firstPersonCam = Input.GetButtonDown("FirstPersonCam");

			/////////////////////////////////
			/////////// Functions ///////////
			/////////////////////////////////

			// 静止状態で衛星カメラへの切り替えがあった場合、優先的に処理する（他処理は無視する）
			bool inputNewCamMode = false;
			if (satelliteCam || firstPersonCam) inputNewCamMode = SwitchCamMode(satelliteCam, firstPersonCam);

			// 衛星カメラへ切り替える場合、他処理は無視する
			if (inputNewCamMode) return;

			// アイテムを持っていないときに拾うかどうか、またはアイテムを持っているときに置くかどうか判定する
			if (pick) 
			{
				if(!holding) JudgePickUpItems(ref pick);
				else JudgePutDownItems(ref pick);
			}

			// プレイヤーのリギッドボディのローカル速度locVelを取得
			locVel = transform.InverseTransformDirection(rBody.velocity);

			UpdateJumpNum();
			aC.MoveAnimation(jump, jumpNum, jumpNum_Max, pick, holding, isGround, lastGround, locVel);
			MoveCharacter(hor, ver, jump, locVel);

			// lastGround（直前に着地していたか）の更新
			lastGround = (isGround) ? true : false;
		}
		else // 次の面に移動処理中のとき（EdgeInformationで処理しているとき）
		{

        }
	}
	void FixedUpdate()
	{
		if (control)
		{
			// 仮想重力をかけ続ける
			rBody.AddForce(-transform.up * gravity);
		}

		if (throughGate)
		{
			ThroughGateRotation();
		}

		/*
		if(aerialJumpRot)
        {
			AerialJumpRotation();
        }
		*/
	}
	IEnumerator OpeningControlStopCoroutine()
	{
		// 開幕n秒間の操作を止める
		yield return new WaitForSeconds(2.0f);

		StageUI sUI = GameObject.Find("UIDirector").GetComponent<StageUI>();
		if (sUI.FirstStage) sUI.HowToPlay();
		else control = true;
	}
	bool SwitchCamMode(bool satelliteCam, bool firstPersonCam)
    {
		// カメラのサテライトモードをオンにする
		if (satelliteCam) cC.Satellite = true;
		else if(firstPersonCam) cC.FirstPerson = true;

		// プレイヤーの操作を無効にする
		control = false;

		// 以下の処理の終了用
		bool iNCM = true;
		return iNCM;
	}

	void UpdateJumpNum()
    {
		if(isGround && !lastGround)
        {
			// 着地時、ジャンプ回数を元に戻す
			jumpNum = jumpNum_Max;
		}

		if (!isGround && lastGround)
        {
			// ジャンプあるいは落下時、ジャンプ回数を１減らす
			jumpNum -= 1;
		}
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
			locVel.z += hor * moveS;
			//rBody.AddForce(transform.forward * hor * moveS);
		}

		// locVel.x：カメラから見てプレイヤーの奥行
		if ((ver > float.Epsilon && locVel.x < moveLimit_XZ) || (ver < float.Epsilon && locVel.x > -moveLimit_XZ))
		{
			// locVel.x：プレイヤーの奥行移動
			locVel.x -= ver * moveS;
			//rBody.AddForce(-transform.right * ver * moveS);
		}

		///////////////////////
		//////// jump /////////
		///////////////////////

		if (jump)
		{
			if (isGround) // 地上にいるとき
			{
				// 地上ジャンプ
				locVel.y = jumpH_G;
			}
			else if (jumpNum > 0 && jumpNum < jumpNum_Max) // 滞空中で、残りジャンプ回数が０より大きく最大数より小さい場合
			{
				// 空中ジャンプ
				locVel.y = jumpH_A;
				// ジャンプ回数を１減らす
				jumpNum -= 1;
				// 空中ジャンプの回転モーションの開始
				//aerialJumpRot = true;
			}
		}

		///////////////////////
		////// movelimit //////
		///////////////////////

		// XとZ軸方向に移動制限をかける
		if (Mathf.Abs(locVel.x) >= moveLimit_XZ) locVel.x = (locVel.x >= 0) ? moveLimit_XZ : -moveLimit_XZ;
		if (Mathf.Abs(locVel.z) >= moveLimit_XZ) locVel.z = (locVel.z >= 0) ? moveLimit_XZ : -moveLimit_XZ;

		// Y軸方向は下降中にのみ制限をかける（ジャンプに制限をかけないようにするため）
		if (locVel.y <= -moveLimit_Y)  locVel.y = -moveLimit_Y;

		///////////////////////
		/////// rotate ////////
		///////////////////////

		// プレイヤーの速度の向きに方向転換する（X、Z軸方向のみ、Y軸は無効）
		if (Mathf.Abs(locVel.x) >= minimumSpeed || Mathf.Abs(locVel.z) >= minimumSpeed)
        {
			Vector3 looking = new Vector3(locVel.x, 0, locVel.z);
			yagikun.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(looking), transform.up); // 向きを変更する
		}

		///////////////////////
		///// integration /////
		///////////////////////

		// 上記で再計算されたlocVelをrigidbodyに反映させる
		rBody.velocity = transform.TransformDirection(locVel);
	}

	/*
	void AerialJumpRotation()
    {
		Debug.Log("Hello");
		// 時間
		timeAJ += rotSpeedAJ * Time.deltaTime;
		// 回転
		yagikun.transform.rotation *= Quaternion.Euler(0, rotSpeedAJ * Time.deltaTime * 360.0f, 0);

		if (timeAJ >= 1.0f)
		{
			// 時間を超過したら初期回転方向に戻し、回転終了
			float diff = timeAJ - 1.0f;
			yagikun.transform.rotation *= Quaternion.Euler(0, -diff * 360.0f, 0);
			
			timeAJ = 0;
			aerialJumpRot = false;
		}
	}
	*/

	void JudgePickUpItems(ref bool pick)
    {
		// stringにタグ名を代入
		string tagName = "Item";

		// 近くにitemがあるか探索
		nearestItem = FindTargetsNearby(tagName, searchDis_Key);

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
		TurnTowardTheTarget(nearestItem);
	}
	void JudgePutDownItems(ref bool pick)
	{
		string tagName = "Lock";

		// 近くにLockBlockがあるか探索
		nearestLock = FindTargetsNearby(tagName, searchDis_Lock);

		if (nearestLock == null) // 近くにLockBlockがない場合、pickは無効、終了する
		{
			pick = false;
			return;
		}

		// LockBlockの方を振り向く
		TurnTowardTheTarget(nearestLock);
	}
	GameObject FindTargetsNearby(string tag, float searchDis)
    {
		// プレイヤーの原点位置を中心にオブジェクト探索
		colList = Physics.OverlapSphere(transform.position, searchDis);

		// 近くのゲームオブジェクト
		GameObject target = null;

		for (int i = 0; i < colList.Length; i++)
		{
			if (colList[i].gameObject.CompareTag(tag)) // 指定のタグが付いたオブジェクトのみを対象とする
			{
				if(target == null) // 近くのオブジェクトが１つのみだった場合ここの処理で終わる
                {
					// targetには衝突したオブジェクトの１つ上の親オブジェクトを代入する（子オブジェクトに回転情報を持たせる都合によるもの）
					target = colList[i].transform.parent.gameObject;
				}
				else // 近くのオブジェクトが複数ある場合、一番距離が近いitemを特定、nItemを更新する
				{
					float disA = (transform.position - colList[i].transform.parent.gameObject.transform.position).sqrMagnitude;
					float disB = (transform.position - target.transform.position).sqrMagnitude;

					if(disA < disB)
                    {
						target = colList[i].transform.parent.gameObject;
					}
				}
			}
		}

		return target;
	}

	void TurnTowardTheTarget(GameObject target)
    {
		// プレイヤーとitemの座標差を調べる
		Vector3 diff = target.transform.position - transform.position;

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
	}
	public void LookAtStarLightSmoothly_Processing()
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
	public void LookAtStarLightSmoothly_End()
	{
		yagikun.transform.rotation = g01_Rot;
		TurnTheOtherWay();

		timeSL = 0.0f;
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

		transform.rotation = Quaternion.Lerp(transform.rotation, rotRef.transform.rotation, timeBr);

		// Playerの前方方向の向きとrotRefの前方方向の向きの角度を確認する（前方はx軸のためtransform.rightとなる）
		float angle = Vector3.Angle(transform.right, rotRef.transform.right);

		// timeBr が1を超過またはangleが0.1度未満になった場合処理を終了させる
		if (timeBr >= 1.0f || angle <= 0.1f)
		{
			transform.rotation = rotRef.transform.rotation;

			timeBr = 0.0f;
			throughGate = false;
		}
	}
	public bool IsGround
	{
		set { isGround = value; }
		get { return isGround; }
	}
	public bool Stopping
	{
		set { stopping = value; }
		get { return stopping; }
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
	public GameObject CurrentFace
	{
		set { currentFace = value; }
		get { return currentFace; }
	}
	public bool Halfway
	{
		set { halfway = value; }
		get { return halfway; }
	}
	public GameObject RotRef
	{
		set { rotRef = value; }
		get { return rotRef; }
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
	public bool GameOver
	{
		set { gameOver = value; }
		get { return gameOver; }
	}
	public int JumpNum_Max
	{
		get { return jumpNum_Max; }
	}
	public int JumpNum
	{
		set { jumpNum = value; }
		get { return jumpNum; }
	}
}
