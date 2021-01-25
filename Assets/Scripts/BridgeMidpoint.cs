using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeMidpoint : MonoBehaviour
{
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Midpoint = this.gameObject;
            Debug.Log("midpoint: " + collision.gameObject.GetComponent<PlayerController>().Midpoint);
        }
    }
}
