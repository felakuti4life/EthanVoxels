using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Cubiquity;

public class VoxelMapFileHandler : MonoBehaviour {
    public ColoredCubesVolume volume;

    void Start()
    {
        volume = GetComponent<ColoredCubesVolume>();
    }

    public void newMap(string path, Vector3 origin, Vector3 size)
    {
        Region r = new Region((int) origin.x, (int)origin.y, (int)origin.z, (int)size.x, (int)size.y, (int)size.z);
        
        volume.data = VolumeData.CreateEmptyVolumeData<ColoredCubesVolumeData>(r, path);
    }

    public void loadMap(string path)
    {
        volume.data = VolumeData.CreateFromVoxelDatabase<ColoredCubesVolumeData>(path);
    }

    public void saveMap(string path)
    {
        File.Copy(volume.data.fullPathToVoxelDatabase, path);
    }
}
