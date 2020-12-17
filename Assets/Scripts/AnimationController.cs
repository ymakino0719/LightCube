using System.Collections;
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
			// 着地時且つジャンプ入力があるときjumpingBoolを有効にする
			if (jump)
			{
				animator.SetBool("jumpingBool", true);
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
				if(!lastGround)
                {
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

		}
	}

	void BringEvent() // PickUpアニメーションの最後と、PutDownアニメーションの手を放した瞬間に実行
    {
		if(!pC.Holding)
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
		pC.Control = true;
	}

	void StartPuttingDown()
    {
		var iC = nearestItem.GetComponent<ItemsController>();
		iC.PuttingDown = true;
	}
	public GameObject NearestItem
	{
		set { nearestItem = value; }
		get { return nearestItem; }
	}
}
