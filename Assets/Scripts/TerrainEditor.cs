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
    [SerializeField] private int seed = 60;
    [SerializeField] private int detail = 100;
    [SerializeField] private float heightCorrection = -0.05f;
    [SerializeField] private TerrainPaint paint;
    [SerializeField] private float minX = 0f;
    [SerializeField] private float maxX = 700f;
    [SerializeField] private float minZ = 0f;
    [SerializeField] private float maxZ = 700f;
    [SerializeField] private int waterHeight = 160;
    [SerializeField] private GameObject water;

    private float[,] matrix;

    public TMPro.TMP_InputField detailInput, waterHeightInput, seedInput, heightCorrectionInput;

    public Animator animator;
    private static readonly int Restart = Animator.StringToHash("Restart");

    void Start()
    {
        matrix = new float[513, 513];
        LoadTerrainEditorPanel();
        UpdateTerrain();
    }

    public void LoadTerrainEditorPanel()
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
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-En");
            heightCorrection = (float)Double.Parse(heightCorrectionInput.text, culture);
            waterHeight = Int32.Parse(waterHeightInput.text);
        }
        catch (FormatException e)
        {
            Debug.Log("Problem: " + e.Message);
        }

        HeightGenerator();

        terrainData.SetHeights(0, 0, matrix);

        paint.TerrainPainting();

        PaintTrees();
        
        animator.SetTrigger(Restart);
    }

    private void PaintTrees()
    {
        for (int i = 0; i < terrainData.treePrototypes.Length; i++)
        {
            TreeInstance tree = new TreeInstance();
            tree.prototypeIndex = i;

            Vector3 position = new Vector3(Random.Range(minX, maxX), 0f, Random.Range(minZ, maxZ));

            tree.position = position;
            tree.widthScale = 5f;
            tree.heightScale = 5f;

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