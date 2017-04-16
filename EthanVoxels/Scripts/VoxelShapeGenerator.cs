using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubiquity;


[ExecuteInEditMode]
public class VoxelShapeGenerator : MonoBehaviour {
    public enum ShapeDirection { UP, FRONT, SIDE };
    public enum ShapeFront { UP_FRONT_OR_RIGHT, DOWN_BACK_OR_LEFT };
    public enum ShapeOrientation { HORIZONTAL, VERTICAL };

    private ColoredCubesVolume volume;
    private ColoredCubesVolumeData data;

    private Vector3 mapPosition;

    private static QuantizedColor TransparentColor = new QuantizedColor(0, 0, 0, 0);


    private Material fakeVoxelMaterial;
    private Texture diffuseMap;
    // Use this for initialization
    void Start () {
        mapPosition = transform.position;

        //get terrain
        volume = GetComponent<ColoredCubesVolume>();
        data = volume.data;

        fakeVoxelMaterial = Resources.Load("Materials/FakeColoredCubes", typeof(Material)) as Material;
        diffuseMap = volume.GetComponent<ColoredCubesVolumeRenderer>().material.GetTexture("_DiffuseMap");
    }

    public void clear()
    {
        int width = (data.enclosingRegion.upperCorner.x - data.enclosingRegion.lowerCorner.x) + 1;
        int height = (data.enclosingRegion.upperCorner.y - data.enclosingRegion.lowerCorner.y) + 1;
        int depth = (data.enclosingRegion.upperCorner.z - data.enclosingRegion.lowerCorner.z) + 1;
        QuantizedColor c = TransparentColor;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < depth; z++)
                    drawVoxel(x, y, z, c, false);
    }

    public void generateCircle(Vector3 position, float radius, bool filledIn, ShapeDirection dir, QuantizedColor color, bool explode)
    {
        int r = (int)radius;
        int x0 = (int)position.x;
        int y0 = (int)position.y;
        int z0 = (int)position.z;


        int x = r;
        int y = 0;
        int err = 0;
        //TODO: handle switches for ShapeDirection (ugh)
        while (x >= y)
        {
            drawVoxel(x0 + x, y0 + y, z0, color, explode);
            drawVoxel(x0 + y, y0 + x, z0, color, explode);
            drawVoxel(x0 - y, y0 + x, z0, color, explode);
            drawVoxel(x0 - x, y0 + y, z0, color, explode);
            drawVoxel(x0 - x, y0 - y, z0, color, explode);
            drawVoxel(x0 - y, y0 - x, z0, color, explode);
            drawVoxel(x0 + y, y0 - x, z0, color, explode);
            drawVoxel(x0 + x, y0 - y, z0, color, explode);

            if (err <= 0)
            {
                y += 1;
                err += 2 * y + 1;
            }
            if (err > 0)
            {
                x -= 1;
                err -= 2 * x + 1;
            }
        }

        if(filledIn)
        {
            r--;
            while (r > 0)
            {
                x = r;
                y = 0;
                err = 0;
                while (x >= y)
                {
                    drawVoxel(x0 + x, y0 + y, z0, color, explode);
                    drawVoxel(x0 + y, y0 + x, z0, color, explode);
                    drawVoxel(x0 - y, y0 + x, z0, color, explode);
                    drawVoxel(x0 - x, y0 + y, z0, color, explode);
                    drawVoxel(x0 - x, y0 - y, z0, color, explode);
                    drawVoxel(x0 - y, y0 - x, z0, color, explode);
                    drawVoxel(x0 + y, y0 - x, z0, color, explode);
                    drawVoxel(x0 + x, y0 - y, z0, color, explode);

                    if (err <= 0)
                    {
                        y += 1;
                        err += 2 * y + 1;
                    }
                    if (err > 0)
                    {
                        x -= 1;
                        err -= 2 * x + 1;
                    }
                }
                r--;
            }
        }
    }

    public void clearCircle(Vector3 position, float radius, bool filledIn, ShapeDirection dir, bool explode) { generateCircle(position, radius, filledIn, dir, TransparentColor, explode); }

    public void generateSphere(Vector3 position, float radius, QuantizedColor color, bool explode)
    {
        int r = Mathf.FloorToInt(radius);
        int x = (int)position.x;
        int y = (int)position.y;
        int z = (int)position.z;

        for (int tx = -r; tx < r + 1; tx++)
        {
            for (int ty = -r; ty < r + 1; ty++)
            {
                for (int tz = -r; tz < r + 1; tz++)
                {
                    if (Mathf.Sqrt(Mathf.Pow(tx, 2) + Mathf.Pow(ty, 2) + Mathf.Pow(tz, 2)) <= r - 2)
                    {
                        drawVoxel(tx + x, ty + y, tz + z, color, explode);
                    }
                }
            }
        }
    }

    public void clearSphere(Vector3 position, float radius, bool explode) { generateSphere(position, radius, TransparentColor, explode); }


    public void generateRect2(Vector3 position, Vector2 size, ShapeDirection dir, QuantizedColor color, bool explode)
    {
        int rx = Mathf.FloorToInt(size.x) / 2;
        int ry = Mathf.FloorToInt(size.y) / 2;

        //static offset on whatever plane we're drawing on:
        int pos = 0;

        int xMin = 0;
        int yMin = 0;

        switch (dir)
        {
            case ShapeDirection.UP:
                xMin = Mathf.FloorToInt(position.x) - rx;
                yMin = Mathf.FloorToInt(position.z) - ry;
                pos = Mathf.FloorToInt(position.y);
                break;
            case ShapeDirection.FRONT:
                xMin = Mathf.FloorToInt(position.x) - rx;
                yMin = Mathf.FloorToInt(position.y) - ry;
                pos = Mathf.FloorToInt(position.z);
                break;
            case ShapeDirection.SIDE:
                xMin = Mathf.FloorToInt(position.z) - rx;
                yMin = Mathf.FloorToInt(position.y) - ry;
                pos = Mathf.FloorToInt(position.x);
                break;
            default:
                break;
        }

        int xMax = xMin + rx * 2;
        int yMax = yMin + ry * 2;

        int x = xMin;
        int y = yMin;

        Debug.Log("rectangle x bounds: " + xMin + " to " + xMax + " y bounds: " + yMin + " to " + yMax);

        while(x <= xMax)
        {
            while(y <= yMax)
            {
                switch (dir)
                {
                    case ShapeDirection.UP:
                        drawVoxel(x, pos, y, color, explode);
                        break;
                    case ShapeDirection.FRONT:
                        drawVoxel(x, y, pos, color, explode);
                        break;
                    case ShapeDirection.SIDE:
                        drawVoxel(pos, y, x, color, explode);
                        break;
                    default:
                        break;
                }
                y++;
            }
            y = yMin;
            x++;
        }
    }

    public void clearRect2(Vector3 position, Vector2 size, ShapeDirection dir, bool explode) { generateRect2(position, size, dir, TransparentColor, explode); }

    public void generateRect3(Vector3 position, Vector3 size, QuantizedColor color, bool explode)
    {
        //start at bottom left front corner:
        int xMin = Mathf.CeilToInt(position.x - size.x / 2);
        int yMin = Mathf.CeilToInt(position.y - size.y / 2);
        int zMin = Mathf.CeilToInt(position.z - size.z / 2);

        int xMax = Mathf.FloorToInt(position.x + size.x / 2);
        int yMax = Mathf.FloorToInt(position.y + size.y / 2);
        int zMax = Mathf.FloorToInt(position.z + size.z / 2);

        int x = xMin;
        int y = yMin;
        int z = zMin;
        
        //draw bottom face:
        while(x <= xMax)
        {
           while(z <= zMax)
            {
                drawVoxel(x, y, z, color, explode);
                z++;
            }
            z = zMin;
            x++;
        }

        //draw side faces by turtling our way around a square on each level of y:
        y++;
        z = zMax;
        while(y < yMax)
        {
            while(x > xMin)
            {
                drawVoxel(x, y, z, color, explode);
                x--;
            }

            while(z > zMin)
            {
                drawVoxel(x, y, z, color, explode);
                z--;
            }

            while(x < xMax)
            {
                drawVoxel(x, y, z, color, explode);
                x++;
            }

            while(z < zMax)
            {
                drawVoxel(x, y, z, color, explode);
                z++;
            }
            y++;
        }

        //finally, draw the top face.
        x = xMin;
        z = zMin;
        while (x <= xMax)
        {
            while (z <= zMax)
            {
                drawVoxel(x, y, z, color, explode);
                z++;
            }
            z = zMin;
            x++;
        }
    }

    public void clearRect3(Vector3 position, Vector3 size, bool explode) { generateRect3(position, size, TransparentColor, explode); }

    public void generateArrayIn2d(float[] array, uint arrayLength, Vector3 position, Vector2 size, ShapeDirection dir, ShapeFront front, ShapeOrientation orientation, QuantizedColor color, bool explode)
    {
        int y0 = 0;
        int xMin = 0;

        int y = 0;
        int x = 0;
        int idx = 0;

        //plane we're drawing on:
        int pos = 0;

        //how many voxels we need for each value in the array:
        int voxelsPerValue = Mathf.FloorToInt(size.x / arrayLength);
        //if we end up getting 0, then just round up to 1.
        if (voxelsPerValue <= 0) voxelsPerValue++;

        float val = 0.0f;

        switch (dir)
        {
            case ShapeDirection.UP:
                y0 = Mathf.FloorToInt(position.x);
                xMin = Mathf.FloorToInt(position.z - size.x / 2);
                pos = Mathf.FloorToInt(position.y);
                break;
            case ShapeDirection.FRONT:
                y0 = Mathf.FloorToInt(position.y);
                xMin = Mathf.FloorToInt(position.x - size.x / 2);
                pos = Mathf.FloorToInt(position.z);
                break;
            case ShapeDirection.SIDE:
                y0 = Mathf.FloorToInt(position.y);
                xMin = Mathf.FloorToInt(position.z - size.x / 2);
                pos = Mathf.FloorToInt(position.x);
                break;
            default:
                break;
        }
        y = y0;
        x = xMin;
            
        //draw array:
        for(idx = 0; idx < arrayLength; idx++)
        {
            //if we are rendering facing the back or left, we reverse the draw.
            if(front == ShapeFront.DOWN_BACK_OR_LEFT && (dir == ShapeDirection.FRONT || dir == ShapeDirection.SIDE))
            {
                val = array[arrayLength - idx];
            }
            else
            {
                val = array[idx];
            }

            //if we are rendering facing downwards, we just negate the array:
            if(front == ShapeFront.DOWN_BACK_OR_LEFT && dir == ShapeDirection.FRONT)
            {
                y = y0 - Mathf.FloorToInt(val * size.y);
            }
            else
            {
                y = y0 + Mathf.FloorToInt(val * size.y);
            }
            //to do: if voxels per value is more than 1, draw squares rather than single voxels
            //Debug.Log("Drawing value " + val + " at x: " + x + " y: " + y);
            if (orientation == ShapeOrientation.HORIZONTAL)
            {
                switch (dir)
                {
                    case ShapeDirection.UP:
                        drawVoxel(y, pos, x, color, explode);
                        break;
                    case ShapeDirection.FRONT:
                        drawVoxel(x, y, pos, color, explode);
                        break;
                    case ShapeDirection.SIDE:
                        drawVoxel(pos, y, x, color, explode);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (dir)
                {
                    case ShapeDirection.UP:
                        drawVoxel(x, pos, y, color, explode);
                        break;
                    case ShapeDirection.FRONT:
                        drawVoxel(y, x, pos, color, explode);
                        break;
                    case ShapeDirection.SIDE:
                        drawVoxel(pos, x, y, color, explode);
                        break;
                    default:
                        break;
                }
            }
            x += voxelsPerValue;
        }   
        
    }

    public void clearArrayIn2d(float[] array, uint arrayLength, Vector3 position, Vector2 size, ShapeDirection dir, ShapeFront front, ShapeOrientation orientation, bool explode) { generateArrayIn2d(array, arrayLength, position, size, dir, front, orientation, TransparentColor, explode); }

    public void drawVoxel(int x, int y, int z, QuantizedColor color, bool explode)
    {
        if(explode)
        {
            ExplodeVoxel(x, y, z);
        }
        data.SetVoxel(x, y, z, color);
        
    }

    private Vector3 getOffsetInCubemap(Vector3 pos)
    {
        return pos + mapPosition;
    }

    //from Cubiquity class' ClickToDestroy:
    public bool IsSurfaceVoxel(int x, int y, int z)
    {
        QuantizedColor quantizedColor;

        quantizedColor = volume.data.GetVoxel(x, y, z);
        if (quantizedColor.alpha < 127) return false;

        quantizedColor = volume.data.GetVoxel(x + 1, y, z);
        if (quantizedColor.alpha < 127) return true;

        quantizedColor = volume.data.GetVoxel(x - 1, y, z);
        if (quantizedColor.alpha < 127) return true;

        quantizedColor = volume.data.GetVoxel(x, y + 1, z);
        if (quantizedColor.alpha < 127) return true;

        quantizedColor = volume.data.GetVoxel(x, y - 1, z);
        if (quantizedColor.alpha < 127) return true;

        quantizedColor = volume.data.GetVoxel(x, y, z + 1);
        if (quantizedColor.alpha < 127) return true;

        quantizedColor = volume.data.GetVoxel(x, y, z - 1);
        if (quantizedColor.alpha < 127) return true;

        return false;
    }

    //from Cubiquity class' ClickToDestroy. generate a cube to go flying:
    void ExplodeVoxel(int xPos, int yPos, int zPos)
    {
        // Set up a material which we will apply to the cubes which we spawn to replace destroyed voxels.
        
        if (diffuseMap != null)
        {
            List<string> keywords = new List<string> { "DIFFUSE_TEXTURE_ON" };
            fakeVoxelMaterial.shaderKeywords = keywords.ToArray();
            fakeVoxelMaterial.SetTexture("_DiffuseMap", diffuseMap);
        }
        fakeVoxelMaterial.SetTexture("_NormalMap", volume.GetComponent<ColoredCubesVolumeRenderer>().material.GetTexture("_NormalMap"));
        fakeVoxelMaterial.SetFloat("_NoiseStrength", volume.GetComponent<ColoredCubesVolumeRenderer>().material.GetFloat("_NoiseStrength"));

                   
                        // Get the current color of the voxel
                        QuantizedColor color = volume.data.GetVoxel(xPos, yPos, zPos);

                        // Check the alpha to determine whether the voxel is visible. 
                        if (color.alpha > 127)
                        {
                            Vector3i voxel = new Vector3i(xPos, yPos, zPos);

                            if (IsSurfaceVoxel(xPos, yPos, zPos))
                            {
                                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                cube.AddComponent<Rigidbody>();
                                cube.transform.parent = volume.transform;
                                cube.transform.localPosition = new Vector3(xPos, yPos, zPos);
                                cube.transform.localRotation = Quaternion.identity;
                                cube.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                                cube.GetComponent<Renderer>().material = fakeVoxelMaterial;
                                cube.GetComponent<Renderer>().material.SetColor("_CubeColor", (Color32)color);
                                cube.GetComponent<Renderer>().material.SetVector("_CubePosition", new Vector4(xPos, yPos, zPos, 0.0f));

                                Vector3 explosionForce = cube.transform.position - new Vector3(xPos, yPos, zPos);

                                // These are basically random values found through experimentation.
                                // They just add a bit of twist as the cubes explode which looks nice
                                float xTorque = (xPos * 1436523.4f) % 56.0f;
                                float yTorque = (yPos * 56143.4f) % 43.0f;
                                float zTorque = (zPos * 22873.4f) % 38.0f;

                                Vector3 up = new Vector3(0.0f, 2.0f, 0.0f);

                                cube.GetComponent<Rigidbody>().AddTorque(xTorque, yTorque, zTorque);
                                cube.GetComponent<Rigidbody>().AddForce((explosionForce.normalized + up) * 100.0f);

                                // Cubes are just a temporary visual effect, and we delete them after a few seconds.
                                float lifeTime = Random.Range(8.0f, 12.0f);
                                Destroy(cube, lifeTime);
            }
        }
    }
}
