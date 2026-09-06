using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyLOS))]
public class LOSEditor : Editor
{
    // Visualise the line of sight radius
    private void OnSceneGUI()
    {
        EnemyLOS Enemy = (EnemyLOS)target;

        // Draw the circle fov radius
        Handles.DrawWireArc(Enemy.transform.position, Vector3.up, Vector3.forward, 360, Enemy.Enemies.Enemy.LOSRadius);

        // left side of the fov angle
        Vector3 AngleA = Enemy.DirectionFromAngle(-Enemy.Enemies.Enemy.LOSAngle / 2 );

        // right side of the fov angle
        Vector3 AngleB = Enemy.DirectionFromAngle(Enemy.Enemies.Enemy.LOSAngle / 2);

        // Draw both lines to create the view angle
        Handles.DrawLine(Enemy.transform.position, Enemy.transform.position + AngleA * Enemy.Enemies.Enemy.LOSRadius);
        Handles.DrawLine(Enemy.transform.position, Enemy.transform.position + AngleB * Enemy.Enemies.Enemy.LOSRadius);
    }
}
