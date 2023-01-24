using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Vector))]
public class VectorEditor : Editor
{
    private Vector vector;

    private void OnEnable()
    {
        vector = target as Vector;
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector fields
        DrawDefaultInspector();

        // Check if the any fields have been changed
        if (GUI.changed)
        {
            vector.Redraw();
        }
    }
}
