using UnityEngine;
using System.Collections;

public class Bezier : MonoBehaviour
{
    /// <summary>
    /// Get a point on a bezier curve
    /// </summary>
    /// <param name="a">control point 1</param>
    /// <param name="b">control point 2</param>
    /// <param name="c">control point 3</param>
    /// <param name="d">control point 4</param>
    /// <param name="t">t from 0 to 1</param>
    /// <returns>the point on the curve</returns>
    public static Vector3 Point(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        return (1 - t) * (1 - t) * (1 - t) * a + 
            3 * (1 - t) * (1 - t) * t * b + 
            3 * (1 - t) * t * t * c + 
            t * t * t * d;
    }
}
