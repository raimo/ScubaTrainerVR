using UnityEngine;
using System.Collections;
using System;

public class DolphinBehavior : MonoBehaviour {
	public GameObject dolphin;
	float delta;
	float angle=1;
	float angleDelta=-0.1f;

	// Use this for initialization
	void Start () {
		delta = -Time.deltaTime * 140f;
		dolphin.transform.RotateAround (dolphin.transform.position, new Vector3 (0,0,1), angle/angleDelta);
	}
	
	// Update is called once per frame
	void Update () {
		dolphin.transform.Translate (delta, 0, 0);
		dolphin.transform.RotateAround (dolphin.transform.position, new Vector3 (0,0,1), angle);
		if (Math.Abs(dolphin.transform.position.x) > 150) {
			dolphin.transform.RotateAround (dolphin.transform.position, new Vector3 (0,1,0), 180);
		}
		if (Math.Abs(dolphin.transform.position.y) > 100) {
			dolphin.transform.RotateAround (dolphin.transform.position, new Vector3 (0,0,1), 180);
		}
		angle += angleDelta;
		if (Math.Abs (angle) > 1f)
			angleDelta = - angleDelta;
	}
}
