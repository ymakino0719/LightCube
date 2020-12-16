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
	// 手に持った時のitemのPos
	GameObject bringingPos;

	// Start is called before the first frame update
	void Awake()
	{
		// Yagikun3DにアタッチされているAnimatorを取得する
		animator = GetComponent<Animator>();
		// PlayerControllerの取得
		pC = GameObject.Find("Player").GetComponent<PlayerController>();
		// BringingPosの取得
		bringingPos = GameObject.Find("BringingPos");

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

	void PickUpEvent()
    {
		animator.SetBool("bring", true);
		pC.Holding = true;

		// ※仮設定、後で変える
		nearestItem.transform.parent = bringingPos.transform;
		nearestItem.transform.localPosition = Vector3.zero;
		nearestItem.transform.localEulerAngles = Vector3.zero;
	}

	void PutDownEvent()
    {
		animator.SetBool("bring", false);
		pC.Holding = false;

		// ※仮設定、後で変える
		nearestItem.transform.parent = null;
	}

	void CantMovingEvent()
    {
		pC.Control = false;
    }
	void CanMovingEvent()
	{
		pC.Control = true;
	}
	public GameObject NearestItem
	{
		set { nearestItem = value; }
		get { return nearestItem; }
	}
}
