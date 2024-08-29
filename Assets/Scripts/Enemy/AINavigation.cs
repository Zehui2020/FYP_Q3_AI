using Pathfinding;
using UnityEngine;

public class AINavigation : MonoBehaviour
{
    private Transform target;
    private AIDestinationSetter destinationSetter;
    private AIPath aiPath;

    public void InitPathfindingAgent()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
    }

    public void SetPathfindingTarget(Transform target, float speed, bool ignoreY)
    {
        this.target = target;

        if (!aiPath.enabled)
            return;

        aiPath.maxSpeed = speed;

        if (ignoreY)
            destinationSetter.target = new Vector3(target.position.x, transform.position.y, transform.position.z);
        else
            destinationSetter.target = target.position;
    }

    public void StopNavigation()
    {
        if (!aiPath.enabled)
            return;

        aiPath.canMove = false;
    }

    public void ResumeNavigation()
    {
        if (!aiPath.enabled)
            return;

        aiPath.canMove = true;
    }

    public void SetPathfinding(bool active)
    {
        aiPath.enabled = active;
    }

    public Vector2 GetMovementDirection()
    {
        if (target == null)
            return Vector2.zero;

        if (target.position.x > transform.position.x)
            return Vector2.right;
        else
            return Vector2.left;
    }
}