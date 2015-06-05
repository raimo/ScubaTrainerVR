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
			if (secondsBehind < 6) {
				regulator.transform.Translate (0, Time.deltaTime*0.08f, -Time.deltaTime*0.02f);
				secondsBehind += Time.deltaTime;
			} else {
				regulatorMoving = false;
                gameState.AdvanceState (GameState.State.GO_GO);
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if ((other.name.StartsWith ("palm") || other.name.StartsWith ("forearm") || other.name.StartsWith ("bone")) && gameState.state == GameState.State.HOLD_DURING_GIANT_STRIDE) {
			regulatorPosition = regulator.transform.position;
			regulatorMoving = true;
			secondsBehind = 0;
		}
	}
}
