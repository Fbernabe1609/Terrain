using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TerrainEditor : MonoBehaviour
{
    [SerializeField] private TerrainData terrainData;
    [SerializeField] private Terrain terrain;
    [SerializeField] private int seed = 0;
    [SerializeField] private int detail = 150;
    [SerializeField] private float heightCorrection = -1;
    [SerializeField] private TerrainPaint paint;
    [SerializeField] private int numberOfTreesToPaint = 5;
    [SerializeField] private float minX = 0f;
    [SerializeField] private float maxX = 1f;
    [SerializeField] private float minZ = 0f;
    [SerializeField] private float maxZ = 1f;
    [SerializeField] private int waterHeight = 160;
    [SerializeField] private GameObject water;

    private float[,] matrix;

    public TMPro.TMP_InputField detailInput, waterHeightInput, seedInput, heightCorrectionInput;

    public Animator m_Animator;

    void Start()
    {
        matrix = new float[513, 513];
        ShowPanel();
        UpdateTerrain();
    }

    public void ShowPanel()
    {
        detailInput.text = detail.ToString();
        seedInput.text = seed.ToString();
        waterHeightInput.text = waterHeight.ToString();
        heightCorrectionInput.text = heightCorrection.ToString(CultureInfo.InvariantCulture);
    }

    public void UpdateTerrain()
    {
        try
        {
            detail = Int32.Parse(detailInput.text);
            seed = Int32.Parse(seedInput.text);
            CultureInfo culture = CultureInfo.CreateSpecificCulture("fr-FR");
            heightCorrection = (float)Double.Parse(heightCorrectionInput.text, culture);
            waterHeight = Int32.Parse(waterHeightInput.text);
        }
        catch (FormatException e)
        {
            Debug.Log("Error: " + e.Message);
        }

        m_Animator.SetTrigger("Restart");

        HeightGenerator();

        terrainData.SetHeights(0, 0, matrix);

        paint.TerrainPainting();

        PaintTrees();
    }

    // private void PaintTrees()
    // {
    //     for (int i = 0; i < NumberTrees; i++)
    //     {
    //         TreeInstance tree = new TreeInstance();
    //         tree.prototypeIndex = 0;
    //
    //         //tree = terrainData.treeInstances[i]=tree;
    //
    //         Vector3 position = new Vector3(UnityEngine.Random.Range(minX, maxX), 0f,
    //             UnityEngine.Random.Range(minZ, maxZ));
    //         tree.position = position;
    //
    //         tree.position = new Vector3(1/ 100, 0, 1 / 100);
    //         tree.prototypeIndex = 0;
    //         tree.widthScale = 1f;
    //         tree.heightScale = 1f;
    //         tree.color = Color.white;
    //         tree.lightmapColor = Color.white;
    //
    //         terrain.AddTreeInstance(tree);
    //     }
    // }
    
    private void PaintTrees()
    {
        for (int i = 0; i < numberOfTreesToPaint; i++)
        {
            TreeInstance tree = new TreeInstance();
            tree = terrainData.treeInstances[i];
            Vector3 position = new Vector3(Random.Range(minX,maxX), 0, Random.Range(minZ,maxZ));
            tree.position = position;
            terrain.AddTreeInstance(tree);
        }
    }
    
    private void HeightGenerator()
    {
        water.transform.position = new Vector3(water.transform.position.x, waterHeight, water.transform.position.z);

        float seedDetailRatio = (float)seed / detail;
        int matrixLength0 = matrix.GetLength(0);
        int matrixLength1 = matrix.GetLength(1);

        Parallel.For(0, matrixLength0, i =>
        {
            float iDetailRatio = (float)i / detail;
            for (int j = 0; j < matrixLength1; j++)
            {
                matrix[i, j] = Mathf.PerlinNoise(iDetailRatio + seedDetailRatio, (float)j / detail + seedDetailRatio) +
                               heightCorrection;
            }
        });
    }
}