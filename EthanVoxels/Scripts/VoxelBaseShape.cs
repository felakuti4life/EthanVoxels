using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubiquity;

public abstract class VoxelBaseShape : MonoBehaviour {
    public VoxelShapeGenerator generator;

    public Color color;
    public bool explodeOnDestroy;

    protected QuantizedColor qc;

    public abstract void explode();
}
