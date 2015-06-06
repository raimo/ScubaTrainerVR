using UnityEngine;
using System.Collections;

public class FogSwitcher : MonoBehaviour {
    public Color normalFogColor = new Color (0.5f, 0.5f, 0.5f, 0.5f);
    public float normalFogDensity = .003f;
    public Color underwaterFogColor = new Color (0.81f, 0.93f, 0.37f, 0.5f);
    public float underwaterFogDensity = .02f;

    void OnCollisionEnter(Collision collision) {
        Debug.Log ("Collision Enter");
    }

    void OnCollisionExit(Collision collision) {
        Debug.Log ("Collision Exit");
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log ("Trigger Enter " + other.tag);
        if (other.tag == "Underwater") {
            Debug.Log ("Trigger Underwater");
            RenderSettings.fogColor = underwaterFogColor;
            RenderSettings.fogDensity = underwaterFogDensity;
        }
        if (other.tag == "Overwater") {
            Debug.Log ("Trigger Overwater");
            RenderSettings.fogColor = normalFogColor;
            RenderSettings.fogDensity = normalFogDensity;
        }

    }

    void OnTriggerExit(Collider other) {
        Debug.Log ("Trigger Exit");
    }
}
