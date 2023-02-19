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
            // sim.SetTimeScale(sim.timeScale);
            sim.SetReferenceFrame(sim.referenceFrame, 0);
            // sim.SetPerspective(sim.perspective);
        }
    }
}
