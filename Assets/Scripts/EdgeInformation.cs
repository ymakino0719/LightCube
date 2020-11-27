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

	CubeInformation cI;

	// 辺に接する２つの頂点と２つの面の位置
	private Vector3 vertex01, vertex02;
	private Vector3 face01, face02;

	// 次に移動する面
	private Vector3 nextFace;

	void Awake()
	{
		cI = GameObject.Find("Information").GetComponent<CubeInformation>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			// otherからPlayerのTransformを取得する
			Transform playerTra = other.gameObject.transform;

			// 辺の原点位置のtransformを取得する
			Vector3 edge = transform.position;

			GetTransformInformation(); // 辺に接する２面と２頂点のtransform情報を入手する
			CheckCloserFace(playerTra, edge); // ２面の内、Playerからより遠い面を調べる（それがPlayerが次に移動する面となる）
			RotateAroundAndChecking(playerTra, edge); // 軸回転を行い、Playerを次の面に移動させる
			RotatePlayerZ(playerTra); // Playerをローカル座標系でZ方向に180度回転させる
			ResetPlayerVelocity(other); // Playerの速度を0にする
		}
	}

	void GetTransformInformation()
	{
		// vertexList_FromEdge[edgeNum]の要素数は２であるという前提（でなければ、探索距離cI.searchDis_vertexに異常）
		vertex01 = cI.vertexList_FromEdge[edgeNum][0].transform.position;
		vertex02 = cI.vertexList_FromEdge[edgeNum][1].transform.position;

		// faceList_FromEdge[edgeNum]の要素数は２であるという前提（でなければ、探索距離cI.searchDis_faceに異常）
		face01 = cI.faceList_FromEdge[edgeNum][0].transform.position;
		face02 = cI.faceList_FromEdge[edgeNum][1].transform.position;
	}

	void CheckCloserFace(Transform playerTra, Vector3 edge)
	{
		// Edgeを中心とした各２面とPlayerの位置から形成される角度を調べる
		float angle01 = Vector3.Angle(playerTra.position - edge, face01 - edge);
		float angle02 = Vector3.Angle(playerTra.position - edge, face02 - edge);

		// 角度がより大きい面を次に移動する面と定義する
		nextFace = (angle01 < angle02) ? face01 : face02;
	}

	void RotateAroundAndChecking(Transform playerTra, Vector3 edge)
	{
		// PlayerをRotateAroundさせる（中心：辺の原点、軸：２つの頂点のベクトル、角度：辺を中心とした２面の原点が成す角度
		Vector3 axis = vertex02 - vertex01; // 回転軸
		float angle = Vector3.Angle(face01 - edge, face02 - edge); // 回転角度

		// 回転角度の正負が不明なため、両方試し、最終位置が次の面により近い方（織りなす角度がより小さい方）を採用する
		// まずangle度で回転させる
		playerTra.RotateAround(edge, axis, angle);
		// 辺の原点位置を中心とした移動後のPlayerとnextFaceの角度を調べる
		float angle01 = Vector3.Angle(playerTra.position - edge, nextFace - edge);

		// 次に-angle度で回転させる（一旦移動前の場所に戻すために-2 * angle度としている）
		playerTra.RotateAround(edge, axis, -2 * angle);
		// 辺の原点位置を中心とした移動後のPlayerとnextFaceの角度を調べる
		float angle02 = Vector3.Angle(playerTra.position - edge, nextFace - edge);

		// angle01のほうが小さかった場合、angle01が正のため、更に2 * angleで回転しなおす
		if (angle01 < angle02)
		{
			playerTra.RotateAround(edge, axis, -2 * angle);
		}
	}

	void RotatePlayerZ(Transform playerTra)
	{
		playerTra.Rotate(0, 0, 180.0f);
	}

	void ResetPlayerVelocity(Collider other)
	{
		other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	public int EdgeNum
	{
		set { edgeNum = value; }
		get { return edgeNum; }
	}
}
