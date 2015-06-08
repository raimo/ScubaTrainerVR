using UnityEngine;
using System.Collections;
using System;

public class AirComputer : MonoBehaviour {
	public Diver diver;
	public TextMesh gauge;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		decimal depthM = Math.Round (Convert.ToDecimal (diver.transform.position.y), 2);
		decimal rateM = Math.Round (Convert.ToDecimal (diver.ascentRate), 2);
		gauge.text = "<size=13>DEPTH</size>                  <size=13>BCD VOLUME</size>\n" +
			(depthM < 0 ? -depthM : 0).ToString("0.00") + " m        " + (100 * diver.CurrentBCDVolume / (diver.MaxBCDVolume * diver.atm)).ToString("0") + "%\n" +
				"<size=13>ASCENT RATE</size>            \n" +
			(depthM < 0 && (!diver.AtBottom()) ? rateM : 0).ToString("0.00") + " m        " + "" + " \n";
	}
}
