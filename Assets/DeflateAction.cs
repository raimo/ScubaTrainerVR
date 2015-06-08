using UnityEngine;
using System.Collections;

public class DeflateAction : MonoBehaviour {
	public GameState gameState;
	public Diver diver;
	bool isPressed;
	
	// Use this for initialization
	void Start () {
		isPressed = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (isPressed) {

			Debug.Log("DeflateAction");
			diver.deflateBC ();
		}
	}

	void OnTriggerEnter(Collider other) {
		if ((other.name.StartsWith ("palm") || other.name.StartsWith ("forearm") || other.name.StartsWith ("bone"))) {
			isPressed = true;
		}
	}
	void OnTriggerExit(Collider other) {
		if ((other.name.StartsWith ("palm") || other.name.StartsWith ("forearm") || other.name.StartsWith ("bone"))) {
			isPressed = false;
		}
	}
}
