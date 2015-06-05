using UnityEngine;
using System.Collections;

public class Diver : MonoBehaviour {
    private bool isUnderwater = false;
    public Color normalFogColor = new Color (0.5f, 0.5f, 0.5f, 0.5f);
    public Color underwaterFogColor = new Color (0.22f, 0.65f, 0.77f, 0.5f);
    public GameObject waterPlane = null;

    public float SeaLevel = 0;

    public float WaterWeight = 1.02f; //Kg/Litre
    public float BodyWeight = 86f; //kilograms
    public float EquipmentWeight = 10f; //Kg
    public float WeightBelt = 8f; //Kg

    public float MaxWetsuitAirVolume = 8f; //Litre at surface
    public float StaticBodyVolume = 75f; //Litre
    public float MaxLungVolume = 6f; //Litre
    //Airtank?
    public float MaxBCDVolume = 25f; //Litre


    public float CurrentLungVolume = 0;
    public float CurrentBCDVolume = 0;
    public float CurrentWetsuitAirVolume = 0;

    private ConstantForce bouyancy = null;
    private Rigidbody physicsBody = null;
    //private CharacterController characterController = null;

    //inspection variables
    public float currentBouyancy = 0;
    public float atm = 1;
    public float currentVolume = 0;
	public GameState gameState;
	// Use this for initialization
	void Start () {
        bouyancy = gameObject.GetComponent<ConstantForce> ();
        physicsBody = gameObject.GetComponent<Rigidbody> ();
        //characterController = gameObject.GetComponent<CharacterController> ();

        CurrentLungVolume = MaxLungVolume;
        CurrentBCDVolume = MaxBCDVolume/3;
        CurrentWetsuitAirVolume = MaxWetsuitAirVolume;
        normalFogColor = RenderSettings.fogColor;

	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y <= SeaLevel) {
            if (!isUnderwater) {
                isUnderwater = true;
                SetUnderwater ();
            }
        } else if (isUnderwater) {
            isUnderwater = false;
            SetNormal ();
        }
        if (isUnderwater) {
            atm = 1 + Mathf.Abs (transform.position.y / 10);
            currentVolume = StaticBodyVolume + (CurrentLungVolume + CurrentBCDVolume + CurrentWetsuitAirVolume) / atm;
            currentBouyancy = currentVolume * WaterWeight - (EquipmentWeight + WeightBelt + BodyWeight);
            bouyancy.relativeForce = new Vector3 (0, currentBouyancy, 0);
            //characterController.Move (new Vector3 (0, currentBouyancy * Time.deltaTime, 0));
           
        } else {
            bouyancy.relativeForce = new Vector3 (0, 0, 0);
            atm = 1;
        }

	}

    void SetNormal () 
    {
        Debug.Log("Normal");
        RenderSettings.fogColor = normalFogColor;
        RenderSettings.fogDensity = 0.05f;
        //physicsBody.useGravity = true;
        bouyancy.enabled = false;
        if (waterPlane) {
            Vector3 scale = waterPlane.transform.localScale;
            scale.y = 1;
            waterPlane.transform.localScale = scale;
        }
		if (gameState.state == GameState.State.VENT_AIR_FREQUENTLY_DURING_ASCENT) {
			gameState.AdvanceState (GameState.State.YOURE_AT_THE_SURFACE);
		}
    }

    void SetUnderwater () 
    {
        Debug.Log("Underwater");
        RenderSettings.fogColor = underwaterFogColor;
        RenderSettings.fogDensity = 0.03f;
        //physicsBody.useGravity = false;
        bouyancy.enabled = true;
        if (waterPlane) {
            Vector3 scale = waterPlane.transform.localScale;
            scale.y = -1;
            waterPlane.transform.localScale = scale;
        }
		if (gameState.state == GameState.State.SIGNAL_DESCENT) {
			gameState.AdvanceState (GameState.State.VENT_BCD_A_BIT_TO_START_DESCENT);
		}
    }

}
