using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Bezier
{
    // Uncomment order to enable editor functionality. 
    // Comment Out if editor functionality is needed
    [ExecuteInEditMode]
    [RequireComponent(typeof(Transform))]
    public class BezierSpline : MonoBehaviour
    {
        // Exposing a read-only view of the list for safe external access
        public IReadOnlyList<Vector3> ControlPoints => controlPoints.AsReadOnly();
        public Vector3 GetControlPoint(int index) => controlPoints[index];
        public int ControlPointCount => controlPoints.Count;
        public int CurveCount => (controlPoints.Count - 1) / 3;

        [SerializeField]
        private List<Vector3> controlPoints;

        [SerializeField]
        private List<BezierControlPointMode> pointModes;

        // Private fields for data storage and caching
        [SerializeField]
        private bool isLooped;

        [Tooltip("Offset position applied to new control points")]
        public Vector3 NewPointOffset = Vector3.forward;

        // Fields for performance caching
        private float cachedSplineLength;
        private bool isLengthDirty = true;

        // --- Initialization and Validation ---

        private void Awake()
        {
            // Ensure data is initialized at runtime
            Initialize();
        }

        private void OnValidate()
        {
            // Ensure data is initialized when the component is first added or properties change in the editor.            
            Initialize();
        }

        private void Initialize()
        {
            // Ensure the lists are instantiated.
            controlPoints ??= new List<Vector3>();
            pointModes ??= new List<BezierControlPointMode>();

            // If the list is empty (first time creation), call Reset to populate it.
            if (ControlPointCount == 0)
            {
                Reset();
            }
        }

        // --- Public Properties ---

        public bool Loop
        {
            get => isLooped;
            set
            {
                if (isLooped == value) return;
                isLooped = value;
                if (value)
                {
                    pointModes[pointModes.Count - 1] = pointModes[0];
                    SetControlPoint(0, controlPoints[0]);
                }
                isLengthDirty = true;
            }
        }

        // Optimized SplineLength getter using caching
        public float SplineLength
        {
            get
            {
                if (isLengthDirty)
                {
                    cachedSplineLength = CalculateSplineLength();
                    isLengthDirty = false;
                }
                return cachedSplineLength;
            }
        }

        // --- Core Spline Functions ---

        public Vector3 GetPoint(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = controlPoints.Count - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }

            return transform.TransformPoint(BezierUtils.GetPoint(
                controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3], t));
        }

        public Vector3 GetVelocity(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = controlPoints.Count - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            return transform.TransformPoint(BezierUtils.GetFirstDerivative(
                controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3], t)) - transform.position;
        }

        public Vector3 GetTangent(float t) => GetVelocity(t).normalized;

        public Vector3 GetBinormal(float t)
        {
            Vector3 tangentDirection = GetTangent(t);
            Vector3 upVector = Vector3.up;
            return Vector3.Cross(upVector, tangentDirection);
        }

        public Vector3 GetNormal(float t)
        {
            Vector3 tangentDirection = GetTangent(t);
            Vector3 binormalDirection = GetBinormal(t);
            return Vector3.Cross(tangentDirection, binormalDirection);
        }

        // --- Editor Modification Functions ---

        public void Reset()
        {
            // Use new List<T>() for initialization
            controlPoints = new List<Vector3> {
                new(1f, 0f, 0f), new(2f, 0f, 0f),
                new(3f, 0f, 0f), new(4f, 0f, 0f)
            };
            pointModes = new List<BezierControlPointMode> {
                BezierControlPointMode.Free, BezierControlPointMode.Free
            };
            isLengthDirty = true;
        }

        public void AddCurve()
        {
            Vector3 lastPoint = controlPoints[^1];

            if (NewPointOffset.magnitude <= 0.1f)
            {
                Debug.LogWarning("New Point Offset is too small. Last and New Point might overlap.", this);
            }

            // Add the three new control points to the list
            for (int i = 0; i < 3; i++)
            {
                lastPoint += NewPointOffset;
                controlPoints.Add(lastPoint);
            }

            // Add new mode inherited from the previous segment's mode
            pointModes.Add(pointModes[^1]);
            EnforceMode(controlPoints.Count - 4);

            if (isLooped)
            {
                controlPoints[^1] = controlPoints[0];
                pointModes[^1] = pointModes[0];
                EnforceMode(0);
            }

            isLengthDirty = true;
        }

        public void RemoveCurve()
        {
            if (CurveCount <= 2)
            {
                throw new System.Exception("Cannot remove the last curve segment!");
            }

            int lastIndex = controlPoints.Count - 1;

            // Remove the last three control points
            for (int i = 0; i < 3; i++)
            {
                controlPoints.RemoveAt(lastIndex - i);
            }

            pointModes.RemoveAt(pointModes.Count - 1);

            if (isLooped)
            {
                pointModes[^1] = pointModes[0];

                // The first and last anchors are always the same when looped.
                // The new last anchor (P0 of the new last segment) must match the start anchor (controlPoints[0]).
                // We set the position to ensure the loop integrity is maintained.
                controlPoints[^1] = controlPoints[0];

                // Enforce mode on the start point to update its handle based on the new last handle.
                EnforceMode(0);
            }

            isLengthDirty = true;
        }

        public BezierControlPointMode GetControlPointMode(int controlPointIndex)
        {
            return pointModes[(controlPointIndex + 1) / 3];
        }

        public void SetControlPointMode(int controlPointIndex, BezierControlPointMode mode)
        {
            int modeSegmentIndex = (controlPointIndex + 1) / 3;
            pointModes[modeSegmentIndex] = mode;

            if (isLooped)
            {
                if (modeSegmentIndex == 0)
                {
                    pointModes[^1] = mode;
                }
                else if (modeSegmentIndex == pointModes.Count - 1)
                {
                    pointModes[0] = mode;
                }
            }
            EnforceMode(controlPointIndex);
            isLengthDirty = true;
        }

        public void SetControlPoint(int index, Vector3 newPosition)
        {
            if (index % 3 == 0)
            {
                Vector3 positionDelta = newPosition - controlPoints[index];

                if (isLooped)
                {
                    if (index == 0)
                    {
                        controlPoints[1] += positionDelta;
                        controlPoints[^2] += positionDelta;
                        controlPoints[^1] = newPosition;
                    }
                    else if (index == controlPoints.Count - 1)
                    {
                        controlPoints[0] = newPosition;
                        controlPoints[1] += positionDelta;
                        controlPoints[index - 1] += positionDelta;
                    }
                    else
                    {
                        controlPoints[index - 1] += positionDelta;
                        controlPoints[index + 1] += positionDelta;
                    }
                }
                else
                {
                    if (index > 0)
                    {
                        controlPoints[index - 1] += positionDelta;
                    }
                    if (index + 1 < controlPoints.Count)
                    {
                        controlPoints[index + 1] += positionDelta;
                    }
                }
            }

            controlPoints[index] = newPosition;
            EnforceMode(index);
            isLengthDirty = true;
        }

        // --- Private Helpers ---

        private float CalculateSplineLength()
        {
            float accumulatedDistance = 0f;
            Vector3 previousPoint = GetPoint(0);

            const float lengthStepSize = 0.01f;

            for (float t = lengthStepSize; t <= 1f; t += lengthStepSize)
            {
                Vector3 currentPoint = GetPoint(t);
                Vector3 segmentDelta = currentPoint - previousPoint;

                accumulatedDistance += segmentDelta.magnitude;
                previousPoint = currentPoint;
            }
            return accumulatedDistance;
        }

        private void EnforceMode(int controlPointIndex)
        {
            int modeSegmentIndex = (controlPointIndex + 1) / 3;
            BezierControlPointMode mode = pointModes[modeSegmentIndex];

            if (mode == BezierControlPointMode.Free || !isLooped && (modeSegmentIndex == 0 || modeSegmentIndex == pointModes.Count - 1))
            {
                return;
            }

            int middleAnchorIndex = modeSegmentIndex * 3;
            int fixedPointIndex, enforcedPointIndex;

            if (controlPointIndex <= middleAnchorIndex)
            {
                fixedPointIndex = middleAnchorIndex - 1;
                if (fixedPointIndex < 0)
                {
                    fixedPointIndex = controlPoints.Count - 2;
                }
                enforcedPointIndex = middleAnchorIndex + 1;
                if (enforcedPointIndex >= controlPoints.Count)
                {
                    enforcedPointIndex = 1;
                }
            }
            else
            {
                fixedPointIndex = middleAnchorIndex + 1;
                if (fixedPointIndex >= controlPoints.Count)
                {
                    fixedPointIndex = 1;
                }
                enforcedPointIndex = middleAnchorIndex - 1;
                if (enforcedPointIndex < 0)
                {
                    enforcedPointIndex = controlPoints.Count - 2;
                }
            }

            Vector3 anchorPosition = controlPoints[middleAnchorIndex];
            Vector3 fixedTangent = anchorPosition - controlPoints[fixedPointIndex];

            if (mode == BezierControlPointMode.Aligned)
            {
                fixedTangent = fixedTangent.normalized * Vector3.Distance(anchorPosition, controlPoints[enforcedPointIndex]);
            }
            controlPoints[enforcedPointIndex] = anchorPosition + fixedTangent;
        }
    }
}