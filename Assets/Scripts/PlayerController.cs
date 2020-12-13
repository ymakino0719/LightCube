﻿using System.Collections;
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
	bool lastGround = false;
	// ジャンプの高さ
	float jumpH = 6.0f;
	// アイテムを持っているか
	bool holding = false;
	// 移動スピード
	float moveS = 0.8f;
	// 移動上限速度
	float moveLimitS = 2.0f;
	//float walkAnimationSpeed = 0.01f;
	// Playerが動いているかどうかの閾値
	float judgeMoving = 0.2f;

	// 次に移動する面に移動中かどうか
	private bool control = true;

	// PlayerのAnimator
	Animator animator;

	// Start is called before the first frame update
	void Start()
    {
        rBody = GetComponent<Rigidbody>();
		// Playerの直下の一番上に配置しているYagikun3DにアタッチされているAnimatorを取得する
		animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
		if(control) // 次の面に移動中でないとき（通常時）
        {
			// 仮想重力をかけ続ける
			rBody.AddRelativeForce(-Vector3.up * gravity);

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

			MoveAnimation(jump, pick);
			MoveCharacter(hor, ver, jump);
			//TurnDirection(move);

			// lastGround（直前に着地していたか）の更新
			lastGround = (isGround) ? true : false;
		}
		else // 次の面に移動処理中のとき（EdgeInformationで処理しているとき）
		{

        }
	}
	void MoveAnimation(bool jump, bool pick)
	{
		if(isGround)
        {
			// 着地時且つジャンプ入力があるときjumpingBoolを有効にする
			if (jump)
			{
				animator.SetBool("jumpingBool", true);
			}
			else if (pick)
            {
				animator.SetBool("PickUpBool", true);
				//animator.SetBool("jumpingBool", false);
			}
			else
            {
				//animator.SetBool("jumpingBool", false);

				if (rBody.velocity.sqrMagnitude >= judgeMoving)
                {
					animator.SetFloat("movingSpeed", 1);
				}
				else
                {
					animator.SetFloat("movingSpeed", 0);
				}
            }
		}
		else
        {

		}

		/*
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
		*/
	}
	void MoveCharacter(float hor, float ver, bool jump)
	{
		///////////////////////
		//////// move /////////
		///////////////////////


		// プレイヤーのリギッドボディのローカル速度locVelを取得
		Vector3 locVel = transform.InverseTransformDirection(rBody.velocity);

		// locVel.z：カメラから見てプレイヤーの左右
		if ((hor > float.Epsilon && locVel.z < moveLimitS) || (hor < float.Epsilon && locVel.z > -moveLimitS))
		{
			// locVel.z：プレイヤーの左右移動
			rBody.AddForce(transform.forward * hor * moveS);
		}

		// locVel.x：カメラから見てプレイヤーの奥行
		if ((ver > float.Epsilon && locVel.x < moveLimitS) || (ver < float.Epsilon && locVel.x > -moveLimitS))
		{
			// locVel.x：プレイヤーの奥行移動
			rBody.AddForce(-transform.right * ver * moveS);
		}

		//Debug.Log("ver: " + ver);
		//Debug.Log("locVel.z: " + locVel.z);

		///////////////////////
		//////// jump /////////
		///////////////////////

		if (isGround && jump)
		{
			rBody.velocity += transform.up * jumpH;
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
}
