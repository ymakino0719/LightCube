using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfwayPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Playerの向きとHalfwayPointの向きを確認し、正しい方角であるかを確認する
            float angle = Vector3.Angle(collision.gameObject.transform.up, transform.up);

            if(angle <= 45) collision.gameObject.GetComponent<PlayerController>().Halfway = true;
        }
    }
}
