using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubiquity;
using System;

[ExecuteInEditMode]
public class VoxelArray : VoxelBaseShape {
    private static float maxSize = 16;
    private static uint maxDataLength = 256;
    public float[] data;
    public uint dataLength;
    public VoxelShapeGenerator.ShapeDirection direction;
    public VoxelShapeGenerator.ShapeFront front;
    public VoxelShapeGenerator.ShapeOrientation orientation;



    //cached positions:
    private Vector3 cached_position;
    private Vector2 cached_size;
    private Color cached_color;
    private float[] cached_data;
    private uint cached_data_length;

    private bool dataChanged = false;

    private VoxelShapeGenerator.ShapeDirection cached_direction;
    private VoxelShapeGenerator.ShapeFront cached_front;
    private VoxelShapeGenerator.ShapeOrientation cached_orientation;

    // Use this for initialization
    void Start()
    {
        qc = (QuantizedColor)color;
        cached_color = color;
        data = new float[maxDataLength];
        cached_data = new float[maxDataLength];

        cacheValues();
        generator.generateArrayIn2d(cached_data, dataLength, cached_position, cached_size, cached_direction, cached_front, cached_orientation, qc, false);
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

        if (transform.hasChanged || dataChanged)
        {
            generator.clearArrayIn2d(cached_data, dataLength, cached_position, cached_size, cached_direction, cached_front, cached_orientation, false);
            cacheValues();
            redraw = true;
        }

        if (redraw) generator.generateArrayIn2d(cached_data, dataLength, cached_position, cached_size, cached_direction, cached_front, cached_orientation, qc, false);
    }

    void onDestroy()
    {
        generator.clearArrayIn2d(cached_data, dataLength, cached_position, cached_size, cached_direction, cached_front, cached_orientation, explodeOnDestroy);
    }

    public void refreshData()
    {
        dataChanged = true;
    }

    public override void explode()
    {
        generator.clearArrayIn2d(cached_data, dataLength, cached_position, cached_size, cached_direction, cached_front, cached_orientation, true);
    }

    private void cacheValues()
    {
        cached_position = transform.position;
        cached_direction = direction;
        cached_front = front;
        cached_orientation = orientation;
        System.Array.Copy(data, cached_data, data.Length);
        cached_data_length = dataLength;

        switch (cached_direction)
        {
            case VoxelShapeGenerator.ShapeDirection.UP:
                cached_size.x = transform.lossyScale.x;
                cached_size.y = transform.lossyScale.z;
                break;
            case VoxelShapeGenerator.ShapeDirection.FRONT:
                cached_size.x = transform.lossyScale.x;
                cached_size.y = transform.lossyScale.y;
                break;
            case VoxelShapeGenerator.ShapeDirection.SIDE:
                cached_size.x = transform.lossyScale.z;
                cached_size.y = transform.lossyScale.y;
                break;
            default:
                break;
        }

        transform.hasChanged = false;
        dataChanged = false;
    }
}
