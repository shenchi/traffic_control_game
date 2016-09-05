using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Agent for objects in the way. 
/// Contains information relating to locomotion, and physical calculations
/// Supposed to be the interface between AI and path-finding
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class WayAgent : MonoBehaviour
{
    /// <summary>
    /// A threshold to check if we arrive the end point
    /// </summary>
    private const float ThresholdDistanceToEnd = 0.2f;

    /// <summary>
    /// Max distance when raycast test in front
    /// </summary>
    private const float MaxRaycastDistanceInFront = 10.0f;

    /// <summary>
    /// A delegate for AI script to choose way
    /// </summary>
    /// <param name="label">labal of the way point we just arrived</param>
    /// <param name="numOfWay">count of way that you can choose from</param>
    /// <returns>the way you choose, from 0 to numOfWay - 1 </returns>
    public delegate int WaySelectDelegate(string label, int numOfWay);

    /// <summary>
    /// If a vehicle is still alive on the way
    /// </summary>
    public bool IsAlive { get; private set; }

    /// <summary>
    /// The start point of current segment we are in
    /// </summary>
    public WayPoint StartPoint { get; set; }

    /// <summary>
    /// The end point of current segment we are in
    /// </summary>
    public WayPoint EndPoint { get; private set; }

    /// <summary>
    /// Normalized direction vector to the end point, a suggested direction
    /// </summary>
    public Vector3 Direction { get; private set; }

    /// <summary>
    /// Distance to the end point
    /// </summary>
    public float Distance { get; private set; }

    /// <summary>
    /// Length of this vehicle (collider box);
    /// </summary>
    public float VehicleLength { get; private set; }

    /// <summary>
    /// A callback for user to choose way when arriving a way point with more than 1 outbound.
    /// the first outbound in the waypoint will be chosen if left this callback null.
    /// </summary>
    public WaySelectDelegate WaySelectCallback { get; set; }

    private LayerMask m_VehicleLayerMask;

    private List<Vector3> m_CurvePath = null;
    private List<float> m_CurveLengthRemain = null;
    private float m_CurveLength = 0.0f;
    private int m_CurveSegmentId = -1;

    /// <summary>
    /// Update current position within the path of the way
    /// </summary>
    public void UpdateWayPosition()
    {
        if (null == StartPoint) // we are not in any segment of path
        {
            Direction = Vector3.zero;
            Distance = 0;
            return;
        }

        if (null != StartPoint as EndPoint)
        {
            IsAlive = false;
            if (null != GamePlay.Instance)
            {
                GamePlay.Instance.OnVehicleDestoried();
            }
            Destroy(gameObject);
        }

        if (null == EndPoint)
        {
            SelectNextPoint();

            if (null == EndPoint) // we might arrive the final point
            {
                Direction = Vector3.zero;
                Distance = 0;
                return;
            }
        }

        Vector3 vecToEnd = (EndPoint.transform.position - transform.position);

        if (vecToEnd.magnitude < ThresholdDistanceToEnd)
        {
            StartPoint = EndPoint;
            SelectNextPoint();
        }

        if (null == m_CurvePath)
        {
            Direction = vecToEnd.normalized;
            Distance = vecToEnd.magnitude;
        }
        else
        {
            Vector3 vecToSegEnd = m_CurvePath[m_CurveSegmentId + 1] - transform.position;
            Direction = vecToSegEnd.normalized;
            Distance = vecToSegEnd.magnitude + m_CurveLengthRemain[m_CurveSegmentId + 1];
        }
    }

    void SelectNextPoint()
    {
        int selectedOutbound = null != WaySelectCallback ? WaySelectCallback(StartPoint.m_Label, StartPoint.m_Outbounds.Count) : 0;
        EndPoint = StartPoint.NextWayPoint(selectedOutbound);
        m_CurvePath = StartPoint.GetCurvePathByOutboundIdx(selectedOutbound);
        m_CurveSegmentId = (null != m_CurvePath ? 0 : -1);
        m_CurveLength = 0.0f;
        if (null != m_CurvePath)
        {
            m_CurveLengthRemain = new List<float>();
            m_CurveLengthRemain.Add(0.0f);
            for (int i = m_CurvePath.Count - 2; i >= 0; i--)
            {
                m_CurveLength += (m_CurvePath[i + 1] - m_CurvePath[i]).magnitude;
                m_CurveLengthRemain.Insert(0, m_CurveLength);
            }
            transform.position = m_CurvePath[0];
        }
        else
        {
            m_CurveLengthRemain = null;
        }
    }

    /// <summary>
    /// Move forward in suggested direction
    /// </summary>
    /// <param name="distance">Amount of distance to move</param>
    public void MoveForward(float distance)
    {
        if (null == m_CurvePath)
        {
            transform.position += Direction * distance;
            if (Direction != Vector3.zero)
                transform.forward = Direction;
        }
        else
        {
            float dist = distance;
            float distRemain = (m_CurvePath[m_CurveSegmentId + 1] - transform.position).magnitude;
            int targetSegId = m_CurveSegmentId;
            while (dist > distRemain && targetSegId < m_CurvePath.Count - 2)
            {
                targetSegId++;
                dist -= distRemain;
                distRemain = (m_CurvePath[targetSegId + 1] - m_CurvePath[targetSegId]).magnitude;
            }
            
            Vector3 dir = (m_CurvePath[targetSegId + 1] - m_CurvePath[targetSegId]).normalized;

            Vector3 targetPos = m_CurveSegmentId == targetSegId ? 
                transform.position + dist * dir :
                (m_CurvePath[targetSegId] + dist * dir);
            
            m_CurveSegmentId = targetSegId;

            Vector3 tailPos = transform.position - transform.forward * VehicleLength;
            
            Vector3 targetDir = (targetPos - (transform.position + tailPos) * 0.5f).normalized;

            transform.position = targetPos;
            transform.forward = targetDir;
        }
    }

    /// <summary>
    /// Get current light type of the traffic light
    /// </summary>
    /// <returns>type of the traffic light that is on</returns>
    public TrafficLight.LightType GetCurrentTrafficLight()
    {
        if (null != EndPoint)
        {
            StopLinePoint p = EndPoint as StopLinePoint;
            if (null != p)
            {
                return p.GetTrafficLightType();
            }
        }
        return TrafficLight.LightType.SteadyGreen;
    }

    /// <summary>
    /// Find the vehicle in front of us
    /// </summary>
    /// <returns>the vehicle in front of us. returns null if there is not</returns>
    public bool VehicleInFront(out float distance, out WayAgent agent)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hitInfo, MaxRaycastDistanceInFront, m_VehicleLayerMask))
        {
            agent = hitInfo.transform.GetComponent<WayAgent>();
            if (null != agent)
            {
                distance = hitInfo.distance;
                return true;
            }
        }

        distance = float.MaxValue;
        agent = null;
        return false;
    }

    void Awake()
    {
        m_VehicleLayerMask = LayerMask.GetMask("Vehicle");
        IsAlive = true;
        VehicleLength = (GetComponent<BoxCollider>()).size.z;
    }

    void OnEnable()
    {
        UpdateWayPosition();
    }

    void Start()
    {
        UpdateWayPosition();
    }

    void Update()
    {
        UpdateWayPosition();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<WayAgent>() != null && GamePlay.Instance != null)
        {
            GamePlay.Instance.OnVehicleCollision();
        }
    }

}
