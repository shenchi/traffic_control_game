using UnityEngine;
using System.Collections;

/// <summary>
/// AI for debug, DO NOT use
/// </summary>
public class CarAITestJustGo : MonoBehaviour
{
    public float speed;

    public float gas_acceleration = 10.0f;

    public float brake_acceleration = -20.0f;

    private WayAgent agent;

    //public float iniTime;

    void Awake()
    {
        agent = GetComponent<WayAgent>();
        agent.WaySelectCallback = WaySelect;
        //iniTime = Time.realtimeSinceStartup;
    }

    // Use this for initialization
    void Start()
    {

    }

    bool NeedStop()
    {
        // Check if there is a car in front of us
        WayAgent agentInFront;
        float distInFront;
        bool stopForCar = false;
        if (agent.VehicleInFront(out distInFront, out agentInFront))
        {
            
            if (Vector3.Dot(agentInFront.Direction, agent.Direction) > 0.98)
            {
                stopForCar = (distInFront < 4);
            }
        }
        // Check that if we are far enough from next point or the traffic light is green
        return ((agent.GetCurrentTrafficLight() != TrafficLight.LightType.SteadyGreen) && agent.Distance < 3) || stopForCar;

    }

    void Gas()
    {
        if (speed <= 10.0f)
        {
            speed += gas_acceleration * Time.deltaTime;
        }
    }

    void Brake()
    {
        if (speed > 0)
        {
            speed += brake_acceleration * Time.deltaTime;
        }
        else
        {
            speed = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NeedStop())
        {
            Brake();
        }
        else
        {
            Gas();
        }

        float dist = speed * Time.deltaTime;
        agent.MoveForward(dist);
    }

    int WaySelect(string label, int count)
    {
        return count > 0 ? count - 1 : 0;
    }
}
