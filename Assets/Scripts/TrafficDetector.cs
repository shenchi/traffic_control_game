using UnityEngine;
using System.Collections;

public class TrafficDetector : MonoBehaviour {

    public float vehiclePassingTime;
    public float triggerEnterTime;

    public GameObject indicator;

    // Use this for initialization
    void Start ()
    {
        vehiclePassingTime = 0;
        triggerEnterTime = -1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerEnterTime > 0)
        {
            vehiclePassingTime = Time.realtimeSinceStartup - triggerEnterTime;
            indicator.GetComponent<IndicatorShaderChanger>().enabled = vehiclePassingTime >= 1.0f;
        }
    }

    public void OnTriggerEnter()
    {
        triggerEnterTime = Time.realtimeSinceStartup;
    }

    public void OnTriggerExit()
    {
        triggerEnterTime = -1.0f;
        indicator.GetComponent<IndicatorShaderChanger>().enabled = false;
        Color color = indicator.GetComponent<Renderer>().material.color;
        color.a = 0;
        indicator.GetComponent<Renderer>().material.color = color;
    }
}
