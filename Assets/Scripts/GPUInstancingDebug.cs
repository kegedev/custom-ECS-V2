using UnityEngine;

public class GPUInstancingDebug : MonoBehaviour
{
    [Header("Settings")]
    public Mesh mesh;                  // Mesh to be instanced
    public Material material;          // Material to be used for instancing
    public int instanceCount = 1000;   // Number of instances to render
    public float spacing = 2.0f;       // Spacing between instances

    private Matrix4x4[] instanceMatrices;

    void Start()
    {
        // Ensure the material supports instancing
        if (!material.enableInstancing)
        {
            Debug.LogWarning("Material does not support GPU instancing. Enabling instancing.");
            material.enableInstancing = true;
        }

        // Initialize instance matrices
        instanceMatrices = new Matrix4x4[instanceCount];
        for (int i = 0; i < instanceCount; i++)
        {
            // Distribute instances in a grid for visualization
            float x = (i % 10) * spacing;
            float z = (i / 10) * spacing;
            Vector3 position = new Vector3(x, 0, z);
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Vector3.one;

            instanceMatrices[i] = Matrix4x4.TRS(position, rotation, scale);
        }
    }

    void Update()
    {
        // Render instances in batches of 1023 (Unity's Graphics.DrawMeshInstanced limit)
        const int batchSize = 1023;
        for (int i = 0; i < instanceCount; i += batchSize)
        {
            int count = Mathf.Min(batchSize, instanceCount - i);
            Graphics.DrawMeshInstanced(mesh, 0, material, instanceMatrices, count, null,
                UnityEngine.Rendering.ShadowCastingMode.On, true, 0, null);
        }
    }

    void OnDrawGizmos()
    {
        if (mesh == null || material == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawMesh(mesh, Vector3.zero);
    }
}
