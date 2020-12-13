using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
	// Yagikun3DのAnimator
	Animator animator;
	// Playerが動いているかどうかの閾値
	float judgeMoving = 0.01f;
	// AnimationのRunの速さパラメータ
	float runAnimSpeed = 1.0f;

	// Start is called before the first frame update
	void Awake()
	{
		// Yagikun3DにアタッチされているAnimatorを取得する
		animator = GetComponent<Animator>();
	}

	public void MoveAnimation(bool jump, bool pick, bool isGround, float vel)
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
				animator.SetBool("PickUpBool", true);
				animator.SetBool("jumpingBool", false);
			}
			else
			{
				animator.SetBool("jumpingBool", false);

				//Debug.Log("RunSpeed : " + vel * runAnimSpeed);



				if (vel * runAnimSpeed >= judgeMoving)
				{
					// RunのAnimationSpeedの上限を1.0fにする
					float a = (vel * runAnimSpeed <= 1.0f) ? vel * runAnimSpeed : 1.0f;
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

    public void LookForward(float hor, float ver)
    {
		Vector3 diff = new Vector3(-ver, 0, hor);

		// ベクトルの大きさが0.01以上の時に向きを変える処理をする
		if (diff.sqrMagnitude > 0.01f)
		{
			transform.rotation = Quaternion.LookRotation(diff); // 向きを変更する
		}
	}
}
