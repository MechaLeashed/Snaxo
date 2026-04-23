using UnityEngine;

public class PatrolPath : MonoBehaviour {
    public Transform[] waypoints;
    private int currentIndex = 0;

    public Vector3 GetCurrentWaypoint() {
        if (waypoints.Length == 0) {
            Debug.LogWarning("No waypoints set on " + gameObject.name);
            return transform.position;
        }
        return waypoints[currentIndex].position;
    }

    public Vector3 GetNextWaypoint() {
        currentIndex = (currentIndex + 1) % waypoints.Length;
        return waypoints[currentIndex].position;
    }

    public bool ReachedWaypoint(Vector3 agentPosition, float threshold = 0.5f) {
        return Vector3.Distance(agentPosition, GetCurrentWaypoint()) <= threshold;
    }

    // Called by Coordinator when enemy is reused from pool
    public void ResetPath() {
        currentIndex = 0;
    }

    // Draws waypoint path in Scene view
    private void OnDrawGizmos() {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++) {
            if (waypoints[i] == null)
                continue;

            Gizmos.DrawSphere(waypoints[i].position, 0.2f);

            if (i + 1 < waypoints.Length && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        // Close the loop
        if (waypoints.Length > 1 && waypoints[waypoints.Length - 1] != null)
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position,
                            waypoints[0].position);
    }
}