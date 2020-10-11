using UnityEngine;
using Controllers.Curve.Meta;

namespace Controllers.Curve
{
    /// <summary>
    /// The controller class that sets the uniform values for the <b>Curve World</b> shader material properties.
    /// </summary>
    [ExecuteInEditMode]
    public class CurveController : MonoBehaviour
    {
        [Tooltip("The top vertex of the curve parabola. Usually the player position.")]
        public Transform curveOrigin;
        /*
        [Tooltip("Amount of curvature along X axis in XZ plane (left and right).")]
        [Range(-100f, 100f)]
        public float curveAlongX = 0f;

        [Tooltip("Amount of curvature along Y axis in YZ plane (up and down).")]
        [Range(-100f, 100f)]
        public float curveAlongY = 0f;*/

        public BezierSpline spline;

        public float duration;

        public GameObject tilesContainer;

        /*[Tooltip("The curve origin offset along Z axis in YZ plane.")]
        [Range(-100f, 100f)]
        public float horizon = 0f;*/

        public Vector2 BendAmountDiluted;// => new Vector3(curveAlongX * CurveShaderMeta.BEND_AMOUNT_DILUTER, curveAlongY * CurveShaderMeta.BEND_AMOUNT_DILUTER);

        /*[Tooltip("Curve deformation cut off from the curve origin. The items are not deformed beyond this point.")]
        [Range(0f, 50f)]
        public float falloff = 0f;*/

        /// <summary>
        /// Helper variable passed to the shader.
        /// </summary>
        private Vector3 bendAmount = Vector3.zero;

        /// <summary>
        /// Meda data for the <b>WorldShader</b> values.
        /// </summary>
        private CurveShaderMeta shaderMeta = new CurveShaderMeta();

        /// <summary>
        /// Length of a line used to better mark the player/property position. Only used during development.
        /// </summary>
        private const float gizmoLineHeight = 10f;

        private const float gizmoSphereRadius = 0.5f;        

        private float progress;

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
            Shader.SetGlobalFloat(worldLengthId, 91.44f);

            Debug.Log("Spline Points Count: " + points4D.Length);
            Debug.Log("Spline Length: " + spline.SplineLength);
            //Debug.Log("World Length: " + 182.88f);
        }

        void Update()
        {
            progress += Time.deltaTime / duration;
            if (progress > 1f)
            {
                progress = 1f;
            }

            Vector3 progressPoint = spline.GetPoint(progress);

            //tilesContainer.transform.position -= new Vector3(0.0f, 0.0f, progressPoint.z / 100);

            int splineProgressId = Shader.PropertyToID("_splineProgress");
            Shader.SetGlobalFloat(splineProgressId, progress);
                        
            Vector3 splinePoint = spline.GetPoint(progress);

            bendAmount.x = splinePoint.x;
            bendAmount.y = splinePoint.y;
            bendAmount.z = splinePoint.z;

            BendAmountDiluted.x = bendAmount.x;
            BendAmountDiluted.y = bendAmount.y;

            Vector3 origin = curveOrigin.position;
            
            //origin.z += horizon;

            //shaderMeta.BendAroundOrigin(bendAmount, origin);
            //shaderMeta.SetFallOff(falloff);
        }

        private void OnDrawGizmos()
        {
            MarkOriginPosition();

            //if (horizon != 0f) MarkHorizonProperty(horizon, Color.blue);
           // if (falloff != 0f) MarkFallOffProperty(falloff, Color.grey);
        }

        /// <summary>
        /// Draws a Gizmo Origin point visual (sphere and line under).
        /// </summary>
        private void MarkOriginPosition()
        {
            Gizmos.color = Color.green;
            Vector3 position = curveOrigin.position;

            Vector3 lineTop = new Vector3(position.x, position.y + gizmoLineHeight, position.z);
            Vector3 lineBottom = new Vector3(position.x, position.y, position.z);

            Gizmos.DrawLine(lineTop, lineBottom);

            Vector3 spherBottom = new Vector3(position.x, position.y + gizmoLineHeight, position.z);
            Gizmos.DrawSphere(spherBottom, gizmoSphereRadius);
        }

        /// <summary>
        /// Draws a Gizmo Horizon point visual (sphere and line through).
        /// </summary>
        /// <param name="protpertyPostion">Offset on Z axis from a curve origin.</param>
        /// <param name="color">Color of a visual.</param>
        private void MarkHorizonProperty(float protpertyPostion, Color color)
        {
            Vector3 originPosition = curveOrigin.position;

            Vector3 lineTop = new Vector3(originPosition.x - bendAmount.x, originPosition.y - bendAmount.y - gizmoLineHeight * 0.5f, originPosition.z + protpertyPostion);
            Vector3 lineBottom = new Vector3(originPosition.x - bendAmount.x, originPosition.y - bendAmount.y + gizmoLineHeight * 0.5f, originPosition.z + protpertyPostion);

            Gizmos.color = color;
            Gizmos.DrawLine(lineTop, lineBottom);

            Vector3 spherPosition = new Vector3(originPosition.x - bendAmount.x, originPosition.y - bendAmount.y, originPosition.z + protpertyPostion);

            Gizmos.DrawSphere(spherPosition, gizmoSphereRadius);

            Gizmos.DrawLine(originPosition, spherPosition);
        }

        // <summary>
        /// Draws a Gizmo Offset point visual (sphere and line through).
        /// </summary>
        /// <param name="protpertyPostion">Offset on Z axis from a curve origin.</param>
        /// <param name="color">Color of a visual.</param>
        private void MarkFallOffProperty(float protpertyPosition, Color color)
        {
            Vector3 originPosition = curveOrigin.position;

            Vector3 lineTop = new Vector3(originPosition.x, originPosition.y - gizmoLineHeight * 0.5f, originPosition.z + protpertyPosition);
            Vector3 lineBottom = new Vector3(originPosition.x, originPosition.y + gizmoLineHeight * 0.5f, originPosition.z + protpertyPosition);

            Gizmos.color = color;
            Gizmos.DrawLine(lineTop, lineBottom);

            Vector3 spherPosition = new Vector3(originPosition.x, originPosition.y, originPosition.z + protpertyPosition);

            Gizmos.DrawSphere(spherPosition, gizmoSphereRadius);
            Gizmos.DrawLine(originPosition, spherPosition);

            spherPosition = new Vector3(originPosition.x, originPosition.y, originPosition.z + protpertyPosition);
            Gizmos.DrawSphere(spherPosition, gizmoSphereRadius);
        }
    }
}