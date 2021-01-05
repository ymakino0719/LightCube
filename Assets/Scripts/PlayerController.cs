using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // PlayerのRigidbody
    Rigidbody rBody;
    // 仮想重力の係数
    float gravity = 2.0f;

	// Yagikun3D
	GameObject yagikun;

	//Animator animator;
	// 着地しているかどうか（CheckIsGroundからの判定結果）
	bool isGround = false;
	// 1つ前のフレームで着地していたかどうか
	bool lastGround = false;
	// ジャンプの高さ
	float jumpH = 9.0f;
	// アイテムを持っているか
	bool holding = false;
	// 移動スピード
	float moveS = 1.5f;
	// 平面方向（XとZ方向）に対する移動上限速度
	float moveLimit_XZ = 2.0f;
	// 上下方向（Y方向）に対する移動上限速度
	float moveLimit_Y = 3.0f;

	// 1フレーム前にキャラクターがいた場所
	Vector3 latestPos;
	// Playerが動いているかどうかの閾値
	float minimumSpeed = 0.01f;

	// 次に移動する面に移動中かどうか
	bool control = true;

	// Yagikun3DにアタッチされているAnimationControllerの取得
	AnimationController aC;

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

	// 振り向き時間
	float time = 0.0f;
	// 振り向き速度
	float rotSpeed = 0.02f;

	// Start is called before the first frame update
	void Awake()
    {
        rBody = GetComponent<Rigidbody>();
		// Playerの直下の一番上に配置しているYagikun3DにアタッチされているAnimationControllerを取得する
		yagikun = transform.GetChild(0).gameObject;
		aC = yagikun.GetComponent<AnimationController>();
		
    }
	void Start()
	{
		latestPos = transform.position;
	}

	// Update is called once per frame
	void Update()
    {
		if(control) // 次の面に移動中でないとき（通常時）、その他キャラクターの操作を受け付けないとき
        {
			// 仮想重力をかけ続ける
			rBody.AddForce(-transform.up * gravity);


			// Playerをキー操作で動かす
			/////////////////////////////////
			//////////// Moving /////////////
			/////////////////////////////////

			float hor = Input.GetAxis("Horizontal");
			float ver = Input.GetAxis("Vertical");

			/////////////////////////////////
			//////////// Jumping ////////////
			/////////////////////////////////
			bool jump = false;
			// 着地しているときジャンプを可能にする
			if (isGround) jump = Input.GetButtonDown("Jump");

			/////////////////////////////////
			/////// PickUp or PutDown ///////
			/////////////////////////////////
			///
			bool pick = false;
			if (rBody.velocity.sqrMagnitude < minimumSpeed) pick = Input.GetButtonDown("Pick");

			/////////////////////////////////
			/////////// Functions ///////////
			/////////////////////////////////

			// アイテムを持っていないときにpickの入力があった場合のみ、アイテムを拾うかどうか判定する
			if (pick && !holding) JudgePickUpItems(ref pick);

			// プレイヤーのリギッドボディのローカル速度locVelを取得
			Vector3 locVel = transform.InverseTransformDirection(rBody.velocity);

			aC.MoveAnimation(jump, pick, holding, isGround, lastGround, locVel);
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
		if (gameOver01) LookAtStarLightSmoothly_Processing();
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
		time += rotSpeed * Time.deltaTime;

		yagikun.transform.rotation = Quaternion.Lerp(yagikun.transform.rotation, g01_Rot, time);

		if(time >= 1.0f)
        {
			yagikun.transform.rotation = g01_Rot;
			time = 0.0f;

			TurnTheOtherWay();
			gameOver01 = false;
		}
	}
	void TurnTheOtherWay()
	{
		// yagikun3Dを180度回転させる
		yagikun.transform.Rotate(0, 180, 0);
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
}
