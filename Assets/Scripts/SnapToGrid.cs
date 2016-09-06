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
            transform.position = new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y), Mathf.Floor(transform.position.z)) + offset;
        }
    }
#endif
}
