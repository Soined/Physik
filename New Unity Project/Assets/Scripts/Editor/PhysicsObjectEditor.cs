using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(PhysicsObject))]
public class PhysicsObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PhysicsObject po = (PhysicsObject)target;

        if(po.body == null)
        {
            EditorGUILayout.HelpBox("No Body set for this Object! In that case, the object is unable to collide with anything", MessageType.Warning);
        }

        EditorGUILayout.LabelField("This Component will enable Physics for this object.");

        base.OnInspectorGUI();
    }
}
