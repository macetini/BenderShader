using Assets.Scripts.Bezier;
using UnityEditor;
using UnityEngine;

// Define the editor for the BezierSpline component
[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{    
    private const float HandleSize = 0.1f;
    private const float PickSize = 0.01f;

    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;
    private int selectedIndex = -1;

    // Called when the editor is first enabled/opened
    private void OnEnable()
    {
        spline = target as BezierSpline;

        // Ensure the spline is initialized when the inspector starts
        if (spline != null && spline.ControlPointCount == 0)
        {
            spline.Reset();
        }
    }

    // Called whenever the Inspector GUI is drawn
    public override void OnInspectorGUI()
    {
        spline = target as BezierSpline;

        // Draw the default inspector properties first
        DrawDefaultInspector();

        if (spline == null) return;

        // --- Custom Spline Properties and Buttons ---

        // Draw Loop property
        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Toggle Loop");
            spline.Loop = loop;
        }

        GUILayout.Label($"Curves: {spline.CurveCount}, Points: {spline.ControlPointCount}", EditorStyles.boldLabel);

        // Draw Add Curve button
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            selectedIndex = -1;
        }

        // Draw Reset button
        if (GUILayout.Button("Reset Spline"))
        {
            Undo.RecordObject(spline, "Reset Spline");
            spline.Reset();
            selectedIndex = -1;
        }

        // Display selected point info
        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
        {
            DrawSelectedPointInspector();
        }
    }

    // Called for drawing in the Scene View
    private void OnSceneGUI()
    {
        spline = target as BezierSpline;
        if (spline == null) return;

        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        // Defensive check: If the spline is not initialized (less than 4 points), do not draw handles.
        if (spline.ControlPointCount < 4)
        {
            Handles.Label(spline.transform.position, "Spline component is not initialized.");
            return;
        }

        // Draw and handle the anchor and control points
        for (int i = 0; i < spline.ControlPointCount; i++)
        {
            ShowPoint(i);
        }

        // Draw the spline lines
        DrawSplineLines();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(spline);
        }
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Space(10);
        // FIX APPLIED HERE: Replaced EditorStyles.boldHeader with EditorStyles.boldLabel
        EditorGUILayout.LabelField("Selected Point", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        // Get the current control point position
        Vector3 point = spline.GetControlPoint(selectedIndex);

        // Draw the position field
        EditorGUI.BeginChangeCheck();
        point = EditorGUILayout.Vector3Field("Position", point);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            spline.SetControlPoint(selectedIndex, point);
        }

        // Draw the point mode ONLY if it is an anchor point (index % 3 == 0)
        if (selectedIndex % 3 == 0)
        {
            EditorGUI.BeginChangeCheck();
            BezierControlPointMode mode =
                (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Change Point Mode");
                spline.SetControlPointMode(selectedIndex, mode);
            }
        }

        EditorGUI.indentLevel--;
    }

    private void ShowPoint(int index)
    {
        // Get point position in world space
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
        // Use HandleUtility.GetHandleSize(point) to make handles scale with distance
        float size = HandleUtility.GetHandleSize(point) * HandleSize;

        // Reduce size for control points (index % 3 != 0)
        if (index % 3 != 0)
        {
            size *= 0.5f;
        }

        // Determine handle color based on selection and point type
        Handles.color = index == selectedIndex ? Color.yellow : (index % 3 == 0 ? Color.white : Color.cyan);

        // Draw a selectable handle
        if (Handles.Button(point, handleRotation, size, PickSize * HandleUtility.GetHandleSize(point), Handles.DotHandleCap))
        {
            selectedIndex = index;
            Repaint(); // Force the Inspector window to update
        }

        // If selected, draw a move handle and update position
        if (selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");

                // Convert back to local space before setting
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
            }
        }        
    }

    private void DrawSplineLines()
    {
        int numCurves = spline.CurveCount;
        for (int i = 0; i < numCurves; i++)
        {
            DrawCurve(i);
        }
    }

    private void DrawCurve(int curveIndex)
    {
        Handles.color = Color.gray;

        int anchorIndex = curveIndex * 3;

        // Get the four control points for the Bezier curve segment
        Vector3 p0 = handleTransform.TransformPoint(spline.GetControlPoint(anchorIndex));
        Vector3 p1 = handleTransform.TransformPoint(spline.GetControlPoint(anchorIndex + 1));
        Vector3 p2 = handleTransform.TransformPoint(spline.GetControlPoint(anchorIndex + 2));
        Vector3 p3 = handleTransform.TransformPoint(spline.GetControlPoint(anchorIndex + 3));

        // Draw the control lines
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p2, p3);

        // Draw the Bezier curve
        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
    }
}