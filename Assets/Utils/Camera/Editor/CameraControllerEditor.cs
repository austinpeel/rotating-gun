using UnityEditor;
using UnityEngine;

namespace Utils
{
    [CustomEditor(typeof(CameraController))]
    public class CameraControllerEditor : Editor
    {
        private CameraController controller;

        private void OnEnable()
        {
            controller = target as CameraController;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Set Camera"))
            {
                controller.SetCamera();
            }
        }
    }
}
