using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIUnit : MonoBehaviour
{
    // The AI Unit script feeds instructions to the main unit script based on its intentions.

    // The unit being controlled.
    private BattleUnit unit;

    // The seeker is used for pathfinding.
    private Seeker seeker;

    // The target that the AI is attempting to follow.
    private BattleUnit target;

    private Path path;
    private int currentWaypoint = 0;

    // The sight of the character.
    private FieldOfView FoV;

    [SerializeField]
    private float attackRange;

    private void Start()
    {
        unit = GetComponentInParent<BattleUnit>();
        seeker = GetComponent<Seeker>();
        FoV = GetComponent<FieldOfView>();
        InvokeRepeating("UpdatePath", 0, 1);
    }

    private void Update()
    {
        target = FindTarget();
        if (path != null) { PathFollow(); }
        if (TargetInRange()) { unit.Attack01(); }
    }

    private BattleUnit FindTarget()
    {
        List<GameObject> objectsInView = FoV.GetObjectsInView();
        foreach(GameObject go in objectsInView)
        {
            BattleUnit foundTarget = go.GetComponent<BattleUnit>();
            if (foundTarget)
            {
                return foundTarget;
            }
        }
        return null;
    }

    private bool TargetInRange()
    {
        if (target == null) { return false; }
        if (Vector2.Distance(transform.position, target.transform.position) < attackRange) { return true; }
        else { return false; }
    }

    private void UpdatePath()
    {
        if (target == null) { return; }
        if (seeker.IsDone())
        {
            seeker.StartPath(gameObject.transform.position, UnderObject(target.transform.position), OnPathComplete);
        }
    }

    private Vector3 UnderObject(Vector3 ob)
    {
        RaycastHit2D hit = Physics2D.Raycast(ob, Vector2.down);
        if (hit) { return hit.point; }
        return ob;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void PathFollow()
    {
        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count - 1)
        {
            return;
        }

        // Which direction is the next waypoint?
        Vector3 p1 = path.vectorPath[currentWaypoint];
        Vector3 p2 = path.vectorPath[currentWaypoint + 1];
        Vector2 direction = new Vector2(p2.x - p1.x, p2.y - p1.y);
        direction.x = Mathf.Clamp(direction.x, -1, 1);

        if (direction.y > 0)
        {
            unit.Jump(direction.x);
        }

        unit.ControlCharacter(direction);
        currentWaypoint++;
    }

}
