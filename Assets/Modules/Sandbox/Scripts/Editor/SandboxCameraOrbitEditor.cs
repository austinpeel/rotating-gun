using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SandboxCameraOrbit))]
public class SandboxCameraOrbitEditor : Editor
{
    SerializedProperty orbitTarget;
    SerializedProperty canOrbit;
    SerializedProperty orbitSpeed;
    SerializedProperty minPolarAngle;
    SerializedProperty maxPolarAngle;
    SerializedProperty clampAzimuthalAngle;
    SerializedProperty minAzimuthalAngle;
    SerializedProperty maxAzimuthalAngle;


    private void OnEnable()
    {
        orbitTarget = serializedObject.FindProperty("target");
        canOrbit = serializedObject.FindProperty("canOrbit");
        orbitSpeed = serializedObject.FindProperty("orbitSpeed");
        minPolarAngle = serializedObject.FindProperty("minPolarAngle");
        maxPolarAngle = serializedObject.FindProperty("maxPolarAngle");
        clampAzimuthalAngle = serializedObject.FindProperty("clampAzimuthalAngle");
        minAzimuthalAngle = serializedObject.FindProperty("minAzimuthalAngle");
        maxAzimuthalAngle = serializedObject.FindProperty("maxAzimuthalAngle");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(orbitTarget);

        if (orbitTarget.objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(canOrbit);
            if (canOrbit.boolValue)
            {
                EditorGUILayout.PropertyField(orbitSpeed);
                EditorGUILayout.PropertyField(minPolarAngle);
                EditorGUILayout.PropertyField(maxPolarAngle);
                EditorGUILayout.PropertyField(clampAzimuthalAngle);

                if (clampAzimuthalAngle.boolValue)
                {
                    EditorGUILayout.PropertyField(minAzimuthalAngle);
                    EditorGUILayout.PropertyField(maxAzimuthalAngle);
                }
            }
        }
        else
        {
            canOrbit.boolValue = false;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
