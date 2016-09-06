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
    private GameObject[] spawners;

    /// <summary>
    /// The maximum amount of cars in game at a given time
    /// </summary>
    private int maxCarCount = 0;

    /// <summary>
    /// The top number of cars for each level (so that at one time the maximum amount of cars won't increase)
    /// </summary>
    private int topCarCount = 0;

    /// <summary>
    /// The number of cars currently in the level
    /// </summary>
    private int carCount = 0;

    /// <summary>
    /// The number of spawners
    /// </summary>
    private int spawnerCount;

    private bool gameOver;
    private float timeSinceLevelStart;

    void Awake()
    {
        Instance = this;
        spawners = GameObject.FindGameObjectsWithTag("spawner");
        Time.timeScale = 1.0f;
    }

    void Start()
    {
        gameOver = false;
        timeSinceLevelStart = 0.0f;
        spawnerCount = spawners.Length;
        Random.InitState((int)System.DateTime.Now.Ticks);
        topCarCount = 16;
    }

    void Update()
    {
        timeSinceLevelStart += Time.deltaTime;

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
            maxCarCount = (int)(timeSinceLevelStart * 0.5f);
        }

        if (carCount <= maxCarCount)
        {
            var spawner = spawners[(int)Random.Range(0, spawnerCount - 0.1f)].GetComponent<VehicleSpawner>();
            if (spawner.CanSpawn)
            {
                spawner.spawnVehicle();
                carCount++;
            }
        }
        if (timeSinceLevelStart >= 60) {
            Time.timeScale = 0;
            GameObject.Find("UICtrl").GetComponent<UICtroller>().win();
        }


    }

    public void OnVehicleCollision()
    {
        GameObject.Find("UICtrl").GetComponent<UICtroller>().gameisover();
        gameOver = true;
    }

    public void OnVehicleDestoried()
    {
        carCount--;
    }
    public float getGameTime() {
        return timeSinceLevelStart;
    }


}
