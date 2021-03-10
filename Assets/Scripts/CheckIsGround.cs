using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIsGround : MonoBehaviour
{
	// Player
	GameObject player;
	// PlayerController
	PlayerController pC;
	// PlayerのRigidbody
	Rigidbody rBody;

	// 上昇中
	bool rising = false;
	// 上昇速度閾値
	float risingSpeedThreshold = 0.01f;
	void Awake()
	{
		player = transform.parent.gameObject;
		pC = player.GetComponent<PlayerController>();
		rBody = player.GetComponent<Rigidbody>();
	}
	void Update()
    {
		// プレイヤーのリギッドボディのローカル速度locVelを取得
		Vector3 locVel = player.transform.InverseTransformDirection(rBody.velocity);

		// 上昇中かどうか
		rising = (locVel.y >= risingSpeedThreshold) ? true : false;
	}

	// 着地判定
	void OnTriggerEnter(Collider collision)
	{
		// 着地判定（上昇中は着地できない）
		if (!rising && JudgeGroundOrNot(collision.gameObject)) pC.IsGround = true;

		// スイッチを踏んだ時
		if (!rising && collision.gameObject.CompareTag("Switch")) collision.transform.parent.gameObject.GetComponent<SwitchBehavior>().CheckSwitchAndPlayerDir(collision.gameObject);
	}

	void OnTriggerStay(Collider collision)
	{
		// 着地判定（上昇中は着地できない）
		if (!rising && JudgeGroundOrNot(collision.gameObject)) pC.IsGround = true;
	}

	void OnTriggerExit(Collider collision)
	{
		// 着地判定
		if (JudgeGroundOrNot(collision.gameObject)) pC.IsGround = false;
	}
	bool JudgeGroundOrNot(GameObject gao)
    {
		bool isGround;

		if (gao.CompareTag("Face") || gao.CompareTag("Edge") || gao.CompareTag("Gate") || gao.CompareTag("MiddlePoint")) isGround = false;
		else isGround = true;

		return isGround;
	}
}
