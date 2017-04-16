using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VoxelMapFileHandler))]
public class VoxelMapFileHandlerEditor : Editor
{
    private string[] dimensionOptions = { "32", "64", "128", "256", "512", "1024" };
    private int index = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VoxelMapFileHandler obj = (VoxelMapFileHandler)target;
        index = EditorGUILayout.Popup(index, dimensionOptions);
        if (GUILayout.Button("New"))
        {
            
            var path = EditorUtility.SaveFilePanel(
                    "New Voxel Database",
                    "",
                    "WholeNewWorld",
                    "vdb");
            if (path.Length != 0)
            {
                int size = 2 << (4 + index);
                Debug.Log("GENERATING " + size + "sized voxel map");
                obj.newMap(path, new Vector3(0,0,0), new Vector3(size, size, size));
            }
        }

        else if (GUILayout.Button("Save"))
        {

            var path = EditorUtility.SaveFilePanel(
                    "New Voxel Database",
                    "",
                    "WholeNewWorld",
                    "vdb");
            if (path.Length != 0)
            {
                obj.saveMap(path);
            }
        }

        else if (GUILayout.Button("Load"))
        {

            var path = EditorUtility.OpenFilePanel(
                    "New Voxel Database",
                    "",
                    "vdb");
            if (path.Length != 0)
            {
                obj.loadMap(path);
            }
        }
    }
}
