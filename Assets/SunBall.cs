using UnityEngine;
using System.Collections;

public class SunBall : MonoBehaviour {
    private float setIntensity = 1;
    private Light theLight = null;
    private LensFlare theFlare = null;
    public GameObject target = null;

    //Variation of sunball brightness
    public float slope = 0.5f;
    public float interval = .5f;
    private float smooth = 0.5f;
    private float tNext = 0f;
    private float dir = 1f;

    void Start () {
        theLight = GetComponent<Light> ();
        theFlare = GetComponent<LensFlare> ();

        setIntensity = theLight.intensity;

	}
	
	// Update is called once per frame
	void Update () {
        if (target.transform.position.y < 0) {
            theLight.intensity = setIntensity/-target.transform.position.y*10;
            Vector3 aboveDiver = new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z);
            transform.position = aboveDiver;
            transform.LookAt (target.transform, Vector3.up);

            if (Time.time>tNext){
                tNext += interval*(0.5f+Random.value);
                dir = -dir;
            }
            smooth += dir*slope*Time.deltaTime;
            if (smooth>1 || smooth<0) dir = -dir;
            smooth = Mathf.Clamp(smooth,.7f,1.4f);
            theFlare.brightness = smooth;
        } else {
            theLight.intensity = 0;
        }
	}
}
