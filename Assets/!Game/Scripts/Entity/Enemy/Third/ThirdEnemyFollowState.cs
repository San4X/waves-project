using UnityEngine;
using UnityEngine.AI;

public class ThirdEnemyFollowState : EnemyFollowState
{
    public ThirdEnemyFollowState(Transform target, NavMeshAgent navAgent, float attackDistance, float farAwayDistance, float followSpeed, float maxSpeed) : base(target, navAgent, attackDistance, farAwayDistance, followSpeed, maxSpeed)
    {
    }
}
