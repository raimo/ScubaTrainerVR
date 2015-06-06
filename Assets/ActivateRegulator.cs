using UnityEngine;
using System.Collections;

public class ActivateRegulator : MonoBehaviour {

	public GameObject regulator;
	public GameState gameState;
	private bool regulatorMoving = false;
	private float secondsBehind = 0;

	// Use this for initialization
	void Start () {
		regulator.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (gameState.state == GameState.State.HOLD_DURING_GIANT_STRIDE && !regulator.active) {
			regulator.SetActive (true);
		}
	}
}
