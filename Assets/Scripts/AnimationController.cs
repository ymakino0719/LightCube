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

	public void MoveAnimation(bool jump, int jumpNum, int jumpNum_Max, bool pick, bool holding, bool isGround, bool lastGround, Vector3 vec)
	{
		if (isGround)
		{
			// 着地時且つジャンプ入力があるときjumpBoolを有効にする
			if (jump) animator.SetBool("jumpBool", true);
			else if (pick)
			{
				animator.SetTrigger("pickUpTrigger");

				if (!holding) animator.SetBool("holding", false);
				else animator.SetBool("holding", true);
			}
			else
			{
				if (!lastGround)
				{
					// ジャンプboolを元に戻す
					animator.SetBool("jumpBool", false);
					animator.SetBool("doubleJumpBool", false);
					animator.SetBool("jumpingBool", false);
				}

				// 縦のY要素は不要のため0を代入
				vec.y = 0;
				// RunのAnimationSpeedの上限を1.0fにする
				float a = (vec.sqrMagnitude * runAnimSpeed <= 1.0f) ? vec.sqrMagnitude * runAnimSpeed : 1.0f;

				if (a >= judgeMoving) animator.SetFloat("movingSpeed", a);
				else animator.SetFloat("movingSpeed", 0);
			}
		}
		else
		{
			if (jump && jumpNum > 0 && jumpNum < jumpNum_Max) // 滞空中にジャンプ入力があり、残りジャンプ回数が０より大きく最大数より小さい場合、空中ジャンプする
			{
				animator.SetBool("doubleJumpBool", true);
			}
			else if (lastGround) // ジャンプしていない状態でIsGroundのみがfalseになった場合は、自由落下とみなし滞空モーションに移行する（jumpingBoolを有効にする）
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
		if (!pC.Holding) animator.SetBool("bring", true);
		else animator.SetBool("bring", false);

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
		if (!pC.GameOver) pC.Control = true;
	}

	void StartPuttingDown()
	{
		var iC = nearestItem.GetComponent<ItemsController>();
		iC.PuttingDown = true;
	}

	void DisplayGameClearPanel()
    {
		GameObject.Find("UIDirector").GetComponent<StageUI>().DisplayGameClear();
	}
	void CompleteFlag()
	{
		var cJ = GameObject.Find("GameDirector").GetComponent<ClearJudgement>();
		cJ.GameOver02 = false;
		cJ.GameOver03 = true;
	}
	void ResetDoubleJumpBool()
	{
		animator.SetBool("doubleJumpBool", false);
	}
	void StartAerialJumpRot()
    {
		// 既に回転中の場合は初期化する
		if (pC.AerialJumpRot) pC.ResetAerialJumpRotation();

		pC.AerialJumpRot = true;
	}
	public GameObject NearestItem
	{
		set { nearestItem = value; }
		get { return nearestItem; }
	}
}