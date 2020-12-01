using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    // PlayerのGameObject
    GameObject player;
    // CamPosのGameObject
    GameObject camPos;

    // Start is called before the first frame update
    void Start()
    {
        // playerの取得
        player = GameObject.Find("Player");
        // camPosの取得
        camPos = GameObject.Find("CamPos");
    }

    // Update is called once per frame
    void Update()
    {
        // カメラの移動
        transform.position = camPos.transform.position;
        // カメラの視点: playerの方を向く（playerの頭が上になるように）
        Quaternion rotation = Quaternion.LookRotation(player.transform.position - camPos.transform.position, camPos.transform.up);
        transform.rotation = rotation;
    }
}
