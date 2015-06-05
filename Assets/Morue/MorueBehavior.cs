using UnityEngine;
using System;
using System.Collections;

public class MorueBehavior : MonoBehaviour {
	public GameObject morue;
	float delta;
	float angle=1;
	float angleDelta=0.1f;
	// Use this for initialization
	void Start () {
		delta = Time.deltaTime * 140f;
	}
	
	// Update is called once per frame
	void Update () {
		morue.transform.Translate (delta, 0, 0);
//		morue.transform.RotateAround (morue.transform.position, new Vector3 (0,0,1), angle);
		if (Math.Abs(morue.transform.position.x) > 150) {
			morue.transform.RotateAround (morue.transform.position, new Vector3 (0,1,0), 180);
		}
		angle += angleDelta;
		if (Math.Abs (angle) > 1f)
			angleDelta = - angleDelta;
	}
}
