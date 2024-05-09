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

        for (int x = 0; x < terrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
                float localHeight = terrainData.GetHeight(y, x);
                float[] paint = new float[heightTexture.Length];

                for (int i = 0; i < heightTexture.Length-1; i++)
                {
                    if (localHeight >= heightTexture[i].initHeight && localHeight < heightTexture[i+1].initHeight)
                    {
                        paint[i] = 1;
                    }
                }
                if (localHeight >= heightTexture[heightTexture.Length-1].initHeight)
                {
                    paint[heightTexture.Length-1] = 1;
                }

                for (int i = 0; i < heightTexture.Length; i++)
                {
                    mapData[x, y, i] = paint[i];
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, mapData);
    }

}
