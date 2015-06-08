using UnityEngine;
using System.Collections;

public class Diver : MonoBehaviour {
    private bool isUnderwater = false;
    private bool isSwimming = false;
    private bool isFloating = false;

    public float SeaLevel = 0;
    public AudioClip hitBottom = null;
    public AudioClip surface = null;
    public AudioClip deflate = null;
    public AudioClip inflate = null;
    public AudioClip splash = null;
    private AudioSource sound = null;

    public float WaterWeight = 1.02f; //Kg/Litre
    public float BodyWeight = 86f; //kilograms
    public float EquipmentWeight = 10f; //Kg
    public float WeightBelt = 8f; //Kg

    public float MaxWetsuitAirVolume = 8f; //Litre at surface
    public float StaticBodyVolume = 75f; //Litre
    public float MaxLungVolume = 6f; //Litre
    //Airtank?
    public float MaxBCDVolume = 40f; //Litre
    public float BCDFillRate = 20f; //Litres/second

    public float CurrentLungVolume = 6;
    public float CurrentBCDVolume = 25;

    private float lastDepth = 0;
    private float[] ascentSamples = new float[10];

    //private ConstantForce bouyancy = null;
    private Rigidbody physicsBody = null;
    private CapsuleCollider capsule = null;
    private SwimController swimController = null;
    private ParticleSystem ambientParticles = null;

    //inspection variables
    public float currentBouyancy = 0;
    public float atm = 1;
    public float currentVolume = 0;
    public GameState gameState;
    public bool isSurfaced = false;
    public bool isBottomed = false;
    public float ascentRate = 0;
    public int BCDPercentFull = 0;

	// Use this for initialization
	void Start () {
        physicsBody = gameObject.GetComponent<Rigidbody> ();
        physicsBody.centerOfMass = new Vector3 (0, -1, 0);
        capsule  = gameObject.GetComponent<CapsuleCollider> ();
        swimController = gameObject.GetComponent<SwimController> ();
        ambientParticles = gameObject.GetComponent<ParticleSystem> ();
        sound = GetComponent<AudioSource> ();
        CurrentLungVolume = MaxLungVolume;

	}

	public bool isBCDFull() {
		return CurrentBCDVolume / atm >= MaxBCDVolume;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (isBCDFull())
            CurrentBCDVolume = MaxBCDVolume * atm;

        if (isUnderwater) {
            atm = 1 - (transform.position.y - SeaLevel)/ 10;
            if(atm < 1) atm = 1;

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
        BCDPercentFull = GetBCDPercentFull ();

    }
    void SetNormal () 
    {
        //Debug.Log("Normal");
        physicsBody.useGravity = true;
        swimController.airSuppressor = .7f;
        isFloating = true;
        if (gameState.state == GameState.State.VENT_AIR_FREQUENTLY_DURING_ASCENT) {
            gameState.AdvanceState (GameState.State.YOURE_AT_THE_SURFACE);
        }
        sound.Stop ();
        ambientParticles.Stop();
    }

    void SetUnderwater () 
    {
        //Debug.Log("Underwater");
        isSwimming = true;
        physicsBody.useGravity = false;
        swimController.airSuppressor = 1.0f;
        if (gameState.state == GameState.State.GO_GO) {
            gameState.AdvanceState (GameState.State.VENT_BCD_A_BIT_TO_START_DESCENT);
            sound.PlayOneShot (splash, .05f);
        }
        sound.Play ();
        ambientParticles.Play();
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
            physicsBody.AddForce (Vector3.back * speed * 40 * Time.deltaTime, ForceMode.Impulse);
        }
    }

    public void inflateBC(){
        CurrentBCDVolume += BCDFillRate * Time.deltaTime;
        if (CurrentBCDVolume / atm > MaxBCDVolume)
            CurrentBCDVolume = MaxBCDVolume * atm;
        sound.PlayOneShot (inflate, .05f);

    }

    public void deflateBC(){
        CurrentBCDVolume -= BCDFillRate * Time.deltaTime;
        if (CurrentBCDVolume < 0)
            CurrentBCDVolume = 0;
        sound.PlayOneShot (deflate, .05f);
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
        if (Input.GetKey (KeyCode.Z))
            AutoCompensate ();
    }

    public bool AtBottom(){
        float terrainOffset = -Terrain.activeTerrain.transform.position.y;
        Vector3 pos = transform.position;
        float height = Terrain.activeTerrain.SampleHeight(transform.position);
        if (Mathf.Abs (pos.y + (terrainOffset - height)) < 4) {
            if (gameState.state == GameState.State.VENT_BCD_A_BIT_TO_START_DESCENT) {
                gameState.AdvanceState (GameState.State.YOURE_AT_THE_BOTTOM);
                StartCoroutine(GoBackUp());
            }
            sound.PlayOneShot (hitBottom, .05f);
            return true;
        }
        return false;
    }

	private IEnumerator GoBackUp() {
		yield return new WaitForSeconds(9.0f);
        gameState.AdvanceState (GameState.State.SIGNAL_ASCENT);
		yield return new WaitForSeconds(6.0f);
        gameState.AdvanceState (GameState.State.VENT_AIR_FREQUENTLY_DURING_ASCENT);
	}

    public bool AtSurface(){
        if (transform.position.y > SeaLevel - 1) {
            sound.PlayOneShot (surface, .05f);
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

    public int GetBCDPercentFull(){
        return (int)(CurrentBCDVolume / atm / MaxBCDVolume * 100);
    }

    public void AutoCompensate(){
        float delta = (BodyWeight + EquipmentWeight + WeightBelt) - currentBouyancy;
        CurrentBCDVolume += delta * atm;
    }

	void OnTriggerEnter(Collider other) {
		if ((other.name.StartsWith ("palm") || other.name.StartsWith ("forearm") || other.name.StartsWith ("bone"))) {
			Debug.Log("ignoring " + other.name);
//			Physics.IgnoreCollision(gameObject.GetComponent<Collider>().transform.root.GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
		}
	}


}
