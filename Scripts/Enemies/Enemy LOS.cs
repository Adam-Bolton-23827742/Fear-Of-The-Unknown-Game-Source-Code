using UnityEngine;

public class EnemyLOS : MonoBehaviour
{
    public Enemies Enemies;
    [SerializeField] private LayerMask PlayerLayer, ObstacleLayers;
    [HideInInspector] public Transform Player;
    private Vector3 DirectionToPlayer;

    public Vector3 DirectionFromAngle(float AngleInDegrees)
    {
        // Sets the angle direction, converting it to radians from degrees
        return new Vector3(Mathf.Sin(AngleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(AngleInDegrees * Mathf.Deg2Rad));
    }

    // Update is called once per frame
    void Update()
    {
        // Overlap sphere that stores the player's collider in an array when the player is within the view radius
        Collider[] LOSCollider = Physics.OverlapSphere(transform.position, Enemies.Enemy.LOSRadius, PlayerLayer);

        for (int i = 0; i < LOSCollider.Length; i++)
        {
            // Store the player's transform
            Player = LOSCollider[i].transform;

            // Store the direction from the enemy to the player
            DirectionToPlayer = Player.position - transform.position;

            // If the player is inside the fov angle
            if (Vector3.Angle(transform.forward, DirectionToPlayer) < Enemies.Enemy.LOSAngle / 2)
            {
                // If there is no obstacle between the enemy and the player
                if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), DirectionToPlayer, out RaycastHit Hit, Mathf.Infinity, ObstacleLayers))
                {
                    if (LayerMask.LayerToName(Hit.transform.gameObject.layer) == "Player")
                    {
                        // The enemy can see the player
                        Enemies.CanSeePlayer = true;

                        // If the player is not within the enemy's attack radius, set the enemy ststae to the move state to get closer
                        if (Vector3.Distance(transform.position, Player.position) > Enemies.Enemy.AttackDistance && Enemies.CurrentState != 4)
                        {
                            Enemies.CurrentState = 1;
                        }
                    }
                }
            }

            // The enemy cannot see the player if they are not inside the fov angle
            else
            {
                Enemies.CanSeePlayer = false;
            }
        }
    }

    // Draw the raycast connecting the enemy and the player when the player is visible to the enemy
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (Enemies != null && Enemies.CanSeePlayer)
        {
            Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), DirectionToPlayer);
        }
    }
}
