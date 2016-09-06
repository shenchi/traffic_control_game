using UnityEngine;
using System.Collections;

public class IndicatorShaderChanger : MonoBehaviour {

    float duration = 0.7f;
    float alpha = 0f;


	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        lerpAlpha();
	}

    void lerpAlpha()
    {
        float lerp = Mathf.PingPong(Time.time, duration) / duration;

        alpha = Mathf.Lerp(0.0f, 0.2f, lerp);

        Color now = this.GetComponent<Renderer>().material.color;

        now.a = alpha;

        this.GetComponent<Renderer>().material.color = now;
    }
}
