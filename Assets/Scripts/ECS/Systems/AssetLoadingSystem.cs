using Game.ECS.Base.Systems;
using UnityEngine;

public class AssetLoadingSystem : IInitSystem
{
    public void Init(SystemManager systemManager)
    {
        LoadMeshes(systemManager);
        LoadMaterials(systemManager);
    }

    private void LoadMeshes(SystemManager systemManager)
    {
        Mesh[] meshes = Resources.LoadAll<Mesh>(AssetPaths.MeshAssetPath);
        SortArrayByName<Mesh>(ref meshes);

        systemManager.AddSharedData(new MeshContainer()
        {
            Meshes = meshes
        });
    }

    private void LoadMaterials(SystemManager systemManager)
    {
        Material[] materials = Resources.LoadAll<Material>(AssetPaths.MaterialAssetPath);
        SortArrayByName<Material>(ref materials);

        systemManager.AddSharedData(new MaterialContainer()
        {
            Materials = materials
        });
    }

    public void SortArrayByName<T>(ref T[] items) where T : Object
    {
        if (items == null || items.Length == 0)
        {
            Debug.LogWarning($"{typeof(T).Name} array is null or empty.");
            return;
        }

        for (int i = 0; i < items.Length - 1; i++)
        {
            for (int j = 0; j < items.Length - i - 1; j++)
            {
                if (string.Compare(items[j].name, items[j + 1].name) > 0)
                {
                    T temp = items[j];
                    items[j] = items[j + 1];
                    items[j + 1] = temp;
                }
            }
        }


    }
}

public static class AssetPaths
{
    public static string MeshAssetPath = "Meshes";
    public static string MaterialAssetPath = "Materials";
}
public struct MeshContainer : ISharedData
{
    public Mesh[] Meshes;
}
public struct MaterialContainer : ISharedData
{
    public Material[] Materials;
}