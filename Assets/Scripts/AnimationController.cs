﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
	// Yagikun3DのAnimator
	Animator animator;
	// PlayerController
	PlayerController pC;
	// Playerが動いているかどうかの閾値
	float judgeMoving = 0.01f;
	// AnimationのRunの速さパラメータ
	float runAnimSpeed = 1.0f;

	// 近くにあるitem
	GameObject nearestItem;

	// Start is called before the first frame update
	void Awake()
	{
		// Yagikun3DにアタッチされているAnimatorを取得する
		animator = GetComponent<Animator>();
		// PlayerControllerの取得
		pC = GameObject.Find("Player").GetComponent<PlayerController>();
	}

	public void MoveAnimation(bool jump, bool pick, bool holding, bool isGround, bool lastGround, Vector3 vec)
	{
		if (isGround)
		{
			if (jump) // 着地時且つジャンプ入力があるときjumpBoolを有効にする
			{
				animator.SetBool("jumpBool", true);
			}
			else if (pick)
			{
				animator.SetTrigger("pickUpTrigger");

				if (!holding)
				{
					animator.SetBool("holding", false);
				}
				else
				{
					animator.SetBool("holding", true);
				}
			}
			else
			{
				if (!lastGround)
				{
					animator.SetBool("jumpBool", false);
					animator.SetBool("jumpingBool", false);
				}

				//Debug.Log("RunSpeed : " + vel * runAnimSpeed);

				// 縦のY要素は不要のため0を代入
				vec.y = 0;
				// RunのAnimationSpeedの上限を1.0fにする
				float a = (vec.sqrMagnitude * runAnimSpeed <= 1.0f) ? vec.sqrMagnitude * runAnimSpeed : 1.0f;

				if (a >= judgeMoving)
				{
					animator.SetFloat("movingSpeed", a);
				}
				else
				{
					animator.SetFloat("movingSpeed", 0);
				}
			}
		}
		else
		{
			// ジャンプしていない状態でIsGroundのみがfalseになった場合は、自由落下とみなし滞空モーションに移行する（jumpingBoolを有効にする）
			if (lastGround)
			{
				animator.SetBool("jumpingBool", true);
			}
		}
	}

	public void VictoryAnimation()
    {
		float a = Random.Range(0.0f, 10.0f);

		animator.SetFloat("victoryValue", a);
		animator.SetTrigger("victoryTrigger");
	}

	void BringEvent() // PickUpアニメーションの最後と、PutDownアニメーションの手を放した瞬間に実行
	{
		if (!pC.Holding)
		{
			animator.SetBool("bring", true);
		}
		else
		{
			animator.SetBool("bring", false);
		}
		pC.Holding = !pC.Holding;
	}

	void PickUpEvent() // PickUpアニメーションのアイテムを掴んだ瞬間に実行
	{
		// Itemを掴む
		var iC = nearestItem.GetComponent<ItemsController>();
		iC.BeingHeld();
	}

	void PutDownEvent() // PutDownアニメーションの手を放した瞬間に実行
	{
		// Itemを放す
		var iC = nearestItem.GetComponent<ItemsController>();
		iC.NotBeingHeld();
	}

	void CantMovingEvent()
	{
		pC.Control = false;
	}
	void CanMovingEvent()
	{
		if(!pC.GameOver)
        {
			pC.Control = true;
		}
	}

	void StartPuttingDown()
	{
		var iC = nearestItem.GetComponent<ItemsController>();
		iC.PuttingDown = true;
	}

	void DisplayGameClearPanel()
    {
		GameObject.Find("UIDirector").GetComponent<GameClearUI>().DisplayGameClear();
	}
	void CompleteFlag()
	{
		var cJ = GameObject.Find("GameDirector").GetComponent<ClearJudgement>();
		cJ.GameOver02 = false;
		cJ.GameOver03 = true;
	}
	public GameObject NearestItem
	{
		set { nearestItem = value; }
		get { return nearestItem; }
	}
}