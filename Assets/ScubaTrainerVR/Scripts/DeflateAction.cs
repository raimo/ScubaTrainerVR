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
		}
	}

	void OnTriggerEnter(Collider other) {
		if ((other.name.StartsWith ("palm") || other.name.StartsWith ("forearm") || other.name.StartsWith ("bone"))) {
			diver.deflateBC ();
		}
	}
}
