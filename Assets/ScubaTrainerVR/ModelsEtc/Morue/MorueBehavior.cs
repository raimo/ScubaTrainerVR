using UnityEngine;
using System;
using System.Collections;

public class MorueBehavior : MonoBehaviour {
	public GameObject morue;
	float delta;
	float angle=3;
	float angleDelta=0.1f;
	// Use this for initialization
	void Start () {
		morue = gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		morue.transform.Translate (Time.deltaTime * 40f, 0, 0);
		morue.transform.RotateAround (morue.transform.position, new Vector3 (0,1,0), angle);
		if (Math.Abs(morue.transform.position.x) > 250 || Math.Abs(morue.transform.position.z) > 250) {
			morue.transform.RotateAround (morue.transform.position, new Vector3 (0,1,0), 180);
		}
		angle += angleDelta;
		if (Math.Abs (angle) > 3f)
			angleDelta = - angleDelta;
	}
}
