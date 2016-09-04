using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpawnPoint))]
public class VehicleSpawner : MonoBehaviour
{
    /// <summary>
    /// List of prefabs of vehicle to be spawned
    /// </summary>
    public WayAgent[] m_Prefabs;

    public float m_SpawnCoolingDown = 2.0f;

    private float m_CoolingDownTimer = 0;

    void Update()
    {
        if (m_CoolingDownTimer > 0.0f)
        {
            m_CoolingDownTimer -= Time.deltaTime;
        }
    }

    public bool CanSpawn
    {
        get
        {
            return m_CoolingDownTimer <= 0.0f;
        }
    }

    public void spawnVehicle()
    {
        if (!CanSpawn)
        {
            return;
        }

        m_CoolingDownTimer = m_SpawnCoolingDown;
        var vehicle = Instantiate<WayAgent>(m_Prefabs[0]);
        vehicle.transform.position = transform.position;
        vehicle.transform.rotation = transform.rotation;
        vehicle.StartPoint = GetComponent<SpawnPoint>();
    }
}
