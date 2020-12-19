﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsController : MonoBehaviour
{
    // パズルとして正解の位置にあるかどうか
    bool puzzle = false;
    // パズルとして正解の位置に移動する際中かどうか
    bool putKey01 = false;
    bool putKey02 = false;
    // 衝突したLockBlockの位置・回転・拡大率情報
    Vector3 lockPos, lockRot, lockSca;
    // LockBlockと衝突した瞬間のこのオブジェクトの位置・回転・拡大率情報
    Vector3 keyPos, keyRot, keySca;

    // このオブジェクトを移動・回転させるときの時間（0～1）
    private float timeX = 0;
    // このオブジェクトを移動・回転させるときの時間の調整パラメータ
    private float timeCoef = 0.8f;

    // ItemsのRigidbody
    Rigidbody rBody;
    // 仮想重力の係数
    float gravity = 2.0f;
    // 移動上限速度
    float maxSpeed = 5.0f;
    // 移動最低速度
    float minSpeed = 0.01f;
    // 移動速度減衰率
    float dampingRatio = 0.97f;

    // ItemsのCollider
    Collider col;
    // プレイヤーに持たれているかどうか
    bool held = false;
    // edgeを回転中かどうか
    bool rotating = false;
    // 置いている最中かどうか
    bool puttingDown = false;

    // 回転させる子オブジェクト
    GameObject child;
    // yagikun3Dのゲームオブジェクト
    GameObject yagikun3D;
    // 手に持った時のitemのPos
    GameObject bringingPos;

    void Awake()
    {
        // ItemにアタッチされているRigidbodyを取得する
        rBody = GetComponent<Rigidbody>();
        // BringingPosの取得
        bringingPos = GameObject.Find("BringingPos");
        // Itemの直下に配置している子オブジェクトを取得する
        child = transform.GetChild(0).gameObject;
        // Itemの直下に配置している子オブジェクトのBoxColliderを取得する
        col = child.GetComponent<BoxCollider>();
        // yagikun3Dのゲームオブジェクトを取得する
        yagikun3D = GameObject.Find("Yagikun3D");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(rBody.velocity);

        if(!puzzle && !held && !rotating)
        {
            ItemsGravityControll();
        }

        if(puttingDown)
        {
            PuttingDownRotation();
        }

        if(putKey01 || putKey02)
        {
            // timeXに時間を加算する
            timeX += timeCoef * Time.deltaTime;

            // 移動・回転に使用するtimeX01の定義
            float timeX01 = (timeX <= 1) ? timeX : 1;
            // 拡大に使用するtimeX02の定義
            float timeX02 = (timeX <= 2) ? timeX * 0.5f : 1;

            // 移動・回転に使用するtimeX01の定義: y = -x(x-2)
            float timeY01 = -timeX01 * (timeX01 - 2);
            // 拡大に使用するtimeX02の定義      : y = -x(x-2)
            float timeY02 = -timeX02 * (timeX02 - 2);

            if (putKey01)
            {
                PutKeys_ChangePosAndRot(timeY01);
                if (timeX01 == 1) putKey01 = false;
                Debug.Log("putKey01");
            }

            if (putKey02)
            {
                PutKeys_ChangeSca(timeY02);
                if (timeX02 == 1) putKey01 = false;
                Debug.Log("putKey02");
            } 

            if (!putKey01 && !putKey02)
            {
                timeX = 0;
            }
        }
    }

    void ItemsGravityControll()
    {
        if (rBody.velocity.sqrMagnitude < maxSpeed) // 制限速度を超過していない場合、ローカル座標系で見て垂直方向（Y軸）に仮想重力をかける
        {
            // 仮想重力をかけ続ける
            rBody.AddForce(-transform.up * gravity);
        }
        else // 制限速度を超過していた場合、ローカル座標系で見て水平方向（XZ軸）の速度を減衰させる、仮想重力もかけない
        {
            // プレイヤーのリギッドボディのローカル速度locVelを取得
            Vector3 locVel = transform.InverseTransformDirection(rBody.velocity);

            // 水平方向（XZ軸）の速度を減衰させる
            if (Mathf.Abs(locVel.x) >= minSpeed) locVel.x *= dampingRatio;
            if (Mathf.Abs(locVel.z) >= minSpeed) locVel.z *= dampingRatio;

            rBody.velocity = transform.TransformDirection(locVel);
        }
    }

    void PuttingDownRotation()
    {
        child.transform.eulerAngles = yagikun3D.transform.eulerAngles;
    }
    public void StartRotatingAroundEdge()
    {
        rotating = true;
    }
    public void EndRotatingAroundEdge()
    {
        rotating = false;
    }

    public void BeingHeld()
    {
        // プレイヤーに抱えられているときはItemsの移動速度、あたり判定及び仮想重力をなくす
        rBody.isKinematic = true;
        col.enabled = false;
        held = true;

        // Itemの位置を腕に来るようにする
        transform.parent = bringingPos.transform;
        transform.localPosition = Vector3.zero;
        // 回転情報はItemの子オブジェクトが受け持つ
        child.transform.eulerAngles = bringingPos.transform.eulerAngles;
    }

    public void NotBeingHeld()
    {
        // プレイヤーがItemsを手放した時、あたり判定及び仮想重力を戻す
        rBody.isKinematic = false;
        col.enabled = true;
        held = false;

        // Itemを放す
        transform.parent = null;

        // Itemを手放した直後の回転処理
        LetGoOfItemsRotation();
        puttingDown = false;
    }
    void LetGoOfItemsRotation()
    {
        // 子オブジェクトの回転情報を記録する
        Vector3 vec = child.transform.eulerAngles;
        // Itemの回転情報を、yagikun3Dの親オブジェクトであるPlayerの回転情報と一致させる
        transform.eulerAngles = yagikun3D.transform.parent.gameObject.transform.eulerAngles;
        // 子オブジェクトの回転情報を更新する
        child.transform.eulerAngles = vec;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Lock")) // Lockのタグが付いたオブジェクトのみを対象とする
        {
            // Itemの物理挙動及び当たり判定を無効化する
            rBody.isKinematic = true;
            col.enabled = false;

            // 衝突したLockBlockの位置・回転・拡大率情報を記録する
            lockPos = other.transform.position;
            lockRot = other.transform.eulerAngles;
            lockSca = other.transform.localScale;

            //Debug.Log("lockPos: " + lockPos);
            //Debug.Log("lockRot: " + lockRot);
            //Debug.Log("lockSca: " + lockSca);

            // LockBlockと衝突した瞬間のこのオブジェクトの位置・回転・拡大率情報を記録する
            keyPos = transform.position;
            keyRot = child.transform.eulerAngles;
            keySca = child.transform.localScale;

            //Debug.Log("keyPos: " + keyPos);
            //Debug.Log("keyRot: " + keyRot);
            //Debug.Log("keySca: " + keySca);

            // 移動、回転の開始
            putKey01 = true;
            // 拡大の開始
            putKey02 = true;
        }
    }

    void PutKeys_ChangePosAndRot(float timeY01)
    {
        transform.position = Vector3.Lerp(keyPos, lockPos, timeY01);
        child.transform.eulerAngles = Vector3.Lerp(keyRot, lockRot, timeY01);
    }
    void PutKeys_ChangeSca(float timeY02)
    {
        child.transform.localScale = Vector3.Lerp(keySca, lockSca, timeY02);
    }

    public bool PuttingDown
    {
        set { puttingDown = value; }
        get { return puttingDown; }
    }
}
