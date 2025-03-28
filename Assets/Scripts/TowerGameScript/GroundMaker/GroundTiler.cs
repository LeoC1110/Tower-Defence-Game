using UnityEngine;

public class GroundTiler : MonoBehaviour
{
    public GameObject forestGroundPrefab; 
    public Transform ground; 
    public float tileSize = 1f;

    [ContextMenu("Tile Ground")]
    public void TileGround()
    {

        // Get the scale of the ground
        Vector3 groundScale = ground.localScale;

        // calculate the actual size of the ground (Plane default unit size is 10x10)
        float width = groundScale.x * 10f;  
        float depth = groundScale.z * 10f;

        // Calculate how many rows and columns are needed
        int rows = Mathf.FloorToInt(width / tileSize);  
        int cols = Mathf.FloorToInt(depth / tileSize);

        // calculate the start position of the ground
        Vector3 groundPosition = ground.position;
        Vector3 startPos = groundPosition - new Vector3(width / 2f, 0, depth / 2f);

        // Tile the ground with ForestGround
        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < cols; z++)
            {
                Vector3 spawnPos = startPos + new Vector3(x * tileSize + tileSize / 2f, 0.01f, z * tileSize + tileSize / 2f);
                Instantiate(forestGroundPrefab, spawnPos, Quaternion.identity, ground);
            }
        }        
    }
}