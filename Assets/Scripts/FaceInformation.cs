using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceInformation : MonoBehaviour
{
	// 辺の固有番号（CubeInformationからプロパティで取得）
	private int faceNum;
	// 面に接する４つの辺の位置
	[System.NonSerialized] public Vector3[] edge = new Vector3[4];

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			// PlayerControllerに今いる（直前までいた）FaceのGameObjectを渡す
			other.gameObject.GetComponent<PlayerController>().CurrentFace = this.gameObject;
		}
	}
	public int FaceNum
	{
		set { faceNum = value; }
		get { return faceNum; }
	}
}
