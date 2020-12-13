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
		if (!collision.gameObject.CompareTag("Face")) pC.IsGround = true;		
	}

	void OnTriggerStay(Collider collision)
	{
		if (!collision.gameObject.CompareTag("Face")) pC.IsGround = true;
	}

	void OnTriggerExit(Collider collision)
	{
		if (!collision.gameObject.CompareTag("Face")) pC.IsGround = false;
	}
}
