using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // PlayerのRigidbody
    Rigidbody rbody;
    // 仮想重力の係数
    float gravity = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rbody.AddRelativeForce(-Vector3.up * gravity);
    }
}
