using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpawnPoint))]
public class VehicleSpawner : MonoBehaviour
{
    /// <summary>
    /// List of prefabs of vehicle to be spawned
    /// </summary>
    public WayAgent[] m_Prefabs;

    public float m_SpawnInterval = 2.0f;

    private float m_SpawnTimer = 0.0f;

    void Update()
    {
        //m_SpawnTimer += Time.deltaTime;
        //
        //if (m_SpawnTimer > m_SpawnInterval)
        //{
        //    m_SpawnTimer -= m_SpawnInterval;
        //
        //    var vehicle = Instantiate<WayAgent>(m_Prefabs[0]);
        //    vehicle.transform.position = transform.position;
        //    vehicle.transform.rotation = transform.rotation;
        //    vehicle.StartPoint = GetComponent<SpawnPoint>();
        //}
    }

    public void spawnVehicle()
    {
        var vehicle = Instantiate<WayAgent>(m_Prefabs[0]);
        vehicle.transform.position = transform.position;
        vehicle.transform.rotation = transform.rotation;
        vehicle.StartPoint = GetComponent<SpawnPoint>();
    }
}
