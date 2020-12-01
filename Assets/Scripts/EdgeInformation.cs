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
	[System.NonSerialized] public Vector3[] vertex = new Vector3[2];
	[System.NonSerialized] public Vector3[] face = new Vector3[2];

	// 次に移動する面
	private Vector3 nextFace;

	// Playerの定義
	private GameObject player;
	private Rigidbody rBody;
	private PlayerController pC;

	void Awake()
	{
		cI = GameObject.Find("Information").GetComponent<CubeInformation>();
		player = GameObject.Find("Player");
		rBody = player.GetComponent<Rigidbody>();
		pC = player.GetComponent<PlayerController>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			// プレイヤーの仮想重力やキー入力操作等を無効にする
			//pC.Control = false;

			// 辺の原点位置のtransformを取得する
			Vector3 edge = transform.position;

			// プレイヤーの回転前の速度情報を取得
			Vector3 rVel = rBody.velocity;

			// プレイヤーの面移動後の回転情報を取得
			Vector3 beforeR = player.transform.eulerAngles;

			CheckCloserFace(edge); // ２面の内、Playerからより遠い面を調べる（それがPlayerが次に移動する面となる）
			RotateAroundAndChecking(edge); // 軸回転を行い、Playerを次の面に移動させる
			RotatePlayer(beforeR); // Playerをローカル座標系でZ方向に180度回転させる
			ResetPlayerVelocity(beforeR, rVel); // Playerの速度を再設定する
		}
	}

	void CheckCloserFace(Vector3 edge)
	{
		// Edgeを中心とした各２面とPlayerの位置から形成される角度を調べる
		float angle01 = Vector3.Angle(player.transform.position - edge, face[0] - edge);
		float angle02 = Vector3.Angle(player.transform.position - edge, face[1] - edge);

		// 角度がより大きい面を次に移動する面と定義する
		nextFace = (angle01 > angle02) ? face[0] : face[1];
	}

	void RotateAroundAndChecking(Vector3 edge)
	{
		// PlayerをRotateAroundさせる（中心：辺の原点、軸：２つの頂点のベクトル、角度：辺を中心とした２面の原点が成す角度
		Vector3 axis = vertex[0] - vertex[1]; // 回転軸
		float angle = Vector3.Angle(face[0] - edge, face[1] - edge); // 回転角度

		// 回転角度の正負が不明なため、両方試し、最終位置が次の面により近い方（織りなす角度がより小さい方）を採用する
		// まずangle度で回転させる
		player.transform.RotateAround(edge, axis, angle);
		// 辺の原点位置を中心とした移動後のPlayerとnextFaceの角度を調べる
		float angle01 = Vector3.Angle(player.transform.position - edge, nextFace - edge);

		// 次に-angle度で回転させる（一旦移動前の場所に戻すために-2 * angle度としている）
		player.transform.RotateAround(edge, axis, -2 * angle);
		// 辺の原点位置を中心とした移動後のPlayerとnextFaceの角度を調べる
		float angle02 = Vector3.Angle(player.transform.position - edge, nextFace - edge);

		// angle01のほうが小さかった場合、angle01が正のため、更に2 * angleで回転しなおす
		if (angle01 < angle02)
		{
			player.transform.RotateAround(edge, axis, -2 * angle);
		}
	}

	void RotatePlayer(Vector3 beforeR)
	{
		// プレイヤーの面移動後の回転を調べる
		Vector3 afterR = player.transform.eulerAngles;

		// 回転前後の回転情報を取得する
		Vector3 vec = afterR - beforeR;

		// 回転方向を元に戻すため、-2倍する
		player.transform.Rotate(-2 * vec);
	}

	void ResetPlayerVelocity(Vector3 beforeR, Vector3 rVel)
	{
		// プレイヤーの面移動後の回転を調べる
		Vector3 afterR = player.transform.eulerAngles;

		// プレイヤーの速度情報を更新
		Vector3 vec = Quaternion.Euler(afterR - beforeR) * rVel;
		rBody.velocity = vec;

		//Debug.Log("vec: " + vec);
		
	}

	public int EdgeNum
	{
		set { edgeNum = value; }
		get { return edgeNum; }
	}
}
