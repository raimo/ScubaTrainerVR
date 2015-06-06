using UnityEngine;
using System.Collections;

public class DeflateAction : MonoBehaviour {
	public GameState gameState;
	public Diver diver;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other) {
		diver.deflateBC ();
	}
}
