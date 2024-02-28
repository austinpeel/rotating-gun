// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
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

        // EditorGUILayout.Space(10);

        // if (GUILayout.Button("Fire!"))
        // {
        //     sim.Fire();
        // }
        // if (GUILayout.Button("Move Camera!"))
        // {
        //     sim.TogglePerspective(1);
        // }

        if (GUI.changed)
        {
            sim.SetTimeScale(sim.timeScale);
            sim.SetReferenceFrame(sim.referenceFrame);
            // sim.SetPerspective(sim.perspective);
        }
    }
}
