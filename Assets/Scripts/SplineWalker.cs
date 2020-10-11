using UnityEngine;

public class SplineWalker : MonoBehaviour
{
    public BezierSpline spline;
    public Tiles tiles;

    void Start()
    {
        //int originPointId = Shader.PropertyToID("_OriginPoint");
        //Shader.SetGlobalVector(originPointId, spline.Points[0]);

        Vector3[] points3D = spline.Points;
        Vector4[] points4D = new Vector4[spline.Points.Length];

        for (int i = 0; i < points3D.Length; i++)
        {
            points4D[i] = new Vector4(points3D[i].x, points3D[i].y, points3D[i].z, 0f);
        }

        int splinePointsCountId = Shader.PropertyToID("_SplinePointsCount");
        Shader.SetGlobalInt(splinePointsCountId, points4D.Length);

        int splineLengthId = Shader.PropertyToID("_SplineLength");
        Shader.SetGlobalFloat(splineLengthId, spline.SplineLength); 

        int splinePointsId = Shader.PropertyToID("_SplinePoints");
        Shader.SetGlobalVectorArray(splinePointsId, points4D);        

        int worldLengthId = Shader.PropertyToID("_WorldLength");
        Shader.SetGlobalFloat(worldLengthId,  tiles.GetTotalLength());

        Debug.Log("Spline Points Count: " + points4D.Length);
        Debug.Log("Spline Length: " + spline.SplineLength);
        Debug.Log("World Length: " + tiles.GetTotalLength());
    }

    void Update()
    {        
    }
}