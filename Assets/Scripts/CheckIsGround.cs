﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIsGround : MonoBehaviour
{
	PlayerController pC;

	void Awake()
	{
		pC = transform.parent.GetComponent<PlayerController>();
	}

	// 着地判定
	void OnTriggerEnter(Collider collision)
	{
		// 着地判定
		if (!collision.gameObject.CompareTag("Face") && !collision.gameObject.CompareTag("Edge") && !collision.gameObject.CompareTag("Gate")) pC.IsGround = true;

		// スイッチを踏んだ時
		if (collision.gameObject.CompareTag("Switch")) collision.transform.parent.gameObject.GetComponent<SwitchBehavior>().CheckSwitchAndPlayerDir(collision.gameObject);
	}

	void OnTriggerStay(Collider collision)
	{
		// 着地判定
		if (!collision.gameObject.CompareTag("Face") && !collision.gameObject.CompareTag("Edge") && !collision.gameObject.CompareTag("Gate")) pC.IsGround = true;

		Debug.Log(collision.gameObject);
	}

	void OnTriggerExit(Collider collision)
	{
		// 着地判定
		if (!collision.gameObject.CompareTag("Face") && !collision.gameObject.CompareTag("Edge") && !collision.gameObject.CompareTag("Gate")) pC.IsGround = false;
	}
}
