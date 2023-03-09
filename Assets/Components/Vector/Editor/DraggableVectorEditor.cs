using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DraggableVector))]
public class DraggableVectorEditor : Editor
{
    private DraggableVector vector;

    private Vector3 components;
    private Color color;

    private void OnEnable()
    {
        vector = target as DraggableVector;
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector fields
        DrawDefaultInspector();

        // Check if properties have been changed in the inspector
        if (components != vector.components)
        {
            vector.Redraw();
            components = vector.components;
        }

        if (color != vector.color)
        {
            vector.SetColor();
            vector.SetClickZoneColors();
            color = vector.color;
        }
    }
}
