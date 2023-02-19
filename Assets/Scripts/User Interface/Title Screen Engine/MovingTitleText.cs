using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class MovingTitleText : MonoBehaviour
{
    public Vector2 offset = new Vector2(0.68f, 0.26f);
    public float RESET_POSITION = 0.68f;

    public GameObject titleText = null;
    public Material titleScreenTextMaterial = null;
    public TMP_Text titleTmpText = null;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory
        titleText = null;

        titleScreenTextMaterial = null;
        titleTmpText = null;
    }

    [BurstCompile]
    private void Awake()
    {
        titleTmpText = titleText.GetComponent<TMP_Text>();
        titleScreenTextMaterial = titleTmpText.fontMaterial;        
    }

    // Update is called once per frame
    [BurstCompile]
    public void Update()
    {
        if (offset.x > 0)
        {
            offset.x -= Time.deltaTime / 50;
        }
        else
        {
            offset.x += RESET_POSITION - Time.deltaTime / 50;
        }

        // increment texture off set

        titleScreenTextMaterial.SetTextureOffset("_FaceTex", offset);

        //titleTmpText.fontMaterial = titleScreenTextMaterial;
    }
}
