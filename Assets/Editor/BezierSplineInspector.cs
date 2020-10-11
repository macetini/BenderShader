using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{
    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;
    
    private const float worldVectorsScale = 15f;
    private const int stepsPerCurve = 10;

    private const float handleSize = 0.1f;
    private const float pickSize = 0.1f;

    private int selectedIndex = -1;

    private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    private void OnSceneGUI()
    {
        spline = target as BezierSpline;
        EditorGUI.BeginChangeCheck();

        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < spline.ControlPointCount; i += 3)
        {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;

            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);

            p0 = p3;
        }
        
        ShowWorldVectors();
    }

    private void ShowWorldVectors()
    {
        int steps = stepsPerCurve * spline.CurveCount;
        for (int i = 0; i <= steps; i++)
        {
            float step = i / (float)steps;

            Vector3 point = spline.GetPoint(step);

            Handles.color = Color.red;
            Vector3 tangent = spline.GetTangent(step);
            Handles.DrawLine(point, point + tangent * worldVectorsScale);

            Handles.color = Color.blue;
            Vector3 binormal = spline.GetBinormal(step);
            Handles.DrawLine(point, point + binormal * worldVectorsScale);

            Handles.color = Color.green;
            Vector3 normal = spline.GetNormal(step);
            Handles.DrawLine(point, point + normal * worldVectorsScale);
        }
    }

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));

        float size = HandleUtility.GetHandleSize(point);

        if (index == 0)
        {
            size *= 2f;
        }

        Handles.color = modeColors[(int)spline.GetControlPointMode(index)];

        if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.SphereHandleCap))
        {
            selectedIndex = index;
            Repaint();
        }

        if (selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
            }
        }

        return point;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
        {
            DrawSelectedPointInspector();
        }

        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPoint(selectedIndex, point);
        }

        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Change Point Mode");
            spline.SetControlPointMode(selectedIndex, mode);
            EditorUtility.SetDirty(spline);
        }
    }
}