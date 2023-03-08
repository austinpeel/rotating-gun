using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DraggableVector))]
public class VectorEditorEditor : Editor
{
    private Vector vector;

    private void OnEnable()
    {
        vector = target as DraggableVector;
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
