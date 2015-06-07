using UnityEngine;
using System.Collections;

public class FogSwitcher : MonoBehaviour {
    public Color normalFogColor = new Color (0.5f, 0.5f, 0.5f, 0.5f);
    public float normalFogDensity = .003f;
    public Color underwaterFogColor = new Color (0.81f, 0.93f, 0.37f, 0.5f);
    public float underwaterFogDensity = .02f;

    public bool isUnderwater = false;

    void Update() {
        Debug.Log ("Depth " + transform.position.y);
        if (transform.position.y >= 0) {
            RenderSettings.fogColor = normalFogColor;
            RenderSettings.fogDensity = normalFogDensity;
        } else {
            RenderSettings.fogColor = underwaterFogColor;
            RenderSettings.fogDensity = underwaterFogDensity;
        }
    }

}
