using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{

    public GameObject player;
    public Transform respawnPoint;
    public Vector3 spawnPoint;
    private Vector3 spawnDirection = Vector3.forward;
    private bool hasSpawnPoint = false;
    public bool showSpawnGizmo = true;

    public KeyCode respawnKey = KeyCode.R;

    // script to place a spawnter point in the game world by prssing the key t
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CreateSpawnPoint();            
        }

        if (Input.GetKeyDown(respawnKey))
        {
            Respawn();
        }
    }

    private void CreateSpawnPoint()
    {
        // spawn a respawn point at the player's position
        respawnPoint.position = player.transform.position;
        spawnPoint = player.transform.position;
        spawnDirection = transform.forward;
        hasSpawnPoint = true;
    }

    private void Respawn()
    {
        player.transform.position = respawnPoint.position;
        player.transform.rotation = respawnPoint.rotation;
    }

        private void OnDrawGizmos()
    {
        if (!showSpawnGizmo || !hasSpawnPoint) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(spawnPoint, 0.3f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(spawnPoint, spawnPoint + spawnDirection * 2f);
    }

}
