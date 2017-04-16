using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubiquity;

[ExecuteInEditMode]
public class VoxelCube : VoxelBaseShape {
    private static float maxSize = 16;

    

    //cached positions:
    private Vector3 cached_position;
    private Vector3 cached_size;
    private Color cached_color;

    // Use this for initialization
    void Start()
    {
        qc = (QuantizedColor)color;
        cached_color = color;
        cached_position = transform.position;
        cached_size = transform.lossyScale;
        generator.generateRect3(cached_position, cached_size, qc, false);
        transform.hasChanged = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool redraw = false;
        if (!color.Equals(cached_color))
        {
            qc = (QuantizedColor)color;
            cached_color = color;
            redraw = true;
        }

        if (transform.hasChanged)
        {
            generator.clearRect3(cached_position, cached_size, false);
            cached_position = transform.position;
            cached_size = transform.lossyScale;
            //checkSizeBounds();

            transform.hasChanged = false;
            redraw = true;
        }

        if (redraw)
        {
            Debug.Log("Drawing cube at " + cached_position + " with a size of " + cached_size);
            generator.generateRect3(cached_position, cached_size, qc, false);
        }
    }

    void OnDestroy()
    {
        generator.clearRect3(cached_position, cached_size, explodeOnDestroy);
    }

    public override void explode()
    {
        generator.clearRect3(cached_position, cached_size, true);
    }

    private void checkSizeBounds()
    {
        cached_size.x = Mathf.Min(cached_size.x, maxSize);
        cached_size.y = Mathf.Min(cached_size.y, maxSize);
        cached_size.z = Mathf.Min(cached_size.z, maxSize);
    }
}
