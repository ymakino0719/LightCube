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
	[System.NonSerialized] public GameObject[] vertex = new GameObject[2];
	[System.NonSerialized] public GameObject[] face = new GameObject[2];

	// 辺の原点位置のtransform
	private Vector3 edge;

	// SFXPlayer
	SFXPlayer sfx_P;

	void Awake()
    {
		sfx_P = GameObject.FindWithTag("Player").GetComponent<SFXPlayer>();
	}
	void Start()
	{
		// 辺の原点位置のtransformを取得
		edge = transform.position;
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			var aEC = other.gameObject.GetComponent<AroundEdgeController>(); // PlayerはPlayerにAroundEdgeControllerがアタッチされている

			if(!aEC.StopEdgeEntering)
            {
				sfx_P.PlaySFX(1); // 効果音を鳴らす
				aEC.FromEdgesInformation(edge, vertex[0].transform.position, vertex[1].transform.position, face[0].transform.position, face[1].transform.position);
			}
		}
		else if (other.gameObject.CompareTag("Item"))
		{
			var aEC = other.transform.parent.gameObject.GetComponent<AroundEdgeController>(); // Itemは親オブジェクトにAroundEdgeControllerがアタッチされている
			
			if(!aEC.StopEdgeEntering) // 回転終了直後のEdge再衝突でない場合
			{
				aEC.FromEdgesInformation(edge, vertex[0].transform.position, vertex[1].transform.position, face[0].transform.position, face[1].transform.position);
			}
		}
	}
	public int EdgeNum
	{
		set { edgeNum = value; }
		get { return edgeNum; }
	}
}
