using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class SunOrbit : MonoBehaviour
{
    public Transform sun = null;
    public GameObject sunLight = null;

    public bool isAboveHorizon = false;

    public float rotationSpeed = 5.0f;

    public SpriteRenderer spriteRenderer = null;
    public Material playerSpriteMaterial = null;

    public Material treeSpriteMaterial = null;

    public Color dayTimeColour = new Color();
    public Color nightTimeColour = new Color();

    public Color playerDayTimeColour = new Color();
    public Color playerNightTimeColour = new Color();

    public Material subChunkMaterial = null;

    public float dayNightColourDifference = 0.15f;

    public Vector4 dayTimeVector = new Vector4(0.2f, 0.2f, 0.2f, 0);
    public Vector4 nightTimeVector = new Vector4(0.1f, 0.1f, 0.1f, 0);

    [BurstCompile]
    private void Start()
    {
        playerSpriteMaterial = spriteRenderer.material;
        playerSpriteMaterial.EnableKeyword("_BASECOLOR");
        playerSpriteMaterial.EnableKeyword("_NORMALMAP");
    }

    // Update is called once per frame
    [BurstCompile]
    void Update()
    {
        sun.Rotate(Vector3.right * (rotationSpeed * Time.deltaTime));

        // note transform component display 180 to -180 for "rotation" but using eulerAngles yields results of 0-360
        if (sun.eulerAngles.x < 180) {

            if (isAboveHorizon == false)
            {
                sunLight.SetActive(true);
                playerSpriteMaterial.SetColor("_BaseColor", playerDayTimeColour);
                treeSpriteMaterial.SetColor("_BaseColor", dayTimeColour);
                //spriteRenderer.color = dayTimeColour;
                subChunkMaterial.SetVector("Vector4_59394f3fb3584d17b826a949e58241e0", dayTimeVector);
            }
            isAboveHorizon = true;
        }
        else {

            if (isAboveHorizon == true)
            {
                sunLight.SetActive(false);
                playerSpriteMaterial.SetColor("_BaseColor", playerNightTimeColour);
                treeSpriteMaterial.SetColor("_BaseColor", nightTimeColour);
                //spriteRenderer.color = nightTimeColour;
                subChunkMaterial.SetVector("Vector4_59394f3fb3584d17b826a949e58241e0", nightTimeVector);
            }
            isAboveHorizon = false;
        }
    }
}
