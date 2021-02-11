using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AroundEdgeController : MonoBehaviour
{
	// 辺の固有番号（CubeInformationからプロパティで取得）
	private int edgeNum;

	// 次に移動する面
	private Vector3 nextFace;
	// 次にこのオブジェクトが移動する座標
	private Vector3 nextPos;
	// 次にこのオブジェクトが移動する座標へ向かうための中間点
	private Vector3 middlePos;

	// このオブジェクトの面移動前後の回転情報
	private Vector3 beforeR, afterR;
	// このオブジェクトの回転前のリギッドボディのローカル速度
	private Vector3 locVel;
	// このオブジェクトの回転後の速度情報
	private Vector3 nextVel;
	// このオブジェクトの初期位置
	private Vector3 currentPos;

	// このオブジェクトの移動・回転の開始
	private bool moving = false;
	// 初期位置～中間地点間の移動か、中間地点～最終地点間の移動かを判別するbool
	private bool toMiddle = true;
	// このオブジェクトを移動・回転させるときの時間（0～1）
	private float time = 0;
	// このオブジェクトを移動・回転させるときの時間の調整パラメータ
	private float timeCoef = 2.0f;

	private bool reset = false;

	private Rigidbody rBody;

	// プレイヤーの中心位置のTransform（nextFaceの判定に使用する）
	private Transform middlePosTra;

	// Edgeの接触判定を無効化し回転を禁止する
	bool stopEdgeEntering = false;

	// 射出単位ベクトル
	Vector3 injectionUnitVec;
	void Awake()
	{
		rBody = GetComponent<Rigidbody>();
		// プレイヤーの中心位置のTransformの取得（nextFaceの判定に使用する）
		middlePosTra = GameObject.Find("MiddlePos").transform;
	}

	void Update()
	{
		if (moving)
		{
			if (!reset)
			{
				MoveAndRotatePlayer(); // このオブジェクトの移動と回転
			}
			else
			{
				ResetPlayerVelocity(); // このオブジェクトの速度を再設定する
				RestartObject(); // 無効にしていたこのオブジェクトの仮想重力やキー入力操作、当たり判定等を有効に戻す

				reset = false;
				moving = false;
			}
		}
	}

	public void FromEdgesInformation(Vector3 edge, Vector3 vertex01, Vector3 vertex02, Vector3 face01, Vector3 face02)
    {
		StopObject();

		// 初期位置を取得
		currentPos = transform.position;

		// 面移動前の回転情報を取得
		beforeR = transform.eulerAngles;

		// 回転前のリギッドボディのローカル速度locVelを取得
		locVel = transform.InverseTransformDirection(rBody.velocity);

		//Debug.Log("locVel: " + locVel);

		// 回転前の速度を一旦無効にする
		rBody.velocity = Vector3.zero;

		CheckCloserFace(edge, face01, face02); // ２面の内、Playerからより遠い面を調べる（それがこのオブジェクトが次に移動する面となる）
		CheckNextPlayerPos(edge, vertex01, vertex02, face01, face02); // 軸回転を行い、このオブジェクトが次に移動する座標nextPosを調べ、取得する
		CheckMiddlePos(vertex01, vertex02); // このオブジェクトがnextPosまで移動するときの中間点middlePos（経由するEdgeあたりの座標）を調べる

		moving = true;
	}

	void StopObject()
    {
		if (gameObject.CompareTag("Player")) // Playerは当たり判定のあるPlayerにPlayerのタグ付けがされている
		{
			// 重力と操作の無効化
			GetComponent<PlayerController>().Control = false;

			// 当たり判定の無効化
			GetComponent<CapsuleCollider>().enabled = false;
			transform.Find("HitBox_side").GetComponent<CapsuleCollider>().enabled = false;
		}
		else if (transform.GetChild(0).gameObject.CompareTag("Item")) // Itemは当たり判定のある子オブジェクトにItemのタグ付けがされている
		{
			// 重力と操作と当たり判定の無効化
			GetComponent<ItemsController>().StartRotatingAroundEdge();
		}
	}

	void RestartObject()
    {
		if (gameObject.CompareTag("Player")) // Playerは当たり判定のあるPlayerにPlayerのタグ付けがされている
		{
			// 重力と操作の有効化
			GetComponent<PlayerController>().Control = true;

			// 当たり判定の有効化
			GetComponent<CapsuleCollider>().enabled = true;
			transform.Find("HitBox_side").GetComponent<CapsuleCollider>().enabled = true;

			// 当たり判定を再度入れなおした瞬間に同じEdgeに再衝突してしまう場合に備え、その入れなおした１フレームだけ接触判定を無効化し回転を禁止する
			StartCoroutine("ReCollisionInhibitionTime");
		}
		else if (transform.GetChild(0).gameObject.CompareTag("Item")) // Itemは当たり判定のある子オブジェクトにItemのタグ付けがされている
		{
			// 重力と操作と当たり判定の有効化
			GetComponent<ItemsController>().EndRotatingAroundEdge();

			// 当たり判定を再度入れなおした瞬間に同じEdgeに再衝突してしまう場合に備え、その入れなおした１フレームだけ接触判定を無効化し回転を禁止する
			StartCoroutine("ReCollisionInhibitionTime");
		}
	}
	IEnumerator ReCollisionInhibitionTime()
	{
		stopEdgeEntering = true;

		yield return new WaitForSeconds(0.1f);

		stopEdgeEntering = false;
	}
	void CheckCloserFace(Vector3 edge, Vector3 face01, Vector3 face02)
	{
		// Edgeを中心とした各２面とPlayerの位置から形成される角度を調べる
		// （原点位置（足元）からの角度計算の場合、プレイヤーの速度と関数処理のタイミング上エラーが起きる場合があったので、プレイヤーの中心位置（middlePosTra）からの角度計算に変更した）、
		float angle01 = Vector3.Angle(middlePosTra.position - edge, face01 - edge);
		float angle02 = Vector3.Angle(middlePosTra.position - edge, face02 - edge);

		//Debug.Log("angle01: " + angle01);
		//Debug.Log("angle02: " + angle02);

		// 角度がより大きい面を次に移動する面と定義する
		nextFace = (angle01 > angle02) ? face01 : face02;
		//Debug.Log("nextFace = " + nextFace);

		// 射出単位ベクトルの算出（ResetPlayerVelocity()で用いる）
		injectionUnitVec = (nextFace - edge).normalized;
	}

	void CheckNextPlayerPos(Vector3 edge, Vector3 vertex01, Vector3 vertex02, Vector3 face01, Vector3 face02)
	{
		// このオブジェクトをRotateAroundさせる（中心：辺の原点、軸：２つの頂点のベクトル、角度：辺を中心とした２面の原点が成す角度
		Vector3 axis = vertex01 - vertex02; // 回転軸
		float angle = Vector3.Angle(face01 - edge, face02 - edge); // 回転角度

		// 回転角度の正負が不明なため、両方試し、最終位置が次の面により近い方（織りなす角度がより小さい方）を採用する
		// まずangle度で回転させる
		transform.RotateAround(edge, axis, angle);
		// angle度で軸回転後のこのオブジェクトの座標を取得しておく
		Vector3 nextPos01 = transform.position;
		// 辺の原点位置を中心とした、移動後のこのオブジェクト（の中心；middlePosTra）とnextFaceの角度を調べる
		float angle01 = Vector3.Angle(middlePosTra.position - edge, nextFace - edge);
		// 更にRotateAroundで180度回転させたあとのEulerAngleを取得する
		Vector3 afterR01 = CheckRotation(edge, axis);

		// 次に-angle度で回転させる（一旦移動前の場所に戻すために-2 * angle度としている）
		transform.RotateAround(edge, axis, -2 * angle);
		// -angle度で軸回転後のこのオブジェクトの座標を取得しておく
		Vector3 nextPos02 = transform.position;
		// 辺の原点位置を中心とした、移動後のこのオブジェクト（の中心；middlePosTra）とnextFaceの角度を調べる
		float angle02 = Vector3.Angle(middlePosTra.position - edge, nextFace - edge);
		// 更にRotateAroundで180度回転させたあとのEulerAngleを取得する
		Vector3 afterR02 = CheckRotation(edge, axis);

		//Debug.Log("angle01: " + angle01);
		//Debug.Log("angle02: " + angle02);

		// このオブジェクトを初期位置に戻す（angle度で回転させる）
		transform.RotateAround(edge, axis, angle);

		// angle01と02を比較し、より小さい値の方のnextPosをこのオブジェクトが移動する座標先として採用する
		nextPos = (angle01 < angle02) ? nextPos01 : nextPos02;
		afterR = (angle01 < angle02) ? afterR01 : afterR02;		
	}

	Vector3 CheckRotation(Vector3 edge, Vector3 axis)
	{
		// RotateAroundで180度回転させたあとのEulerAngleを取得する
		transform.RotateAround(edge, axis, 180);
		Vector3 vec = transform.eulerAngles;
		// 元に戻す（-180度回転させる）
		transform.RotateAround(edge, axis, -180);

		return vec;
	}

	void CheckMiddlePos(Vector3 vertex01, Vector3 vertex02)
	{
		// このオブジェクトはEdgeのTriggerに接触した際、一度Edgeを経由して、最終地点nextPosへ向かう処理とする
		// そのため、ここでは経由する中間点middlePosを取得する

		// 回転軸に対しこのオブジェクトの初期位置から垂線を引き、交点をmiddlePosとして採用する
		middlePos = vertex01 + Vector3.Project(transform.position - vertex01, vertex02 - vertex01);
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
			// ①このオブジェクトをcurrentPosからmiddlePosまで移動させる
			transform.position = Vector3.Slerp(currentPos, middlePos, time);

			// このオブジェクトを回転量の半分だけ回転させる
			transform.rotation = Quaternion.Slerp(bR, aR, time / 2);

			if (time >= 1)
			{
				transform.position = middlePos;
				time = 0;
				toMiddle = false;
			}
		}
		else
		{
			// ②このオブジェクトをmiddlePosからnextPosまで移動させる
			transform.position = Vector3.Slerp(middlePos, nextPos, time);

			// このオブジェクトを回転量のもう半分だけ回転させる
			transform.rotation = Quaternion.Slerp(bR, aR, 0.5f + time / 2);

			if (time >= 1)
			{
				transform.position = nextPos;
				transform.eulerAngles = afterR;
				time = 0;
				reset = true;
				toMiddle = true;
			}
		}
	}

	void ResetPlayerVelocity()
	{
		Vector3 vec = transform.TransformDirection(locVel);
		// 射出の際、その方向にベクトルを加算する（1.2fでEdge側にめり込まないようになったため、安全を見て1.35fとした）
		Vector3 addedVec = vec + injectionUnitVec * 1.35f;
		rBody.velocity = addedVec;
		
	}
	public bool StopEdgeEntering
	{
		set { stopEdgeEntering = value; }
		get { return stopEdgeEntering; }
	}
}
