using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_CameraController : MonoBehaviour
{
	private Vector3 lastPos;
	private Vector3 newPos;

	int mapNum, mapDir;
	int dir;
	bool rotatingX = false;
	bool rotatingY = false;
	bool x_45 = false;
	bool y_45 = false;
	float rotSpeed = 1.5f;
	float rotAngle = 0.0f;
	bool rotEnd = false;

	void Start()
	{
		StartingCameraRotation();
	}

	void FixedUpdate()
	{
		if (!rotatingX && !rotatingY)
		{
			InputRotationDir();
		}

		if (rotatingX || rotatingY)
		{
			RotateCamera();
		}
	}

	void StartingCameraRotation()
	{
		float rotX = 0;
		float rotY = 0;

		if (mapNum == 1)
		{
			rotX = -90.0f;
		}
		else if (mapNum == 2)
		{
			rotY = 90.0f;
		}
		else if (mapNum == 3)
		{
			rotY = -90.0f;
		}
		else if (mapNum == 4)
		{
			rotX = 90.0f;
		}
		else if (mapNum == 5)
		{
			rotX = 180.0f;
		}
		else // mapNum == 0
		{

		}
		transform.RotateAround(Vector3.zero, -transform.right, rotX);
		transform.RotateAround(Vector3.zero, -transform.up, rotY);

		transform.Rotate(0, 0, -mapDir * 90.0f);
	}

	void InputRotationDir()
	{
		float v = Input.GetAxis("Vertical");
		float h = Input.GetAxis("Horizontal");

		if (Mathf.Abs(h) > float.Epsilon && !y_45)
		{
			rotatingX = true;
			x_45 = !x_45;

			if (h >= 0)
			{
				dir = 1;
			}
			else
			{
				dir = -1;
			}
		}
		else if (Mathf.Abs(v) > float.Epsilon && !x_45)
		{
			rotatingY = true;
			y_45 = !y_45;

			if (v >= 0)
			{
				dir = 1;
			}
			else
			{
				dir = -1;
			}
		}
	}

	void RotateCamera()
	{
		rotAngle += rotSpeed;

		if (rotAngle >= 45.0f)
		{
			rotAngle = 45.0f;
			rotEnd = true;
		}

		if (rotatingX)
		{
			transform.RotateAround(Vector3.zero, -transform.up, rotSpeed * dir);
		}
		else
		{
			transform.RotateAround(Vector3.zero, transform.right, rotSpeed * dir);
		}

		if (rotEnd)
		{
			rotEnd = false;
			rotAngle = 0.0f;
			rotatingX = false;
			rotatingY = false;
		}
	}

	/*
	void RotateCamera()
	{
		if (Input.GetMouseButtonDown(0))
		{
			lastPos.x = Input.mousePosition.x;
			lastPos.y = Input.mousePosition.y;
		}
		else if (Input.GetMouseButton(0))
		{
			newPos.x = Input.mousePosition.x - lastPos.x;
			newPos.y = Input.mousePosition.y - lastPos.y;
			lastPos.x = Input.mousePosition.x;
			lastPos.y = Input.mousePosition.y;

			transform.RotateAround(Vector3.zero, transform.up, newPos.x);
			transform.RotateAround(Vector3.zero, -transform.right, newPos.y);
		}
	}
	*/

	public int MapNum
	{
		set { mapNum = value; }
		get { return mapNum; }
	}

	public int MapDir
	{
		set { mapDir = value; }
		get { return mapDir; }
	}
}
