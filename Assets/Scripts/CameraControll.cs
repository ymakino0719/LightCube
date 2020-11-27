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
        // カメラの回転: playerの方を向く
        transform.LookAt(player.transform);
    }
}
