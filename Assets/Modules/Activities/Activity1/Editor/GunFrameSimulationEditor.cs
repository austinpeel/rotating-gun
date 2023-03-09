using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GunFrameSimulation))]
public class GunFrameSimulationEditor : Editor
{
    private GunFrameSimulation sim;

    private void OnEnable()
    {
        sim = target as GunFrameSimulation;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUI.changed)
        {
            sim.SetTimeScale(sim.timeScale);
            // sim.SetReferenceFrame(sim.referenceFrame);
            sim.SetPerspective(sim.perspective);
        }
    }
}
