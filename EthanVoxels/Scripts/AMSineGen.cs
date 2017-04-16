using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AMSineGen : MonoBehaviour {
    private static uint maxLength = 256;

    public VoxelArray voxelArray;

    public float frequency = 10f;
    public float ModFrequency = 0.2f;
    public uint length;

    private float[] data;
    private float t = 0f;

	// Use this for initialization
	void Start () {
        data = new float[maxLength];
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < length; i++)
        {
            data[i] = Mathf.Sin(t * ModFrequency * 2.0f * Mathf.PI) * Mathf.Sin(t * frequency * 2.0f * Mathf.PI);
            t += Time.deltaTime;
        }

        if(voxelArray != null)
        {
            System.Array.Copy(data, voxelArray.data, length);
            voxelArray.dataLength = length;
            voxelArray.refreshData();
        }
	}
}
