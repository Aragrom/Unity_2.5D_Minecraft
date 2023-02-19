using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class PlayerStats : MonoBehaviour
{
    public int MAX_HEALTH;
    public int health;

    public int MAX_MANA;
    public int mana;

    public int MAX_STAMINA;
    public int stamina;
}
