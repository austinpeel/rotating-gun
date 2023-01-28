using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Slides.CameraController))]
public class CameraControllerEditor : Editor
{
    private Slides.CameraController cameraController;

    private void OnEnable()
    {
        cameraController = target as Slides.CameraController;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Set Camera Position / Rotation"))
        {
            Transform camera = Camera.main.transform;
            camera.position = cameraController.targetPosition;
            camera.rotation = Quaternion.Euler(cameraController.targetRotation);
        }
    }
}
