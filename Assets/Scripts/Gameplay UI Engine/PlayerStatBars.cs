using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class PlayerStatBars : MonoBehaviour
{
    public PlayerStats playerStats = null;

    public RectTransform healthTransform = null;
    public RectTransform manaTransform = null;
    public RectTransform staminaTransform = null;

    public int MAX_HEALTH = 1000;
    public int MAX_MANA = 1000;
    public int MAX_STAMINA = 1000;

    public float fullScreenBarWidth = 0;

    public float healthMaxLength = 0;
    public float manaMaxLength = 0;
    public float staminaMaxLength = 0;

    public float healthLength = 0;
    public float manaLength = 0;
    public float staminaLength = 0;

    public float healthPercent = 0;
    public float manaPercent = 0;
    public float staminaPercent = 0;

    [BurstCompile]
    public void Start()
    {
        fullScreenBarWidth = Screen.width - 10; // minus 10 to not go to the edge of screen

        healthMaxLength = fullScreenBarWidth * ((float)playerStats.MAX_HEALTH / (float)MAX_HEALTH);

        //                                      . Minus sign used due to component design
        healthTransform.offsetMax = new Vector2(-(fullScreenBarWidth - healthMaxLength) - 10, healthTransform.offsetMax.y);
    }
}
