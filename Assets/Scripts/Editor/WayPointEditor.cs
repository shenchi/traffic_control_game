using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(WayPoint))]
public class WayPointEditor : Editor
{
    SerializedProperty labelProp;
    SerializedProperty outboundProp;
    SerializedProperty outboundMappingProp;
    SerializedProperty startTanPointsProp;
    SerializedProperty endTanPointsProp;

    protected virtual void OnEnable()
    {
        labelProp = serializedObject.FindProperty("m_Label");
        outboundProp = serializedObject.FindProperty("m_Outbounds");
        outboundMappingProp = serializedObject.FindProperty("m_OutboundPathMapping");
        startTanPointsProp = serializedObject.FindProperty("m_StartTangentPoints");
        endTanPointsProp = serializedObject.FindProperty("m_EndTangentPoints");

        serializedObject.Update();
        Refresh();
        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void OnSceneGUI()
    {
        serializedObject.Update();

        WayPoint point = target as WayPoint;

        for (int i = 0; i < outboundProp.arraySize; i++)
        {
            var outbound = outboundProp.GetArrayElementAtIndex(i).objectReferenceValue as WayPoint;
            int cpIdx = point.GetControlPointsIdxByOutboundIdx(i);

            if (cpIdx < 0)
            {
                continue;
            }

            bool changed = false;
            Vector3 sp = startTanPointsProp.GetArrayElementAtIndex(cpIdx).vector3Value;
            Vector3 ep = endTanPointsProp.GetArrayElementAtIndex(cpIdx).vector3Value;

            Vector3 newSp = Handles.PositionHandle(sp, Quaternion.identity);
            if (newSp != sp)
            {
                changed = true;
            }

            Vector3 newEp = Handles.PositionHandle(ep, Quaternion.identity);
            if (newEp != ep)
            {
                changed = true;
            }

            if (changed)
            {
                startTanPointsProp.GetArrayElementAtIndex(cpIdx).vector3Value = newSp;
                endTanPointsProp.GetArrayElementAtIndex(cpIdx).vector3Value = newEp;
            }

            Handles.DrawBezier(point.transform.position, outbound.transform.position, newSp, newEp, Color.white, null, HandleUtility.GetHandleSize(Vector3.zero));
        }

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        WayPoint point = target as WayPoint;

        EditorGUILayout.PropertyField(labelProp, new GUIContent("Label (Optional)"));

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            outboundProp.InsertArrayElementAtIndex(outboundProp.arraySize);
        }

        if (GUILayout.Button("-", GUILayout.Width(20)) && outboundProp.arraySize > 0)
        {
            outboundProp.arraySize--;
        }

        if (GUILayout.Button("Refresh", GUILayout.Width(60)))
        {
            Refresh();
            serializedObject.ApplyModifiedProperties();
            return;
        }

        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < outboundProp.arraySize; i++)
        {
            var outbound = outboundProp.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(outbound, new GUIContent(i.ToString()));
            GUILayout.Label(IsStrightOutbound(point, outbound.objectReferenceValue as WayPoint) ? "Straight" : "Turn", GUILayout.Width(60));

            EditorGUILayout.EndHorizontal();
        }

        Refresh();

        serializedObject.ApplyModifiedProperties();
    }

    protected void Refresh()
    {
        WayPoint point = target as WayPoint;
        bool changed = false;
        Dictionary<int, Vector3> startTanMap = new Dictionary<int, Vector3>();
        Dictionary<int, Vector3> endTanMap = new Dictionary<int, Vector3>();

        for (int i = 0; i < outboundProp.arraySize; i++)
        {
            var outbound = outboundProp.GetArrayElementAtIndex(i);
            var outboundObj = outbound.objectReferenceValue as WayPoint;
            var cpIdx = point.GetControlPointsIdxByOutboundIdx(i);
            if (!IsStrightOutbound(point, outboundObj))
            {
                if (cpIdx < 0)
                {
                    startTanMap.Add(i, point.transform.position + point.transform.forward * 5);
                    endTanMap.Add(i, outboundObj.transform.position - outboundObj.transform.forward * 5);
                    changed = true;
                }
                else
                {
                    startTanMap.Add(i, point.m_StartTangentPoints[cpIdx]);
                    endTanMap.Add(i, point.m_EndTangentPoints[cpIdx]);
                }
            }
            else
            {
                if (cpIdx >= 0)
                {
                    changed = true;
                }
            }
        }

        if (outboundMappingProp.arraySize != outboundProp.arraySize)
        {
            changed = true;
        }

        if (changed)
        {
            outboundMappingProp.ClearArray();
            startTanPointsProp.ClearArray();
            endTanPointsProp.ClearArray();

            for (int i = 0; i < outboundProp.arraySize; i++)
            {
                if (startTanMap.ContainsKey(i))
                {
                    startTanPointsProp.InsertArrayElementAtIndex(startTanPointsProp.arraySize);
                    startTanPointsProp.GetArrayElementAtIndex(startTanPointsProp.arraySize - 1).vector3Value = startTanMap[i];

                    endTanPointsProp.InsertArrayElementAtIndex(endTanPointsProp.arraySize);
                    endTanPointsProp.GetArrayElementAtIndex(endTanPointsProp.arraySize - 1).vector3Value = endTanMap[i];

                    outboundMappingProp.InsertArrayElementAtIndex(outboundMappingProp.arraySize);
                    outboundMappingProp.GetArrayElementAtIndex(outboundMappingProp.arraySize - 1).intValue = startTanPointsProp.arraySize - 1;
                }
                else
                {
                    outboundMappingProp.InsertArrayElementAtIndex(outboundMappingProp.arraySize);
                    outboundMappingProp.GetArrayElementAtIndex(outboundMappingProp.arraySize - 1).intValue = -1;
                }
            }

        }
    }

    public static bool IsStrightOutbound(WayPoint a, WayPoint b)
    {
        return Vector3.Dot(a.transform.forward, b.transform.forward) > 0.98 &&
            Vector3.Dot(a.transform.forward, b.transform.position - a.transform.position) > 0.98;
    }
}

[CustomEditor(typeof(SpawnPoint))]
public class SpawnPointEditor : WayPointEditor { }

[CustomEditor(typeof(EndPoint))]
public class EndPointEditor : WayPointEditor { }

[CustomEditor(typeof(StopLinePoint))]
public class StopLinePointEditor : WayPointEditor { }
