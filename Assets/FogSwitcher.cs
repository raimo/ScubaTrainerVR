using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class FogSwitcher : MonoBehaviour {
    public Color normalFogColor = new Color (0.5f, 0.5f, 0.5f, 0.5f);
    public float normalFogDensity = .0003f;
    public Color underwaterFogColor = new Color (0.3f, 0.5f, 0.9f, 0.5f);
    public float underwaterFogDensity = .005f;

    public Color UnderSurfaceLight = new Color (.5f, .7f, .9f, 1.0f);
    public Color UnderSurfaceDark = new Color (.01f, .07f, .09f, 1.0f);
    public bool isUnderwater = false;

    public AudioClip underwaterAmbient = null;
    public AudioClip overwaterAmbient = null;
    private AudioSource ambient = null;

    private GlobalFog[] depthShaderControls = null;
    private Camera[] cameras = null;
    private GameObject waterPlane = null;
    private ParticleSystem bubbleMaker = null;

    void Start(){
        depthShaderControls = gameObject.transform.root.GetComponentsInChildren<GlobalFog> ();
		Debug.Log ("NUMBER OF GLOBAL FOG " + depthShaderControls.Length);
		cameras = gameObject.transform.root.GetComponentsInChildren<Camera> ();
        waterPlane = GameObject.FindWithTag ("Overwater");
        ambient = GetComponent <AudioSource> ();
        bubbleMaker = GetComponent <ParticleSystem> ();
    }

    void Update() {
        if (transform.position.y >= 0 && isUnderwater) { //overwater
            RenderSettings.fogColor = normalFogColor;
            RenderSettings.fogDensity = normalFogDensity;
            foreach (GlobalFog depthShaderControl in depthShaderControls) {
                depthShaderControl.enabled = false;
            }
            if (waterPlane) {
                Vector3 scale = waterPlane.transform.localScale;
                scale.y = 1;
                waterPlane.transform.localScale = scale;
				waterPlane.transform.position = new Vector3(
					waterPlane.transform.position.x,
					waterPlane.transform.position.y-1,
					waterPlane.transform.position.z);
            }
            foreach (Camera camera in cameras) {
                camera.clearFlags = CameraClearFlags.Skybox;
            }
            ambient.clip = overwaterAmbient;
            ambient.loop = true;
            ambient.Play ();
            isUnderwater = false;
			// -0.7 .. -0.8
        } else if (transform.position.y < -0.78 && !isUnderwater){ //underwater
            RenderSettings.fogColor = underwaterFogColor;
            RenderSettings.fogDensity = underwaterFogDensity;
            foreach (GlobalFog depthShaderControl in depthShaderControls) {
                depthShaderControl.enabled = true;
                depthShaderControl.height = -transform.position.y;
            }
            if (waterPlane) {
                Vector3 scale = waterPlane.transform.localScale;
				waterPlane.transform.position = new Vector3(
					waterPlane.transform.position.x,
					waterPlane.transform.position.y+1,
					waterPlane.transform.position.z);
                scale.y = -1;
                waterPlane.transform.localScale = scale;
            }
            foreach (Camera camera in cameras) {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.Lerp(UnderSurfaceLight, UnderSurfaceDark, -transform.position.y/120);
            }
            ambient.clip = underwaterAmbient;
            ambient.loop = false;
            ambient.Play ();
            isUnderwater = true;
        }

		if (transform.position.y < -4 && !ambient.isPlaying) {
            ambient.Play ();
            bubbleMaker.Play ();
        }
    }

}
