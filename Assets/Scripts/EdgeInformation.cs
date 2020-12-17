using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EdgeInformation : MonoBehaviour
{
	// 辺の固有番号（CubeInformationからプロパティで取得）
	private int edgeNum;

	// 辺に接する２つの頂点と２つの面の位置
	[System.NonSerialized] public Vector3[] vertex = new Vector3[2];
	[System.NonSerialized] public Vector3[] face = new Vector3[2];

	// 辺の原点位置のtransform
	private Vector3 edge;

	void Start()
	{
		// 辺の原点位置のtransformを取得
		edge = transform.position;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Item"))
		{
			var aEC = other.gameObject.GetComponent<AroundEdgeController>();
			aEC.FromEdgesInformation(edge, vertex[0], vertex[1], face[0], face[1]);
		}
	}

	public int EdgeNum
	{
		set { edgeNum = value; }
		get { return edgeNum; }
	}
}
