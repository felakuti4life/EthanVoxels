using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VoxelArray))]
public class VoxelArrayInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VoxelArray obj = (VoxelArray)target;
        if (GUILayout.Button("Refresh Data"))
        {
            obj.refreshData();
        }
    }
}