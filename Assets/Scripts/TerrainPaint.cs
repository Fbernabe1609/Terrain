using UnityEngine;

public class TerrainPaint : MonoBehaviour
{
    [System.Serializable]
    public class HeightTexture
    {
        public int textureIndex;
        public int initHeight;
    }

    public HeightTexture[] heightTexture;
    
    public void TerrainPainting()
    {
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        float[,,] mapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
        float[] paint = new float[heightTexture.Length];

        for (int x = 0; x < terrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
                float localHeight = terrainData.GetHeight(y, x);

                for (int i = 0; i < heightTexture.Length; i++)
                {
                    paint[i] = 0;
                    if (i < heightTexture.Length - 1 && localHeight >= heightTexture[i].initHeight && localHeight < heightTexture[i+1].initHeight)
                    {
                        paint[i] = 1;
                    }
                    else if (localHeight >= heightTexture[heightTexture.Length-1].initHeight)
                    {
                        paint[heightTexture.Length-1] = 1;
                    }
                    mapData[x, y, i] = paint[i];
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, mapData);
    }
}
