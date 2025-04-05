using UnityEngine;

public class TerrainTextureGenerator : MonoBehaviour
{
    public Terrain terrain; // Reference to the Terrain object
    public TerrainLayer grassLayer; // Terrain layer for grass texture
    public TerrainLayer stoneLayer; // Terrain layer for stones texture
    public TerrainLayer soilLayer; // Terrain layer for soil texture
    public TerrainLayer roadLayer; // Terrain layer for road texture (for lower areas)
    public TerrainLayer rockLayer; // Terrain layer for rock texture
    public float roadWidth = 0.1f; // Adjust this value to control road width

    void Update()
    {
        // Update the terrain textures continuously (every frame)
        GenerateTerrainTextures();
    }

    void GenerateTerrainTextures()
    {
        TerrainData terrainData = terrain.terrainData;

        // Get the number of layers from the terrain's terrain layers
        int textureCount = terrainData.terrainLayers.Length;

        // Create the splat map (3D array) that will hold texture weights
        float[,,] splatMap = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, textureCount];

        for (int x = 0; x < terrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
                // Get the normalized height of the terrain at this point
                float height = terrainData.GetHeight(x, y) / terrainData.size.y;

                // Road generation based on low terrain heights
                if (height < 0.3f)
                {
                    // If the area is low, apply road texture with some random variation
                    if (IsRoad(x, y, terrainData))
                    {
                        if (textureCount > 3) splatMap[x, y, 3] = 1; // Apply road texture (index 3 for road)
                    }
                    else
                    {
                        if (textureCount > 0) splatMap[x, y, 0] = 0.6f; // Apply grass texture (index 0 for grass)
                        if (textureCount > 1) splatMap[x, y, 1] = 0.4f; // Apply stone texture (index 1 for stone)
                    }
                }
                // Medium terrain heights (hills)
                else if (height < 0.6f)
                {
                    if (textureCount > 2) splatMap[x, y, 2] = 0.6f; // Apply soil texture (index 2 for soil)
                    if (textureCount > 1) splatMap[x, y, 1] = 0.4f; // Add a bit of rock (index 1 for stone)
                }
                // High terrain heights (mountains)
                else
                {
                    if (textureCount > 1) splatMap[x, y, 1] = 0.7f; // Apply rock texture (index 1 for rock)
                    if (textureCount > 0) splatMap[x, y, 0] = 0.3f; // Apply grass texture (index 0 for grass)
                }
            }
        }

        // Apply the splat map to the terrain
        terrainData.SetAlphamaps(0, 0, splatMap);
    }

    // Function to check if the area should have a road (based on coordinates and randomization)
    bool IsRoad(int x, int y, TerrainData terrainData)
    {
        float xCoord = (float)x / terrainData.alphamapWidth * terrainData.size.x;
        float yCoord = (float)y / terrainData.alphamapHeight * terrainData.size.z;

        // Road is simply a path that runs in a straight line, you can adjust the logic
        // For example, creating a simple road in the middle of the terrain
        return Mathf.Abs(xCoord - terrainData.size.x * 0.5f) < roadWidth;
    }
}
