using GPUInstancer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GpuInstanceTK : MonoBehaviour
{
    public GPUInstancerPrefabManager prefabManager;
    public GPUInstancerPrefabPrototype prefabPrototype;
    public GPUInstancerPrefab prefab;
    public GameObject obj;
    public GameObject GPU;
    public static GpuInstanceTK Instance;
    public int instanceCount = 10;
    public float areaSize = 100f;
    public bool ifrun;
    public bool ifDraw;
    public int spwancoun;

    public Matrix4x4[] matrixArray;
    void Start()
    {
        if (ifrun)
        {
            Debug.Log("has started GPUinstancer");
            for (int i = 0; i < instanceCount; i++)
            {
                GPUInstancerPrefabManager.Instantiate(obj, new Vector3(Random.Range(-20, 20), 0, 0), Quaternion.identity);//ins GPUInstancerAPI.InitializeGPUInstancer(prefabManager, true);
                GPUInstancerAPI.InitializePrototype(prefabManager, prefabPrototype, 1024, 0);
            }
            Debug.Log(GPU.GetComponent<GPUInstancerPrefabManager>().prototypeList[0]);
        }
        if (ifDraw)
        {
            DrawGpuMonster();
        
        }
    }
    private void Awake()
    {
        Instance = this;
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void DrawGpuMonster()
    {

        matrixArray = new Matrix4x4[spwancoun];
        for (int i = 0; i < matrixArray.Length; i++)
        {
            matrixArray[i] = Matrix4x4.TRS(new Vector3(Random.Range(-1000, 1000), 1, Random.Range(-1000, 1000)), Quaternion.identity, new Vector3(1, 1, 1));
          
        }
        GPUInstancerAPI.InitializeWithMatrix4x4Array(prefabManager,prefab.prefabPrototype, matrixArray);
        GPUInstancerAPI.SetInstanceCount(prefabManager,prefabPrototype, spwancoun);
        GPUInstancerAPI.UpdateVisibilityBufferWithMatrix4x4Array(prefabManager, prefab.prefabPrototype, matrixArray,
              0, 0, spwancoun);
    }
    public void DrawGpuMonster(NativeArray<float4x4> matrix)
    {
       
        // Debug.Log("has to start darw the GpuMoster");
         var useMatrix = ConvertToMatrix4x4Array(matrix); 
        GPUInstancerAPI.InitializeWithMatrix4x4Array(prefabManager, prefab.prefabPrototype, useMatrix);
        GPUInstancerAPI.SetInstanceCount(prefabManager, prefabPrototype, matrix.Length);
        GPUInstancerAPI.UpdateVisibilityBufferWithNativeArray(prefabManager, prefab.prefabPrototype, matrix,
              0, 0, matrix.Length);
    }
    public void DrawGpuMonster(NativeArray<float4x4> matrix,Matrix4x4[] useMatrix)
    {
        if (matrix.Length > 0)
        {
            GPUInstancerAPI.InitializeWithMatrix4x4Array(prefabManager, prefab.prefabPrototype, useMatrix);
            GPUInstancerAPI.SetInstanceCount(prefabManager, prefab.prefabPrototype, matrix.Length);
            GPUInstancerAPI.UpdateVisibilityBufferWithNativeArray(prefabManager, prefab.prefabPrototype, matrix,
                  0, 0, matrix.Length);
        }
        else
        {
            //GPUInstancerAPI.SetInstanceCount(prefabManager, prefabPrototype, 0);
            //GPUInstancerAPI.InitializePrototype(prefabManager, prefabPrototype, 0, 0);
           // return;
        }
    }
    public Matrix4x4[] ConvertToMatrix4x4Array(NativeArray<float4x4> float4x4Array)
    {
        Matrix4x4[] matrix4x4Array = new Matrix4x4[float4x4Array.Length];

        for (int i = 0; i < float4x4Array.Length; i++)
        {
            float4x4 f4x4 = float4x4Array[i];
            Matrix4x4 m4x4 = new Matrix4x4();

            m4x4.SetRow(0, new Vector4(f4x4.c0.x, f4x4.c0.y, f4x4.c0.z, f4x4.c0.w));
            m4x4.SetRow(1, new Vector4(f4x4.c1.x, f4x4.c1.y, f4x4.c1.z, f4x4.c1.w));
            m4x4.SetRow(2, new Vector4(f4x4.c2.x, f4x4.c2.y, f4x4.c2.z, f4x4.c2.w));
            m4x4.SetRow(3, new Vector4(f4x4.c3.x, f4x4.c3.y, f4x4.c3.z, f4x4.c3.w));

            matrix4x4Array[i] = m4x4;
        }

        return matrix4x4Array;
    }
}
