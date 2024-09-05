using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : BaseStats
{
    [Header("Enemy Stats")]
    public float patrolThreshold;

    public float chaseRange;
    public float attackRange;

    public float idleDuration;

    public float patrolMovementSpeed;
    public float chaseMovementSpeed;
}