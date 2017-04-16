using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubiquity;

[ExecuteInEditMode]
public class VoxelSphere : VoxelBaseShape {
    private static float maxSize = 16;

    //cached positions:
    private Vector3 cached_position;
    private float cached_radius;
    private Color cached_color;

    // Use this for initialization
    void Start () {
        qc = (QuantizedColor)color;
        cached_color = color;
        cached_position = transform.position;
        cached_radius = transform.lossyScale.x;
        generator.generateSphere(cached_position, cached_radius, qc, false);
        transform.hasChanged = false;
	}
	
	// Update is called once per frame
	void Update () {
        bool redraw = false;
        if(!color.Equals(cached_color))
        {
            qc = (QuantizedColor)color;
            cached_color = color;
            redraw = true;
        }

		if(transform.hasChanged)
        {
            generator.clearSphere(cached_position, cached_radius, false);
            cached_position = transform.position;
            cached_radius = Mathf.Min(transform.lossyScale.x, maxSize);
            
            transform.hasChanged = false;
            redraw = true;
        }

        if(redraw) generator.generateSphere(cached_position, cached_radius, qc, false);
    }

    void onDestroy()
    {
        generator.clearSphere(cached_position, cached_radius, explodeOnDestroy);
    }

    public override void explode()
    {
        generator.clearSphere(cached_position, cached_radius, true);
    }
}
