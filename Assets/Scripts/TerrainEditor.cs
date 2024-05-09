using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class TerrainEditor : MonoBehaviour
{
    [SerializeField] private TerrainData terrainData;
    [SerializeField] private Terrain terrain;
    [SerializeField] private int seed = 0;
    [SerializeField] private int detail = 150;
    [SerializeField] private float heightCorrection = -1;
    [SerializeField] private TerrainPaint paint;
    [SerializeField] private int NumberTrees=5;
    [SerializeField] private float minX=0f;
    [SerializeField] private float maxX=1f;
    [SerializeField] private float minZ=0f;
    [SerializeField] private float maxZ=1f;
    [SerializeField] private int waterHeight=160;
    [SerializeField] private GameObject water;

    private float[,] matrix;

    public TMPro.TMP_InputField detailInput;
    public TMPro.TMP_InputField waterHeightInput;
    public TMPro.TMP_InputField seedInput;
    public TMPro.TMP_InputField heightCorrectionInput;

    public Animator m_Animator;



    // Start is called before the first frame update
    void Start()
    {

        matrix = new float[513, 513];
        
        ShowPanel();
        ActualizarTerreno();

    }
    public void ShowPanel()
    {
        detailInput.text = detail.ToString();
        seedInput.text = seed.ToString();
        waterHeightInput.text = waterHeight.ToString();
        heightCorrectionInput.text = heightCorrection.ToString();
    }
    public void ActualizarTerreno()
    {
        try
        {
            detail = Int32.Parse(detailInput.text);
        }
        catch (FormatException e)
        {
        }
        try
        {
            seed = Int32.Parse(seedInput.text);
        }
        catch (FormatException e)
        {
        }
        try
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("fr-FR");

            heightCorrection = (float)Double.Parse(heightCorrectionInput.text,culture);
        }
        catch (FormatException e)
        {
        }
        try
        {
            waterHeight = Int32.Parse(waterHeightInput.text);
        }
        catch (FormatException e)
        {
        }
        m_Animator.SetTrigger("Restart");

        HeightGenerator();

        //Se pone la altura que será el número aleatorio creado
        terrainData.SetHeights(0, 0, matrix);

        paint.TerrainPainting();

       // PaintTrees();
    }

    private void PaintTrees()
    {
        for (int i = 0; i < NumberTrees; i++)
        {
            //Instanciar árbol
            TreeInstance tree= new TreeInstance();
            tree.prototypeIndex =0;

            //tree = terrainData.treeInstances[i]=tree;

            //Colocarlo aleatoriamente dentro de los márgenes
            Vector3 position = new Vector3(UnityEngine.Random.Range(minX, maxX), 0f, UnityEngine.Random.Range(minZ,maxZ));
            tree.position = position;

            /*tree.position = new Vector3(1/ 100, 0, 1 / 100);
            tree.prototypeIndex = 0;
            tree.widthScale = 1f;
            tree.heightScale = 1f;
            tree.color = Color.white;
            tree.lightmapColor = Color.white;*/

            //Añadirlo al terreni
            terrain.AddTreeInstance(tree);
        }
    }

    private void HeightGenerator()
    {
        water.transform.position= new Vector3(water.transform.position.x,waterHeight,water.transform.position.z);
        for(int i=0; i<matrix.GetLength(0); i++)
        {
            for( int j=0; j<matrix.GetLength(1); j++)
            {
                //Cada parte se le da un número aleatorio
                // matrix[i, j] = UnityEngine.Random.Range(0.0f, 0.005f);

                matrix[i, j] = Mathf.PerlinNoise((float)(i+seed)/detail, (float)(j+seed)/detail)+heightCorrection;
            }
        }        
    }

}
