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

	// 辺の原点位置のtransform
	private Vector3 edge;

	// 次に移動する面
	private Vector3 nextFace;
	// 次にプレイヤーが移動する座標
	private Vector3 nextPos;
	// 次にプレイヤーが移動する座標へ向かうための中間点
	private Vector3 middlePos;

	// プレイヤーの面移動前後の回転情報
	private Vector3 beforeR, afterR;
	// プレイヤーの回転前の速度情報
	private Vector3 rVel;
	// プレイヤーの初期位置
	private Vector3 currentPos;

	// プレイヤーの移動・回転の開始
	private bool moving = false;
	// 初期位置～中間地点間の移動か、中間地点～最終地点間の移動かを判別するbool
	private bool toMiddle = true;
	// プレイヤーを移動・回転させるときの時間（0～1）
	private float time = 0;
	// プレイヤーを移動・回転させるときの時間の調整パラメータ
	private float timeCoef = 1.5f;
	//
	private bool reset = false;

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

		// 辺の原点位置のtransformを取得
		edge = transform.position;
	}

	void Update()
    {
		if(moving)
        {
			if(!reset)
            {
				MoveAndRotatePlayer(); // Playerの移動と回転
			}
			else
            {
				ResetPlayerVelocity(); // Playerの速度を再設定する

				// 無効にしていたプレイヤーの仮想重力やキー入力操作等を有効に戻す
				pC.Control = true;

				moving = false;
				reset = false;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			// プレイヤーの仮想重力やキー入力操作等を無効にする
			pC.Control = false;

			// プレイヤーの面移動前の回転情報を取得
			beforeR = player.transform.eulerAngles;

			// プレイヤーの回転前の速度情報を取得
			rVel = rBody.velocity;

			// プレイヤーの初期位置を取得
			currentPos = player.transform.position;

			CheckCloserFace(); // ２面の内、Playerからより遠い面を調べる（それがPlayerが次に移動する面となる）
			CheckNextPlayerPos(); // 軸回転を行い、Playerが次に移動する座標nextPosを調べ、取得する
			CheckMiddlePos(); // PlayerがnextPosまで移動するときの中間点middlePos（経由するEdgeあたりの座標）を調べる

			moving = true;
		}
	}

	void CheckCloserFace()
	{
		// Edgeを中心とした各２面とPlayerの位置から形成される角度を調べる
		float angle01 = Vector3.Angle(player.transform.position - edge, face[0] - edge);
		float angle02 = Vector3.Angle(player.transform.position - edge, face[1] - edge);

		// 角度がより大きい面を次に移動する面と定義する
		nextFace = (angle01 > angle02) ? face[0] : face[1];
	}

	void CheckNextPlayerPos()
	{
		// PlayerをRotateAroundさせる（中心：辺の原点、軸：２つの頂点のベクトル、角度：辺を中心とした２面の原点が成す角度
		Vector3 axis = vertex[0] - vertex[1]; // 回転軸
		float angle = Vector3.Angle(face[0] - edge, face[1] - edge); // 回転角度

		// 回転角度の正負が不明なため、両方試し、最終位置が次の面により近い方（織りなす角度がより小さい方）を採用する
		// まずangle度で回転させる
		player.transform.RotateAround(edge, axis, angle);
		// angle度で軸回転後のPlayerの座標を取得しておく
		Vector3 nextPos01 = player.transform.position;
		// 辺の原点位置を中心とした移動後のPlayerとnextFaceの角度を調べる
		float angle01 = Vector3.Angle(player.transform.position - edge, nextFace - edge);
		// 更にRotateAroundで180度回転させたあとのEulerAngleを取得する
		Vector3 afterR01 = CheckRotation(edge, axis);

		// 次に-angle度で回転させる（一旦移動前の場所に戻すために-2 * angle度としている）
		player.transform.RotateAround(edge, axis, -2 * angle);
		// -angle度で軸回転後のPlayerの座標を取得しておく
		Vector3 nextPos02 = player.transform.position;
		// 辺の原点位置を中心とした移動後のPlayerとnextFaceの角度を調べる
		float angle02 = Vector3.Angle(player.transform.position - edge, nextFace - edge);
		// 更にRotateAroundで180度回転させたあとのEulerAngleを取得する
		Vector3 afterR02 = CheckRotation(edge, axis);

		//Debug.Log("angle01: " + angle01);
		//Debug.Log("angle02: " + angle02);

		// Playerを初期位置に戻す（angle度で回転させる）
		player.transform.RotateAround(edge, axis, angle);

		// angle01と02を比較し、より小さい値の方のnextPosをPlayerが移動する座標先として採用する
		nextPos = (angle01 < angle02) ? nextPos01 : nextPos02;
		afterR = (angle01 < angle02) ? afterR01 : afterR02;

		if(angle01 < angle02)
        {
			Debug.Log("angle01 < angle02, angle01 = " + angle01 + ", afterR01 = " + afterR01 + ", afterR02 = " + afterR02);
        }
		else
        {
			Debug.Log("angle01 > angle02, angle02 = " + angle02 + ", afterR01 = " + afterR01 + ", afterR02 = " + afterR02);
		}

		//Debug.Log("afterR01: " + afterR01);
		//Debug.Log("afterR02: " + afterR02);

		//Debug.Log("beforeR: " + beforeR);
		//Debug.Log("afterR: " + afterR);
	}

	Vector3 CheckRotation(Vector3 edge, Vector3 axis)
    {
		// RotateAroundで180度回転させたあとのEulerAngleを取得する
		player.transform.RotateAround(edge, axis, 180);
		Vector3 vec = player.transform.eulerAngles;
		// 元に戻す（-180度回転させる）
		player.transform.RotateAround(edge, axis, -180);

		/*
		// 現在の回転情報を取得する
		Quaternion q = player.transform.rotation;
		// 回転軸axis周りに180度回転させる
		Quaternion rot = Quaternion.AngleAxis(180, axis);
		player.transform.rotation = q * rot;

		// プレイヤーのEulerAnglesを取得する
		Vector3 vec = player.transform.eulerAngles;

		// 元に戻す
		player.transform.rotation = q;
		*/

		return vec;
	}

	void CheckMiddlePos()
    {
		// プレイヤーはEdgeのTriggerに接触した際、一度Edgeを経由して、最終地点nextPosへ向かう処理とする
		// そのため、ここでは経由する中間点middlePosを取得する

		// 回転軸に対しプレイヤーの初期位置から垂線を引き、交点をmiddlePosとして採用する
		middlePos = vertex[0] + Vector3.Project(player.transform.position - vertex[0], vertex[1] - vertex[0]);
	}

	void MoveAndRotatePlayer()
	{
		// timeに時間を加算する
		time += timeCoef * Time.deltaTime;
		
		// beforeRとafterRをQuaternionで定義しておく
		Quaternion bR = Quaternion.Euler(beforeR);
		Quaternion aR = Quaternion.Euler(afterR);

		//Debug.Log("afterR: " + afterR);
		//Debug.Log("aR: " + aR);

		if (toMiddle) 
		{
			// ①プレイヤーをcurrentPosからmiddlePosまで移動させる
			player.transform.position = Vector3.Lerp(currentPos, middlePos, time);

			// プレイヤーを回転量の半分だけ回転させる
			player.transform.rotation = Quaternion.Lerp(bR, aR, time / 2);

			if(time >= 1)
            {
				player.transform.position = middlePos;
				time = 0;
				toMiddle = false;
            }
		}
		else 
		{
			// ②プレイヤーをmiddlePosからnextPosまで移動させる
			player.transform.position = Vector3.Lerp(middlePos, nextPos, time);

			// プレイヤーを回転量のもう半分だけ回転させる
			player.transform.rotation = Quaternion.Lerp(bR, aR, 0.5f + time / 2);

			if (time >= 1)
			{
				player.transform.position = nextPos;
				player.transform.eulerAngles = afterR;
				time = 0;
				reset = true;
				toMiddle = true;
			}
		}
	}

	void ResetPlayerVelocity()
	{	
		// プレイヤーの速度情報を更新（ベクトルの回転）
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
