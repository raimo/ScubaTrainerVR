﻿using UnityEngine;
using System.Collections;

public class Diver : MonoBehaviour {
    private bool isUnderwater = false;
    private bool isSwimming = false;
    private bool isFloating = false;
    public Color normalFogColor = new Color (0.5f, 0.5f, 0.5f, 0.5f);
    public float normalFogDensity = .003f;
    public Color underwaterFogColor = new Color (0.81f, 0.93f, 0.37f, 0.5f);
    public float underwaterFogDensity = .02f;
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
    public float BCDFillRate = 20f; //Litres/second

    public float CurrentLungVolume = 6;
    public float CurrentBCDVolume = 25;

    private float lastDepth = 0;
    private float[] ascentSamples = new float[10];

    //private ConstantForce bouyancy = null;
    private Rigidbody physicsBody = null;
    private CapsuleCollider capsule = null;

    //inspection variables
    public float currentBouyancy = 0;
    public float atm = 1;
    public float currentVolume = 0;
    public GameState gameState;
    public bool isSurfaced = false;
    public bool isBottomed = false;
    public float ascentRate = 0;

	// Use this for initialization
	void Start () {
        //bouyancy = gameObject.GetComponent<ConstantForce> ();
        physicsBody = gameObject.GetComponent<Rigidbody> ();
        physicsBody.centerOfMass = new Vector3 (0, -1, 0);
        capsule  = gameObject.GetComponent<CapsuleCollider> ();
        CurrentLungVolume = MaxLungVolume;
        normalFogColor = RenderSettings.fogColor;
        normalFogDensity = RenderSettings.fogDensity;

	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (isUnderwater) {
            atm = 1 - (transform.position.y - SeaLevel)/ 10;
            if(atm < 1) atm = 1;

            if (CurrentBCDVolume / atm > MaxBCDVolume)
                CurrentBCDVolume = MaxBCDVolume * atm;
            
            currentVolume = StaticBodyVolume + (CurrentLungVolume + CurrentBCDVolume + MaxWetsuitAirVolume) / atm;
            float currentWeight = BodyWeight + EquipmentWeight + WeightBelt;
            currentBouyancy = currentVolume * WaterWeight;
            physicsBody.AddForce (new Vector3 (0, currentBouyancy, 0), ForceMode.Acceleration);
            physicsBody.AddForce (new Vector3 (0, -currentWeight, 0), ForceMode.Acceleration);
            physicsBody.mass = currentWeight/10;           
        }
        turn ();
        move ();
        sampleAscent ();
	}

    void Update(){
        if (transform.position.y <= SeaLevel) {
            if (!isUnderwater) {
                isUnderwater = true;
                SetUnderwater ();
            }
        } else if (isUnderwater) {
            isUnderwater = false;
            SetNormal ();
        }

        if (transform.position.y > (SeaLevel - .5f) && isSwimming) {
            //transform.position = new Vector3 (transform.position.x, SeaLevel, transform.position.z); 
            isFloating = true;
            //SetNormal ();
        }

        HandleKeyboardInput ();

        isSurfaced = AtSurface ();
        isBottomed = AtBottom ();
        ascentRate = GetAscentRate ();

    }
    void SetNormal () 
    {
        //Debug.Log("Normal");
        //RenderSettings.fogColor = normalFogColor;
        //RenderSettings.fogDensity = normalFogDensity;
        physicsBody.useGravity = true;
        isFloating = true;
        if (gameState.state == GameState.State.VENT_AIR_FREQUENTLY_DURING_ASCENT) {
            gameState.AdvanceState (GameState.State.YOURE_AT_THE_SURFACE);
        }

    }

    void SetUnderwater () 
    {
        //Debug.Log("Underwater");
        isSwimming = true;
        //RenderSettings.fogColor = underwaterFogColor;
        //RenderSettings.fogDensity = underwaterFogDensity;
        physicsBody.useGravity = false;
        if (gameState.state == GameState.State.SIGNAL_DESCENT) {
            gameState.AdvanceState (GameState.State.VENT_BCD_A_BIT_TO_START_DESCENT);
        }
    }

    public void moveForward(){
        physicsBody.AddForce (transform.forward * 40 * Time.deltaTime, ForceMode.Impulse);
    }

    public void turn(){
        float turn = Input.GetAxis("Horizontal");
        physicsBody.AddTorque (Vector3.up * turn);
    }

    public void move(){
        float speed = Input.GetAxis("Vertical");
        if (speed > 0) {
            physicsBody.AddForce (transform.forward * speed * 40 * Time.deltaTime, ForceMode.Impulse);
        } else {
            physicsBody.AddForce (Vector3.up * speed * 40 * Time.deltaTime, ForceMode.Impulse);
        }
    }

    public void inflateBC(){
        CurrentBCDVolume += BCDFillRate * Time.deltaTime;
    }

    public void deflateBC(){
        CurrentBCDVolume -= BCDFillRate * Time.deltaTime;
        if (CurrentBCDVolume < 0)
            CurrentBCDVolume = 0;
        isFloating = false;
    }

    public void dropWeightBelt(){
        WeightBelt = 0;
    }

    private void HandleKeyboardInput(){
        if (Input.GetKey (KeyCode.Comma))
            deflateBC ();
        if (Input.GetKey (KeyCode.Period))
            inflateBC ();
        if (Input.GetKey (KeyCode.Slash))
            dropWeightBelt ();
        if (Input.GetKey (KeyCode.UpArrow))
            moveForward ();
    }

    public bool AtBottom(){
        float terrainOffset = -Terrain.activeTerrain.transform.position.y;
        Vector3 pos = transform.position;
        float height = Terrain.activeTerrain.SampleHeight(transform.position);
        if (Mathf.Abs (pos.y + (terrainOffset - height)) < 4)
            return true;
        return false;
    }

    public bool AtSurface(){
        if (transform.position.y > SeaLevel - 1) {
            return true;
        }
        return false;
    }

    private void sampleAscent(){
        for (int i = 1; i < ascentSamples.Length; i++) {
            ascentSamples [i] = ascentSamples [i - 1];
        }
                ascentSamples[ascentSamples.Length - 1] = ((transform.position.y - lastDepth) / Time.deltaTime) * 60; //Meters/minute safe rate is about 10 
        lastDepth = transform.position.y;
    }

    public float GetAscentRate(){
        float average = 0;
        for (int i = 0; i < ascentSamples.Length; i++) {
            average += ascentSamples [i];
        }

        return average/ascentSamples.Length;
    }
}
