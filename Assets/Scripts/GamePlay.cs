using UnityEngine;
using System.Collections;

/// <summary>
/// Overall control
/// </summary>
public class GamePlay : MonoBehaviour
{
    public static GamePlay Instance { get; private set; }

    /// <summary>
    /// The main camera in the scene
    /// </summary>
    public Camera m_Camera;

    /// <summary>
    /// Time duration for eath level
    /// </summary>
    public float RoundTime;

    /// <summary>
    /// LayerMask for objects that response to user clicks
    /// </summary>
    public LayerMask m_LayerMaskForClick;

    /// <summary>
    /// A list of car spawn points in the level
    /// </summary>
    public GameObject[] spawner;

    /// <summary>
    /// The maximum amount of cars in game at a given time
    /// </summary>
    public int maxCarCount = 0;

    /// <summary>
    /// The top number of cars for each level (so that at one time the maximum amount of cars won't increase)
    /// </summary>
    public int topCarCount = 30;

    /// <summary>
    /// The number of cars currently in the level
    /// </summary>
    public int carCount = 0;

    /// <summary>
    /// The number of spawners
    /// </summary>
    public int spawnerCount;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        spawner = GameObject.FindGameObjectsWithTag("spawner");
    }

    void Start()
    {
        spawnerCount = spawner.Length;
        Random.InitState((int)Time.time);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, float.MaxValue, m_LayerMaskForClick))
            {
                TrafficLight trafficLight = hitInfo.transform.GetComponent<TrafficLight>();
                if (null != trafficLight)
                {
                    trafficLight.Switch();
                }
            }
        }

        if (maxCarCount <= topCarCount)
        {
            maxCarCount = (int)Time.realtimeSinceStartup;
        }

        if(carCount <= maxCarCount)
        {
            spawner[(int)Random.Range(0, spawnerCount - 0.1f)].GetComponent<VehicleSpawner>().spawnVehicle();
            carCount++;
        }

        print("maxCarCount: " + maxCarCount + " carCount" + carCount);
    }

    public void OnVehicleDestoried()
    {
        carCount--;
    }
}
