using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VoxelShapeExploder))]
public class VoxelShapeExploderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VoxelShapeExploder obj = (VoxelShapeExploder)target;
        if (GUILayout.Button("Explode!!"))
        {
            obj.explode();
        }
    }
}
