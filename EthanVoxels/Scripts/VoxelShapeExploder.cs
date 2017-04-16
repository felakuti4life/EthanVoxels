using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubiquity;

public class VoxelShapeExploder : MonoBehaviour {
    public VoxelBaseShape shape;
    
    public void explode(VoxelBaseShape aShape=null)
    {
        if(aShape != null)
        {
            aShape.explode();
        }
        else if(shape != null)
        {
            shape.explode();
        }
    }

}
