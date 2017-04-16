using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VoxelShapeGenerator))]
public class VoxelShapeGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VoxelShapeGenerator obj = (VoxelShapeGenerator)target;
        if (GUILayout.Button("Clear!"))
        {
            obj.clear();
        }
    }
}