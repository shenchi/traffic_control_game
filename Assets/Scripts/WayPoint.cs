using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Base class of all way points
/// </summary>
public class WayPoint : MonoBehaviour
{
    /// <summary>
    /// Count of segment deviding a bezier curve
    /// </summary>
    private const int BezierSegmentCount = 10;

    /// <summary>
    /// List of all way points that the current one connected to. The 0th in the array should be the default one.
    /// </summary>
    [SerializeField]
    public List<WayPoint> m_Outbounds;

    /// <summary>
    /// Label for this way point, optional
    /// </summary>
    [SerializeField]
    public string m_Label;

    /// <summary>
    /// Mapping from a outbound to a pair of control points, -1 indicates straight line
    /// </summary>
    [SerializeField]
    public List<int> m_OutboundPathMapping;

    /// <summary>
    /// Control points of outbounds, mapped by m_OutboundPathMapping
    /// </summary>
    [SerializeField]
    public List<Vector3> m_StartTangentPoints;
    [SerializeField]
    public List<Vector3> m_EndTangentPoints;

    private Dictionary<int, List<Vector3>> m_CurvePath;

    /// <summary>
    /// Find out a way point as the next point in the path
    /// </summary>
    /// <returns>the way point found</returns>
    public virtual WayPoint NextWayPoint(int index = 0)
    {
        if (null != m_Outbounds && m_Outbounds.Count > index)
        {
            return m_Outbounds[index];
        }

        return null;
    }

    /// <summary>
    /// Get index of control points of a outbound
    /// </summary>
    /// <param name="idx">index of this outbound</param>
    /// <returns>index of control points, -1 if it is a straight line</returns>
    public int GetControlPointsIdxByOutboundIdx(int idx)
    {
        if (idx <= m_Outbounds.Count && idx < m_OutboundPathMapping.Count &&
            m_OutboundPathMapping[idx] >= 0 && m_OutboundPathMapping[idx] < m_StartTangentPoints.Count)
        {
            return m_OutboundPathMapping[idx];
        }

        return -1;
    }

    /// <summary>
    /// Get curve path for a outbound
    /// </summary>
    /// <param name="idx">index of this outbound</param>
    /// <returns>paths of this curve, null if it is straight line</returns>
    public List<Vector3> GetCurvePathByOutboundIdx(int idx)
    {
        int cpIdx = GetControlPointsIdxByOutboundIdx(idx);
        if (cpIdx >= 0)
        {
            return m_CurvePath[cpIdx];
        }
        return null;
    }

    void Awake()
    {
        m_CurvePath = new Dictionary<int, List<Vector3>>();
        for (int i = 0; i < m_Outbounds.Count; i++)
        {
            int cpIdx = GetControlPointsIdxByOutboundIdx(i);
            if (cpIdx < 0)
            {
                continue;
            }

            m_CurvePath.Add(cpIdx, new List<Vector3>());
            for (int t = 0; t <= BezierSegmentCount; t++)
            {
                m_CurvePath[cpIdx].Add(Bezier.Point(transform.position, m_StartTangentPoints[cpIdx], m_EndTangentPoints[cpIdx], m_Outbounds[i].transform.position, t / (float)BezierSegmentCount));
            }
        }
    }

    protected virtual Color gizmoColor
    {
        get
        {
            return Color.blue;
        }
    }

    void OnDrawGizmos()
    {
        DrawGizmos(0.5f, Color.yellow);
    }

    void OnDrawGizmosSelected()
    {
        DrawGizmos(1.0f, Color.white);
    }

    void DrawGizmos(float radius, Color lineColor)
    {
        if (null != m_Outbounds)
        {
            Gizmos.color = lineColor;
            for (int i = 0; i < m_Outbounds.Count; i++)
            {
                if (null == m_OutboundPathMapping || m_OutboundPathMapping[i] < 0 || null == m_CurvePath || null == m_CurvePath[m_OutboundPathMapping[i]])
                {
                    Gizmos.DrawLine(transform.position, m_Outbounds[i].transform.position);
                }
                else
                {
                    var path = m_CurvePath[m_OutboundPathMapping[i]];
                    for (int t = 1; t < path.Count; t++)
                    {
                        Gizmos.DrawLine(path[t - 1], path[t]);
                    }
                }
            }
        }

        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }
}
