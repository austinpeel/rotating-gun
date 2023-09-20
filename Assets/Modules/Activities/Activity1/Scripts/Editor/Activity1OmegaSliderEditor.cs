using UnityEditor;

[CustomEditor(typeof(Activity1OmegaSlider))]
public class Activity1OmegaSliderEditor : CenteredSliderEditor
{
    SerializedProperty allowedValues;

    protected override void OnEnable()
    {
        base.OnEnable();

        allowedValues = serializedObject.FindProperty("allowedValues");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.PropertyField(allowedValues);
        serializedObject.ApplyModifiedProperties();
    }
}
