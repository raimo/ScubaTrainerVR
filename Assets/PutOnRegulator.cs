using UnityEngine;
using System.Collections;

public class PutOnRegulator : MonoBehaviour {

	public GameObject regulator;
	public GameState gameState;
	private bool regulatorMoving = false;
	private Vector3 regulatorPosition;
	private float secondsBehind = 0;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (regulatorMoving) {
			if (secondsBehind < 9) {
				regulator.transform.Translate (0, Time.deltaTime*0.08f, -Time.deltaTime*0.02f);
				secondsBehind += Time.deltaTime;
			} else {
				regulatorMoving = false;
				if (gameState.state == GameState.State.SIGNAL_DESCENT) {
					gameState.AdvanceState (GameState.State.VENT_BCD_A_BIT_TO_START_DESCENT);
				}
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if ((other.name.StartsWith ("palm") || other.name.StartsWith ("forearm") || other.name.StartsWith ("bone")) && gameState.state == GameState.State.HOLD_DURING_GIANT_STRIDE) {
			gameState.AdvanceState (GameState.State.SIGNAL_DESCENT);
			regulatorPosition = regulator.transform.position;
			regulatorMoving = true;
			secondsBehind = 0;
		}
	}
}
