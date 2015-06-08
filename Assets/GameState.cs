using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {
    public bool showDebugMessages = true;

	// All the game states:

	public enum State {
		NO_STATE, // USED IN PLACE OF NULL
		INFLATE_BCD_ENTIRELY, // TODO activate this at start when we get the player to stay on top of the dock, and hook HOLD_DURING_GIANT_STRIDE when BCD is full. 
		HOLD_DURING_GIANT_STRIDE, // activated at start
		GO_GO, // when ready to jump
		VENT_BCD_A_BIT_TO_START_DESCENT, // activated in Diver.cs & PutOnRegulator.cs, whichever happens first
		YOURE_AT_THE_BOTTOM, // TODO
		SIGNAL_ASCENT, // TODO
		VENT_AIR_FREQUENTLY_DURING_ASCENT, // activated in Diver.cs
		YOURE_AT_THE_SURFACE, // activated in Diver.cs
		FAILURE // NOT USED SINCE THIS IS A SAFETY TRAINER
	};



	public AudioSource inflateBcdEntirely;
	public AudioSource holdDuringGiantStride;
	public AudioSource goGo;
	public AudioSource ventBcdABitToStartDescent;
	public AudioSource youreAtTheBottom;
	public AudioSource signalAscent;
	public AudioSource ventAirFrequentlyDuringAscent;
	public AudioSource youreAtTheSurface;
    public AudioSource failure;

	public GameObject platform;
	public Rigidbody playerBody;

	private Dictionary<State, string> stateMessages = new Dictionary<State, string>	{
		{State.INFLATE_BCD_ENTIRELY,
			"Welcome to the Scuba Diving Crash Course for Leap Motion VR! Start off by fully inflating your BCD (left)."},
		{State.HOLD_DURING_GIANT_STRIDE, 
			"Hold the regulator in your mouth to keep it still during the giant stride entrance to deep water."},
		{State.GO_GO, 
			"Great! Now off we go! Remember to signal “descent” to your buddies by pointing your thumb downwards."},
		{State.VENT_BCD_A_BIT_TO_START_DESCENT, 
			"Vent your BCD a bit to start descending using the Low Pressure Inflator (LPI), but be ready to inflate it frequently as the water pressure and air density increases."},
		{State.YOURE_AT_THE_BOTTOM, 
			"You’re at the bottom! Swim around for a bit. Become comfortable with reading the air, depth, and compass on your dive computer or dive gauge."},
		{State.SIGNAL_ASCENT, 
			"Signal “ascent” by holding your thumb upwards to start ascending with a small kick with your fins."},
		{State.VENT_AIR_FREQUENTLY_DURING_ASCENT, 
			"Vent air frequently during your ascent as the air expands while the pressure decreases."},
		{State.YOURE_AT_THE_SURFACE,
			"You’re at the surface! Inflate your BCD and signal “okay” to your buddies. Also, go practice in real water after finishing diver's handbook!"},
		{State.FAILURE, 
			"Sorry, you lost the game. Please try again after reading diver's handbook."}
	};
	public State state;
	public State prevState;
	public GameObject messageFieldBox;
	public TextMesh messageField;

	// Use this for initialization
	void Start () {
		messageFieldBox.active = false;
        StartCoroutine(WelcomeMessage());
	}


	private IEnumerator WelcomeMessage() {
		yield return new WaitForSeconds(5.0f);
		state = State.INFLATE_BCD_ENTIRELY;
		inflateBcdEntirely.Play ();
	}

	private IEnumerator HideMessage() {
		yield return new WaitForSeconds(10.0f);
		messageFieldBox.active = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (prevState != state) {
			showCurrentMessage ();
		}
		prevState = state;
	}

	public void AdvanceState (State nextState) {
		state = nextState;
		if (state == State.HOLD_DURING_GIANT_STRIDE) {
			holdDuringGiantStride.Play ();
		} else if (state == State.GO_GO) {
			platform.SetActive(false);
			playerBody.isKinematic = false;
			goGo.Play ();
		} else if (state == State.VENT_BCD_A_BIT_TO_START_DESCENT) {
			ventBcdABitToStartDescent.Play ();
		} else if (state == State.YOURE_AT_THE_BOTTOM) {
			youreAtTheBottom.Play ();
		} else if (state == State.SIGNAL_ASCENT) {
			signalAscent.Play ();
		} else if (state == State.VENT_AIR_FREQUENTLY_DURING_ASCENT) {
			ventAirFrequentlyDuringAscent.Play ();
		} else if (state == State.YOURE_AT_THE_SURFACE) {
			youreAtTheSurface.Play ();
		} else if (state == State.FAILURE) {
			failure.Play ();
		} 
	}

	public void showCurrentMessage () {
		try {
			messageFieldBox.active = true;
			messageField.text = formatText(stateMessages [state], messageField, 40);
            if(showDebugMessages)
    			Debug.Log (stateMessages [state]);
            StartCoroutine(HideMessage());
		}  catch(KeyNotFoundException e) {
			Debug.Log (state);
		}
	}

	private string formatText(string textToFormat, TextMesh textObj, int charsInLine) {
		var words = textToFormat.Split(" "[0]);
		var newString = "";
		var testString = "";
		for (var i = 0; i < words.Length; i++){
			testString = testString + words[i] + " ";
			textObj.text = testString;
			if(testString.Length > charsInLine) {
				testString = words[i] + " ";
				newString = newString + "\n" + words[i] + " ";
			} else {
				newString = newString + words[i] + " ";
			}
		}
		return newString;
	}
}