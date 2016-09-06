using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SnapToGrid : MonoBehaviour
{
#if UNITY_EDITOR
    public Vector3 offset = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            transform.localPosition = new Vector3(Mathf.Floor(transform.localPosition.x), Mathf.Floor(transform.localPosition.y), Mathf.Floor(transform.localPosition.z)) + offset;
        }
    }
#endif
}
