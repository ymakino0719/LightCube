using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // PlayerのRigidbody
    Rigidbody rBody;
    // 仮想重力の係数
    float gravity = 0.5f;

	//Animator animator;
	// 着地しているかどうか（CheckIsGroundからの判定結果）
	bool isGround = false;
	// 1つ前のフレームで着地していたかどうか
	//bool lastGround = false;
	// ジャンプ中かどうか
	bool jump = false;
	// ジャンプの高さ
	float jumpH = 3.0f;
	// 移動スピード
	float moveS = 0.5f;
	// 移動上限速度
	float moveLimitS = 2.0f;
	//float walkAnimationSpeed = 0.01f;

	// 次に移動する面に移動中かどうか
	private bool control = true;

	// Start is called before the first frame update
	void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
		if(control) // 次の面に移動中でないとき（通常時）
        {
			// 仮想重力をかけ続ける
			rBody.AddRelativeForce(-Vector3.up * gravity);

			// Playerをキー操作で動かす

			////Moving////
			float hor = Input.GetAxis("Horizontal");
			float ver = Input.GetAxis("Vertical");

			////Jumping////
			jump = Input.GetButtonDown("Jump");

			//MoveAnimation(move, jump);
			MoveCharacter(hor, ver, jump);
			//TurnDirection(move);

			/* ★MoveAnimationのときに使うかも
			if (isGround)
			{
				lastGround = true;
			}
			else
			{
				lastGround = false;
			}
			*/
		}
		else // 次の面に移動処理中のとき（EdgeInformationで処理しているとき）
		{

        }
	}

	/* ★Animationは後々設定する予定
	void MoveAnimation(Vector2 move, bool jump)
	{
		if (jump)
		{
			animator.SetBool("jumpBool", true);
		}
		else if (!isGround && !lastGround)
		{
			animator.SetBool("jumpBool", false);
			animator.SetBool("jumpingBool", true);
		}
		else if (isGround)
		{
			animator.SetBool("crouchBool", crouch);

			if (!lastGround)
			{
				animator.SetBool("jumpingBool", false);
			}
			else
			{
				float k = Mathf.Abs(rBody.velocity.x) * walkAnimationSpeed;

				if (move.sqrMagnitude > float.Epsilon)
				{
					animator.SetFloat("moveSpeed", k);
				}
				else
				{
					animator.SetFloat("moveSpeed", k);
				}
			}
		}
	}
	*/
	void MoveCharacter(float hor, float ver, bool jump)
	{
		///////////////////////////////////////////////
		//////// ↓(i) AddForceを加える場合↓ /////////
		///////////////////////////////////////////////


		// プレイヤーのリギッドボディのローカル速度locVelを取得
		Vector3 locVel = transform.InverseTransformDirection(rBody.velocity);

		// locVel.z：カメラから見てプレイヤーの左右
		if (Mathf.Abs(locVel.z) < moveLimitS && Mathf.Abs(hor) > float.Epsilon)
		{
			// locVel.z：プレイヤーの左右移動
			rBody.AddForce(transform.forward * hor * moveS);
		}

		// locVel.x：カメラから見てプレイヤーの奥行
		if (Mathf.Abs(locVel.x) < moveLimitS && Mathf.Abs(ver) > float.Epsilon)
		{
			// locVel.x：プレイヤーの奥行移動
			rBody.AddForce(transform.right * hor * moveS);
		}

		///////////////////////////////////////////////


		///////////////////////////////////////////////
		//////// ↓(ii) velocityを変える場合↓ ////////
		///////////////////////////////////////////////

		/*
		// プレイヤーのリギッドボディのローカル速度locVelを取得
		Vector3 locVel = transform.InverseTransformDirection(rBody.velocity);

		// locVel.z：カメラから見てプレイヤーの左右
		if (Mathf.Abs(locVel.z) < moveLimitS && Mathf.Abs(hor) > float.Epsilon)
		{
			// locVel.z：プレイヤーの左右移動
			locVel.z += hor * moveS;			
		}

		// locVel.x：カメラから見てプレイヤーの奥行
		if (Mathf.Abs(locVel.x) < moveLimitS && Mathf.Abs(ver) > float.Epsilon)
		{
			// locVel.x：プレイヤーの奥行移動
			locVel.x -= ver * moveS;
		}

		// rBodyの速度を更新
		rBody.velocity = transform.TransformDirection(locVel);
		*/

		///////////////////////////////////////////////

		if (isGround && jump)
		{
			rBody.velocity += transform.up * jumpH;
		}
	}

	/*
	void TurnDirection(Vector3 move)
	{
		if (move.sqrMagnitude > float.Epsilon)
		{
			Vector3 scale = transform.localScale;
			float x = transform.localScale.x;

			if (move.x >= 0)
			{
				scale.x = -Mathf.Abs(x);
			}
			else
			{
				scale.x = Mathf.Abs(x);
			}

			transform.localScale = scale;
		}
	}
	*/

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
}
