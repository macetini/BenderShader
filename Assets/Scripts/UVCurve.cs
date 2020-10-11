using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UVCurve : MonoBehaviour
{
    public BezierSpline spline;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] points3D = spline.Points;
        Vector4[] points4D = new Vector4[spline.Points.Length];

        for (int i = 0; i < points3D.Length; i++)
        {
            points4D[i] = new Vector4(points3D[i].x, points3D[i].y, points3D[i].z, 0f);
        }

        int splinePointsCountId = Shader.PropertyToID("_SplinePointsCount");
        Shader.SetGlobalInt(splinePointsCountId, points4D.Length);

        int splineLengthId = Shader.PropertyToID("_SplineLength");
        float splineLength = spline.SplineLength;
        Shader.SetGlobalFloat(splineLengthId, splineLength);

        int splinePointsId = Shader.PropertyToID("_SplinePoints");
        Shader.SetGlobalVectorArray(splinePointsId, points4D);

        Debug.Log("Spline Length: " + splineLength);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] points3D = spline.Points;
        Vector4[] points4D = new Vector4[spline.Points.Length];

        for (int i = 0; i < points3D.Length; i++)
        {
            points4D[i] = new Vector4(points3D[i].x, points3D[i].y, points3D[i].z, 0f);
        }

        int splinePointsId = Shader.PropertyToID("_SplinePoints");
        Shader.SetGlobalVectorArray(splinePointsId, points4D);
    }
}
