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
	float jumpH = 6.0f;
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
	private bool control = true;

	// Yagikun3DにアタッチされているAnimationControllerの取得
	AnimationController aC;

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
		if(control) // 次の面に移動中でないとき（通常時）
        {
			// 仮想重力をかけ続ける
			rBody.AddForce(-transform.up * gravity);

			// Playerをキー操作で動かす

			//// Moving ////
			float hor = Input.GetAxis("Horizontal");
			float ver = Input.GetAxis("Vertical");

			//// Jumping ////
			bool jump = false;

			// 着地しているときジャンプを可能にする
			if (isGround)
            {
				jump = Input.GetButtonDown("Jump");
			}

			//// PickUp & PutDown ////
			bool pick = Input.GetButtonDown("Pick");
			//if (pick) holding = !holding;

			Vector3 vec = rBody.velocity;

			aC.MoveAnimation(jump, pick, holding, isGround, lastGround, vec);
			MoveCharacter(hor, ver, jump);
			//TurnDirection(move);

			// lastGround（直前に着地していたか）の更新
			lastGround = (isGround) ? true : false;
		}
		else // 次の面に移動処理中のとき（EdgeInformationで処理しているとき）
		{

        }

		Debug.Log("isGround : " + isGround);
	}

	void MoveCharacter(float hor, float ver, bool jump)
	{
		///////////////////////
		//////// move /////////
		///////////////////////

		// プレイヤーのリギッドボディのローカル速度locVelを取得
		Vector3 locVel = transform.InverseTransformDirection(rBody.velocity);

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

		//Debug.Log("ver: " + ver);
		//Debug.Log("locVel.z: " + locVel.z);

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
}
