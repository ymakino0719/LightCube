using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIsGround : MonoBehaviour
{
	PlayerController pC;

	void Awake()
	{
		pC = transform.parent.GetComponent<PlayerController>();
	}

	void OnTriggerEnter(Collider collision)
	{
		pC.IsGround = true;
	}

	void OnTriggerStay(Collider collision)
	{
		pC.IsGround = true;
	}

	void OnTriggerExit(Collider collision)
	{
		pC.IsGround = false;
	}
}
