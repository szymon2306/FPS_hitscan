using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{

    public GameObject player;
    public Transform respawnPoint;

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

        // draw a gizmo at the respawn point for debugging
        Gizmos.DrawSphere(respawnPoint.position, 0.5f);
        Gizmos.color = Color.red;
    }

    private void Respawn()
    {
        player.transform.position = respawnPoint.position;
        player.transform.rotation = respawnPoint.rotation;
    }

}
