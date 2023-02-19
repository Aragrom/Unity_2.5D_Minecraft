using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEditor;
using UnityEngine;

[BurstCompile]
public class BlockMipMap : MonoBehaviour
{
    public bool generateNewTexture = false;

    public Texture2D blockMap1024 = null;
    public Texture2D blockMap512 = null;
    public Texture2D blockMap256 = null;
    public Texture2D blockMap128 = null;
    public Texture2D blockMap64 = null;

#if UNITY_EDITOR

    [BurstCompile]
    private void Start()
    {
        if (!generateNewTexture) return;

        Texture2D limitedMips = new Texture2D(1024, 1024, TextureFormat.ARGB32, 5, false);

        // ... fill it ...
        limitedMips.SetPixels(blockMap1024.GetPixels(), 0);
        limitedMips.SetPixels(blockMap512.GetPixels(), 1);
        limitedMips.SetPixels(blockMap256.GetPixels(), 2);
        limitedMips.SetPixels(blockMap128.GetPixels(), 3);
        limitedMips.SetPixels(blockMap64.GetPixels(), 4);

        limitedMips.alphaIsTransparency = true;
        limitedMips.wrapMode = TextureWrapMode.Clamp;
        limitedMips.filterMode = FilterMode.Trilinear;

        // apply changes
        limitedMips.Apply(true);

        AssetDatabase.CreateAsset(limitedMips, "Assets/Textures/LimitedMips.asset");
        AssetDatabase.SaveAssets();
    }

#endif

}
