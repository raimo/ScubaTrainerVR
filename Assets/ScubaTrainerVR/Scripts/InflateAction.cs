using UnityEngine;
using System.Collections;

public class InflateAction : MonoBehaviour {	
	public GameState gameState;
	public Diver diver;
	bool isPressed;

	// Use this for initialization
	void Start () {
		isPressed = false;
	}
	
	// Update is called once per frame
	void Update () {
	}

	private IEnumerator GoToNextStage() {
		yield return new WaitForSeconds(3.0f);
		gameState.AdvanceState (GameState.State.HOLD_DURING_GIANT_STRIDE);
	}


	void OnTriggerEnter(Collider other) {
		if ((other.name.StartsWith ("palm") || other.name.StartsWith ("forearm") || other.name.StartsWith ("bone"))) {
			Debug.Log("InflateAction");
			diver.inflateBC ();
			// TODO do this only after BCD is inflated, now we do it immediately
			if (diver.isBCDFull () && gameState.state == GameState.State.INFLATE_BCD_ENTIRELY) {
				StartCoroutine(GoToNextStage());
			}
		}
	}
}