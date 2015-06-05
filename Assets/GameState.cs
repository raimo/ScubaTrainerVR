using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {

	// All the game states:

	public enum State {
		NO_STATE, // USED IN PLACE OF NULL
		INFLATE_BCD_ENTIRELY, // TODO activate this at start when we get the player to stay on top of the dock, and hook HOLD_DURING_GIANT_STRIDE when BCD is full. 
		HOLD_DURING_GIANT_STRIDE, // activated at start
		SIGNAL_DESCENT, // activated in PutOnRegulator.cs
		VENT_BCD_A_BIT_TO_START_DESCENT, // activated in Diver.cs & PutOnRegulator.cs, whichever happens first
		YOURE_AT_THE_BOTTOM, // TODO
		SWIM_AROUND_FOR_A_BIT, // TODO
		VENT_AIR_FREQUENTLY_DURING_ASCENT, // activated in Diver.cs
		YOURE_AT_THE_SURFACE, // activated in Diver.cs
		FAILURE // NOT USED SINCE THIS IS A SAFETY TRAINER
	};

	private Dictionary<State, string> stateMessages = new Dictionary<State, string>	{
		{State.INFLATE_BCD_ENTIRELY,
			"Welcome to Scuba Diving Crash Course for Leap Motion VR! Start off by inflating your BCD completely."},
		{State.HOLD_DURING_GIANT_STRIDE, 
			"Hold the regulator in your mouth to keep it still during the giant stride entrance to deep water"},
		{State.SIGNAL_DESCENT, 
			"Signal “descent” to your buddies by pointing your thumb downwards"},
		{State.VENT_BCD_A_BIT_TO_START_DESCENT, 
			"Vent your BCD a bit to start descending using the Low Pressure Inflator (LPI), but be ready to inflate it frequently as the water pressure and air density increases"},
		{State.YOURE_AT_THE_BOTTOM, 
			"You’re at the bottom! Signal “ascent” by holding your thumb upwards to start ascending with a small kick with your fins."},
		{State.SWIM_AROUND_FOR_A_BIT, 
			"Swim around for a bit. Become comfortable with reading the air, depth, and compass on your dive computer or dive gauge."},
		{State.VENT_AIR_FREQUENTLY_DURING_ASCENT, 
			"Vent air frequently during your ascent as the air expands while the pressure decreases"},
		{State.YOURE_AT_THE_SURFACE,
			"You’re at the surface! Inflate your BCD and signal “okay” to your buddies. Also, go practice in real water after finishing diver's handbook!"},
		{State.FAILURE, 
			"Sorry, you lost the game. Please try again after reading diver's handbook"}
	};
	public State state;

	// Use this for initialization
	void Start () {
		// TODO use this when BCD inflate is ready
		//state = State.INFLATE_BCD_ENTIRELY;
		state = State.HOLD_DURING_GIANT_STRIDE;
	}
	
	// Update is called once per frame
	void Update () {
		showCurrentMessage ();
	}

	public void AdvanceState (State nextState) {
		state = nextState;
	}

	public void showCurrentMessage () {
		try {
			Debug.Log (stateMessages [state]);
		}  catch(KeyNotFoundException e) {
			Debug.Log (state);
		}
	}
}