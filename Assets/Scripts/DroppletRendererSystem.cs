using UnityEngine;
using Unity.Entities;

public class DroppletRendererSystem : ComponentSystem
{
    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer colorBuffer;

    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    Settings settings;

    int instanceCount;
    private int cachedInstanceCount = -1;

    protected override void OnCreate()
    {

        settings = GameObject.FindObjectOfType<Settings>();
        instanceCount = settings.side * settings.side * settings.side;
        argsBuffer = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();
    }

    protected override void OnUpdate()
    {
        UpdateBuffers();
        Graphics.DrawMeshInstancedIndirect(settings.droppletMesh, 0, settings.droppletMaterial, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer);
    }

    protected override void OnDestroy()
    {
        if (positionBuffer != null) positionBuffer.Release();
        positionBuffer = null;

        if (colorBuffer != null) colorBuffer.Release();
        colorBuffer = null;

        if (argsBuffer != null) argsBuffer.Release();
        argsBuffer = null;
    }


    private void UpdateBuffers()
    {
        UpdatePositions();
        UpdateColors();

        // indirect args
        uint numIndices = (settings.droppletMesh != null) ? (uint)settings.droppletMesh.GetIndexCount(0) : 0;
        args[0] = numIndices;
        args[1] = (uint)instanceCount;
        argsBuffer.SetData(args);

        cachedInstanceCount = instanceCount;
    }

    private void UpdatePositions()
    {
       
        if (instanceCount < 1) instanceCount = 1;

        // Positions & Colors
        if (positionBuffer != null) positionBuffer.Release();

        positionBuffer = new ComputeBuffer(instanceCount, 4 * 4);

        Vector4[] positions = new Vector4[instanceCount];
        Vector4[] colors = new Vector4[instanceCount];
       
        for (int i = 0; i < instanceCount; i++)
        {
            int side = 100;
            float size = 1;
            positions[i] = new Vector4(i%side , (i/side)%side, (i/ (side*side))%side , size);
           // positions[i] = new Vector4(Random.value * side, Random.value * side, Random.value * side, size);
            if (i == 103) Debug.Log(positions[i].x);
        }

        positionBuffer.SetData(positions);

        settings.droppletMaterial.SetBuffer("positionBuffer", positionBuffer);
       // settings.droppletMaterial.SetBuffer("colorBuffer", colorBuffer);



    }
    private void UpdateColors()
    {
        
        Vector3 p = settings.cam.position;
        //int x = instanceCount;
        int side = 100;

        if (colorBuffer != null)
        {
            colorBuffer.Release();
        }

        colorBuffer = new ComputeBuffer(instanceCount, 4 * 4);

        Vector4[] colors = new Vector4[instanceCount];
        Vector3 cp = settings.cam.position;
        Vector3 ld = settings.sun.forward;
        for (int i = 0; i < instanceCount; i++)
        {
           
            Vector3 pos = new Vector3(i % side, (i / side) % side, (i / (side * side)) % side);
            float angle = Vector3.Angle(ld, pos -cp );
            if (angle < 35 || angle > 40)
            {
                colors[i] = Vector4.zero;
            }
            else
            {
                float hue = (angle - 30) / 10;
                Color cl = Color.HSVToRGB(1-hue, 0.81f,0.81f);
                colors[i] = new Vector4(cl.r, cl.g,cl.b, 0.13f);
            }
        }
       colorBuffer.SetData(colors);
       settings.droppletMaterial.SetBuffer("colorBuffer", colorBuffer);
    }
}
