using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RotatingGunSimulation))]
public class RotatingGunSimulationEditor : Editor
{
    private RotatingGunSimulation sim;

    private void OnEnable()
    {
        sim = target as RotatingGunSimulation;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Fire!"))
        {
            sim.Fire();
        }
        if (GUILayout.Button("Move Camera!"))
        {
            sim.TogglePerspective(1);
        }

        if (GUI.changed)
        {
            sim.SynchronizeReferenceFrame();
            sim.SynchronizeTimeScale();
            sim.SetPerspective(sim.perspective);
        }
    }
}
