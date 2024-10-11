using UnityEngine;
using System.Collections.Generic;

public class PathFollower : MonoBehaviour
{
    public List<Transform> waypoints;
    public float speed = 5.0f;
    private int currentIndex = 0;

    void Update()
    {
        if (currentIndex < waypoints.Count)
        {
            Transform targetWaypoint = waypoints[currentIndex];

            if (Vector3.Distance(transform.position, targetWaypoint.position) < 3f)
            {
                currentIndex++;
            }

            transform.LookAt(targetWaypoint.position);
        }
    }
}
