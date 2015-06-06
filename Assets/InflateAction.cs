using UnityEngine;
using System.Collections;

public class InflateAction : MonoBehaviour {	
	public GameState gameState;
	public Diver diver;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		diver.inflateBC ();
		// TODO do this only after BCD is inflated, now we do it immediately
		if (diver.isBCDFull() && (other.name.StartsWith ("palm") || other.name.StartsWith ("forearm") || other.name.StartsWith ("bone")) && gameState.state == GameState.State.INFLATE_BCD_ENTIRELY) {
			gameState.AdvanceState (GameState.State.HOLD_DURING_GIANT_STRIDE);
		}
	}
}