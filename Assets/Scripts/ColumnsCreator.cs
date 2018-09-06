using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LZWPlib;

public class ColumnsCreator : MonoBehaviour {

    public GameObject columnPrefab;
    GameObject CustomSettings;
    int columnsCount;
    float columnsAreaX;
    float columnsAreaZ;
    float columnsAreaDistance;
    float columnsDistanceMin;
    float columnsDistance;
    Vector3 player1placeCenter;
    Vector3 player2placeCenter;
    float playerPlaceSize;
    bool columnPositionCorrect;
    float columnRadius;
    float columnHeight;
    List<GameObject> columnsList = new List<GameObject>();
    CustomSettings customSettings;
    GameController gameController;
    ColumnsBehaviour columnsBehaviour;
    bool init = false;
    bool notSet = true;
    //float dist = 5.0f;
    System.Random random = new System.Random();
    Dictionary<string, int> columnColor = new Dictionary<string, int>();
    int color1 = 0;
    int color2 = 0;
    Material material;
    public Material materialRed;
    public Material materialGreen;
    public Material materialYellow;
    GameObject column;
    public Texture columnTextureRed;
    public Texture columnTextureGreen;
    public float startPositionX = 0.0f;
    public float startPositionZ = 0.0f;
    //bool devMode = true;
    int yPosition = 0;
    bool moveTowards = false;
    //bool movedTowards = false;
    Vector3 positionAdded;
    Vector3 position;
    Vector3 caveOriginPosition;
    Vector3 cavePosition;
    Vector3 player1Position;
    Vector3 player2Position;
    public bool startDelay = true;
    float step;
    float d;
    bool movedTowards = true;

    Dictionary<string, Vector3> finalPositions = new Dictionary<string, Vector3>();


    // Use this for initialization
    void Start() {

        if (Core.Instance.isServer)
        {
            customSettings = GameObject.Find("CustomSettings").GetComponent<CustomSettings>();
            gameController = GameObject.Find("GameController").GetComponent<GameController>();
            columnsBehaviour = GameObject.Find("Columns").GetComponent<ColumnsBehaviour>();
        }

        /*
        finalPositions["column-4"] = new Vector3(4.7f, 2.95f, 12.81f + 0.92f);
        finalPositions["column-3"] = new Vector3(3.17f, 2.95f, 12.33f + 0.92f);
        finalPositions["column-2"] = new Vector3(1.54f, 2.95f, 11.75f + 0.92f);
        finalPositions["column-1"] = new Vector3(0f, 2.95f, 11.05f + 0.92f);
        finalPositions["column0"] = new Vector3(0f, 2.95f, 9.42f + 0.92f);
        finalPositions["column1"] = new Vector3(1.04f, 2.95f, 8.16f + 0.92f);
        finalPositions["column2"] = new Vector3(2.81f, 2.95f, 7.65f + 0.92f);
        finalPositions["column3"] = new Vector3(4.56f, 2.95f, 6.88f + 0.92f);
        finalPositions["column4"] = new Vector3(5.95f, 2.95f, 5.78f + 0.92f);
        finalPositions["column5"] = new Vector3(5.89f, 2.95f, 4.27f + 0.92f);
        finalPositions["column6"] = new Vector3(5.2f, 2.95f, 2.88f + 0.92f);
        finalPositions["column7"] = new Vector3(4.83f, 2.95f, 1.5f + 0.92f);
        finalPositions["column8"] = new Vector3(3.21f, 2.95f, 0.34f + 0.92f);
        */

        Vector3 finalPosition0 = new Vector3(0.0f, 2.95f, 17f);
        string name = "";
        float r = 3f;
        float alfa = 20;
        alfa = Mathf.Deg2Rad * alfa;
        Vector3 calcPosition = new Vector3(0f, 0f, 0f);

        finalPositions["column0"] = finalPosition0;

        finalPosition0.z += 3f;

        finalPositions["column-1"] = finalPosition0;


        for (int i=-2; i>-5; i--)
        {
            name = "column" + i;
            calcPosition.y = finalPosition0.y;
            calcPosition.x = finalPosition0.x + r * Mathf.Cos(alfa);
            calcPosition.z = finalPosition0.z + r * Mathf.Sin(alfa);
            finalPositions[name] = calcPosition;
            finalPosition0 = calcPosition;
        }

        finalPosition0 = new Vector3(0.0f, 2.95f, 17f);

        r = 1.5f;
        alfa = 50;
        alfa = Mathf.Deg2Rad * alfa;
        calcPosition.y = finalPosition0.y;
        calcPosition.x = finalPosition0.x + r * Mathf.Cos(alfa);
        calcPosition.z = finalPosition0.z - r * Mathf.Sin(alfa);
        finalPositions["column1"] = calcPosition;

        finalPosition0 = calcPosition;
        r = 3f;
        alfa = 20;
        alfa = Mathf.Deg2Rad * alfa;

        for (int i=2; i<5; i++)
        {
            name = "column" + i;
            calcPosition.y = finalPosition0.y;
            calcPosition.x = finalPosition0.x + r * Mathf.Cos(alfa);
            calcPosition.z = finalPosition0.z - r * Mathf.Sin(alfa);
            finalPositions[name] = calcPosition;

            finalPosition0 = calcPosition;
            alfa = Mathf.Rad2Deg * alfa;
            alfa += 8;
            alfa = Mathf.Deg2Rad * alfa;

        }

        name = "column5";
        finalPosition0.z -= 3f;
        finalPositions[name] = finalPosition0;

        alfa = 65;
        alfa = Mathf.Deg2Rad * alfa;

        for (int i=6; i<9; i++)
        {

            name = "column" + i;
            calcPosition.y = finalPosition0.y;
            calcPosition.x = finalPosition0.x - r * Mathf.Cos(alfa);
            calcPosition.z = finalPosition0.z - r * Mathf.Sin(alfa);
            finalPositions[name] = calcPosition;
            finalPosition0 = calcPosition;
            alfa = Mathf.Rad2Deg * alfa;
            alfa -= 8;
            alfa = Mathf.Deg2Rad * alfa;

        }


        foreach (string x in finalPositions.Keys)
        {
            Debug.Log(x + " " + finalPositions[x]);
        }




    }

    // Update is called once per frame
    void Update() {

        if (Core.Instance.isServer)
        {
            
            {
                if (customSettings.AllSet() && notSet)
                {
                    init = true;
                }
            }

            if (init && notSet)
            {
                if (movedTowards )

                {

                    if ((gameController.GetLevel() <= gameController.levelNum))
                    {
                        PrepareColumns();
                        notSet = false;
                    }
                    else
                    {
                        PrepareFinalLevel();
                        notSet = false;
                    }
                    
                }
            }
        }

        if (Core.Instance.isServer)
        {

            if (moveTowards)
            {
                step = 2 * Time.deltaTime;
                position = customSettings.caveOrigin.transform.position + positionAdded;
                customSettings.caveOrigin.transform.position = Vector3.MoveTowards(customSettings.caveOrigin.transform.position, position, step);
                position = customSettings.cave.transform.position + positionAdded;
                customSettings.cave.transform.position = Vector3.MoveTowards(customSettings.cave.transform.position, position, step);
                position = customSettings.player1Place.transform.position + positionAdded;
                customSettings.player1Place.transform.position = Vector3.MoveTowards(customSettings.player1Place.transform.position, position, step);
                position = customSettings.player2Place.transform.position + positionAdded;
                customSettings.player2Place.transform.position = Vector3.MoveTowards(customSettings.player2Place.transform.position, position, step);

                if (gameController.devMode)
                {
                    position = customSettings.camera.transform.position + positionAdded;
                    customSettings.camera.transform.position = Vector3.MoveTowards(customSettings.camera.transform.position, position, step);

                }

                d = Mathf.Sqrt(Mathf.Pow((customSettings.caveOrigin.transform.position.x - (caveOriginPosition.x + positionAdded.x)), 2)
                    + Mathf.Pow((customSettings.caveOrigin.transform.position.z - (caveOriginPosition.z + positionAdded.z)), 2));

                if (d <= 1.0f)
                {
                    movedTowards = true;
                    moveTowards = false;
                    columnsBehaviour.DestroyBrokenColumns();
                    startDelay = true;
                }


            }
        }
        

    }

    public Vector3 GetFinalPosition(string column)
    {

        return finalPositions[column];

    }

    public void PrepareFinalLevel()
    {

       
        Vector3 position = columnPrefab.transform.position;
        string colName;
      

        for (int i=-4; i < customSettings.finalColumnsCount + 1; i++)
        {

            colName = "column" + i;


            position = GetFinalPosition(colName);

            if (gameController.GetLevel() > 1)
                position += customSettings.addedPosition * (gameController.GetLevel() - 1);

            if (i < 1)
            {
                column = Network.Instantiate(columnPrefab, position, columnPrefab.transform.rotation, 0) as GameObject;
                column.gameObject.name = colName;
                columnsList.Add(column.gameObject);
                ColorColumn(column);
            }
            else
            {

                column = Network.Instantiate(columnPrefab, position, columnPrefab.transform.rotation, 0) as GameObject;
                column.gameObject.name = colName + "a";
                columnsList.Add(column.gameObject);
                ColorColumn(column);

                position.x *= -1;
                column = Network.Instantiate(columnPrefab, position, columnPrefab.transform.rotation, 0) as GameObject;
                column.gameObject.name = colName + "b";
                columnsList.Add(column.gameObject);
                ColorColumn(column);

            }
        }
    }


    public bool IsFinalColumnToBeTurned(GameObject column)
    {
        if (columnsList.IndexOf(column) < 5)
            return true;
        else
            return false;
    }

    public bool IsColumnToBeBroken(GameObject column)
    {
        if (columnsList.IndexOf(column) == 4)
            return true;
        else
            return false;
         
    }

    public void moveToNewLocation(float x, float z)
    {
        positionAdded = new Vector3(x, 0, z);
        caveOriginPosition = customSettings.caveOrigin.transform.position;
        cavePosition = customSettings.cave.transform.position;
        player1Position = customSettings.player1Place.transform.position;
        player2Position = customSettings.player2Place.transform.position;
        moveTowards = true;
    }

    public void PrepareColumns()
    {
        columnsCount = customSettings.columnsCount;
        columnsAreaX = customSettings.columnsAreaX;
        columnsAreaZ = customSettings.columnsAreaZ;
        columnsAreaDistance = customSettings.columnsAreaDistance;
        columnsDistanceMin = customSettings.columnsDistanceMin;
        columnsDistance = customSettings.columnsDistance;
        player1placeCenter = customSettings.player1Place.transform.position;
        player2placeCenter = customSettings.player2Place.transform.position;
        playerPlaceSize = customSettings.GetPlayerSize();
        Vector3 position = columnPrefab.transform.position;
        Mesh columnMesh = columnPrefab.GetComponent<MeshFilter>().sharedMesh;
        columnRadius = columnMesh.bounds.size.x * columnPrefab.transform.localScale.x / 2;
        columnHeight = columnMesh.bounds.size.y * columnPrefab.transform.localScale.y;

        position.z += columnsAreaDistance;

        if (gameController.GetLevel() > 1)
            position += customSettings.addedPosition * (gameController.GetLevel()-1);

        column = Network.Instantiate(columnPrefab, position, columnPrefab.transform.rotation, 0) as GameObject; //Network.Instantiate(columnPrefab, position, columnPrefab.rotation,0) as Transform;

        column.gameObject.name = "column0";
        //column.gameObject.SetActive(true);
        columnsList.Add(column.gameObject);
        ColorColumn(column);
        int check = 0;

        for (int i = 1; i < (int)columnsCount / 2; i++)
        {

            position = CalculatePosition(columnsList[i - 1], 1, i);         
            column = Network.Instantiate(columnPrefab, position, columnPrefab.transform.rotation, 0) as GameObject; //Network.Instantiate(columnPrefab, position, columnPrefab.rotation,0) as Transform;
            column.gameObject.name = "column" + i;
            //column.gameObject.SetActive(true);
            columnsList.Add(column.gameObject);

            ColorColumn(column.gameObject);

        }


        for (int i = (int)columnsCount / 2; i < columnsCount; i++)
        {
            if (i == (int)columnsCount / 2)

                position = CalculatePosition(columnsList[0], 0, i);
                
            else
                
                position = CalculatePosition(columnsList[i - 1], 0, i);
            column = Network.Instantiate(columnPrefab, position, columnPrefab.transform.rotation, 0) as GameObject; //Network.Instantiate(columnPrefab, position, columnPrefab.rotation,0) as Transform;
            column.gameObject.name = "column" + i;

            ColorColumn(column.gameObject);

            //column.gameObject.SetActive(true);
            columnsList.Add(column.gameObject);
        }
    }


    void ColorColumn(GameObject column)
    {
        int color = random.Next(1, 3);
        //int color = 1;

        if (color == 1)
        {
            if (color1 <= columnsCount / 2)
            {
                columnColor[column.name] = color;
                color1 += 1;
            }
            else
            {
                columnColor[column.name] = 2;
                color2 += 1;
            }
        }
        else
        {
            if (color2 <= columnsCount / 2)
            {
                columnColor[column.name] = color;
                color2 += 1;
            }
            else
            {
                columnColor[column.name] = 1;
                color1 += 1;
            }
        }

        //Renderer rend = column.GetComponent<Renderer>();

        column.GetComponent<ColorChange>().SetColor(columnColor[column.name]);

        /*
        if (columnColor[column.name] == 1)
            
            rend.material.mainTexture = columnTextureGreen;

        else
            
            rend.material.mainTexture = columnTextureRed;
            */
    }

    Vector3 CalculatePosition(GameObject prevColumn, int direction, int colIndex)
    {
        columnPositionCorrect = false;
        bool columnDistanceCorrect = false;
        bool columnVisible = false;
        bool columnsAngleCorrect = true;
        bool columnsNotInARow = true;

        Vector3 columnPosition = new Vector3(0, columnPrefab.transform.position.y, 0);

        int dup = 0;
        while (!columnPositionCorrect && dup++ < 5000)
        {

            columnPosition = GetRandomPosition(prevColumn, columnPosition, direction);

            
            {

                if (direction == 1)
                {
                    if (colIndex > 1)
                    {
                        columnsAngleCorrect = CheckColumnsAngle(columnsList[colIndex - 1], columnsList[colIndex - 2], columnPosition);
                        columnsNotInARow = CheckColumnsRow(columnsList[colIndex - 1], columnsList[colIndex - 2], columnPosition);
                    }
                    else
                    {
                        columnsNotInARow = true;
                        columnsAngleCorrect = true;
                    }

                }
                else
                {
                    if (colIndex == columnsCount / 2)
                    {
                        columnsAngleCorrect = CheckColumnsAngle(columnsList[0], columnsList[1], columnPosition);
                        columnsNotInARow = CheckColumnsRow(columnsList[0], columnsList[1], columnPosition);
                    }

                    else if (colIndex == columnsCount / 2 + 1)
                    {
                        columnsAngleCorrect = CheckColumnsAngle(columnsList[colIndex - 1], columnsList[0], columnPosition);
                        columnsNotInARow = CheckColumnsRow(columnsList[colIndex - 1], columnsList[0], columnPosition);
                    }

                    else
                    {
                        columnsAngleCorrect = CheckColumnsAngle(columnsList[colIndex - 1], columnsList[colIndex - 2], columnPosition);
                        columnsNotInARow = CheckColumnsRow(columnsList[colIndex - 1], columnsList[colIndex - 2], columnPosition);
                    }

                }

                if (columnsAngleCorrect && columnsNotInARow)
                {
                    columnDistanceCorrect = CheckColumnsDistance(columnPosition);

                    if (columnDistanceCorrect)
                    {
                        columnVisible = CheckColumnsVisible(columnPosition);

                        if (columnVisible)
                            columnPositionCorrect = true;
                    }

                }
            }

            
            
        }

        if (d == 5000)
            Debug.LogError("no i DUPA #######################################");

        //Debug.Log("KOLUMNY USTAWIONE");

        return columnPosition;
    }

    Vector3 GetRandomPosition(GameObject prevColumn, Vector3 columnPosition, int direction)
    {

        List<float> possiblePositionsTemp = new List<float>();
        float x = 0;
        float z = prevColumn.transform.position.z + columnsDistance;
        int k = 10;

        for (int i=1; i<k; i++)
        {
            z -= 2 * columnsDistance / k;
           
            if (z >= customSettings.caveOrigin.transform.position.z + 2.0f)
                possiblePositionsTemp.Add(z);
            
        }

        

        int xx = possiblePositionsTemp.Count - 2;
        possiblePositionsTemp.Sort();
        List<float> possiblePositions = possiblePositionsTemp.GetRange(1, xx);

        int index = random.Next(0, xx);
        //int index = 1;

        columnPosition.z = possiblePositions[index];
        if (direction == 0)
            x = Mathf.Sqrt(columnsDistance * columnsDistance - Mathf.Pow(columnPosition.z - prevColumn.transform.position.z, 2)) + prevColumn.transform.position.x;
        else
            x = (-1) * Mathf.Sqrt(columnsDistance * columnsDistance - Mathf.Pow(columnPosition.z - prevColumn.transform.position.z, 2)) + prevColumn.transform.position.x;

        columnPosition.x = x;
        
        return columnPosition;
    }

    bool CheckColumnsRow(GameObject prevColumn, GameObject prev2Column, Vector3 currentPosition)
    {

        // Sprawdzenie, czy trzy kolejne kolumny nie mają rosnącej/malejącej (lub zbliżonej) wartości zmiennej z.
        // Ma to na celu zapobieganie układania się kolumn w jednolity rząd.

        float approxValue = 1.5f;

        if ((currentPosition.z >= prevColumn.transform.position.z) && (currentPosition.z <= prevColumn.transform.position.z + approxValue))
        {
            if ((prevColumn.transform.position.z >= prev2Column.transform.position.z) && (prevColumn.transform.position.z <= prev2Column.transform.position.z + approxValue))
                return false;
            else
                return true;
        }
        else if ((currentPosition.z <= prevColumn.transform.position.z) && (currentPosition.z >= prevColumn.transform.position.z - approxValue))
        {
            if ((prevColumn.transform.position.z <= prevColumn.transform.position.z) && (prevColumn.transform.position.z >= prevColumn.transform.position.z - approxValue))
                return false;
            else
                return true;
        }
        else
            return true;

    }

    bool CheckColumnsAngle(GameObject prevColumn, GameObject prev2Column, Vector3 currentPosition)
    {

        //Jeżeli kąt pomiędzy dwoma poprzedzającymi kolumnami jest zbyt mały, to może grozić zablokowaniem domina dla następnych kolumn.
        //Sprawdzenie tangensa kąta pomiędzy tymi dwoma kolumnami (tg(30) = sqrt(3)/3, tg(0) = 0)
        
        float angle = Mathf.Abs(prev2Column.transform.position.x - prevColumn.transform.position.x) / Mathf.Abs(prev2Column.transform.position.z - prevColumn.transform.position.z);

        if ((angle > 0) && (angle < Mathf.Sqrt(3) / 3))
        {
            angle = Mathf.Abs(currentPosition.x - prevColumn.transform.position.x) / Mathf.Abs(currentPosition.z - prevColumn.transform.position.z);
            
            if ((angle > 0) && (angle < Mathf.Sqrt(3) / 3))
                return false;
            else
                return true;

        }
 
        else
            return true;
          

        //45 stopni = sqrt(2)/2
        /*float angle = Mathf.Abs(prev2Column.transform.position.x - prevColumn.transform.position.x) / Mathf.Abs(prev2Column.transform.position.z - prevColumn.transform.position.z);

        if ((angle > 0) && (angle < Mathf.Sqrt(2) / 2))
        {
            angle = Mathf.Abs(currentPosition.x - prevColumn.transform.position.x) / Mathf.Abs(currentPosition.z - prevColumn.transform.position.z);

            if ((angle > 0) && (angle < Mathf.Sqrt(2) / 2))
                return false;
            else
                return true;

        }

        else
            return true;
            */

    }

    bool CheckColumnsDistance(Vector3 columnPosition)
    {

        float distance = 0;

        foreach(GameObject column in columnsList)
        {

            distance = Mathf.Sqrt(Mathf.Pow((columnPosition.x - column.transform.position.x),2) + Mathf.Pow((columnPosition.z - column.transform.position.z), 2));
            if (distance <= columnsDistanceMin)
                return false;
        }

        return true;
    }

    bool CheckColumnsVisible(Vector3 columnPosition)
    {
        
        Point p1a, p1b, p2a, p2b, pca, pcb, p4, p5;

        foreach (GameObject column in columnsList)
        {

            if (column.transform.position.x < player1placeCenter.x)
            {
                p1a.x = player1placeCenter.x - playerPlaceSize / 2;
                p1a.z = player1placeCenter.z - playerPlaceSize / 2;
                p1b.x = player1placeCenter.x + playerPlaceSize / 2;
                p1b.z = player1placeCenter.z + playerPlaceSize / 2;

                p4.x = player1placeCenter.x + playerPlaceSize / 2;
                p4.z = player1placeCenter.z - playerPlaceSize / 2;

                if (column.transform.position.x < player2placeCenter.x)
                {
                    p2a.x = player2placeCenter.x - playerPlaceSize / 2;
                    p2a.z = player2placeCenter.z - playerPlaceSize / 2;
                    p2b.x = player2placeCenter.x + playerPlaceSize / 2;
                    p2b.z = player2placeCenter.z + playerPlaceSize / 2;

                    p5.x = player2placeCenter.x + playerPlaceSize / 2;
                    p5.z = player2placeCenter.z - playerPlaceSize / 2;

                }

                else if (column.transform.position.x == player2placeCenter.x)
                {

                    p2a.x = player2placeCenter.x - playerPlaceSize / 2;
                    p2a.z = player2placeCenter.z + playerPlaceSize / 2;
                    p2b.x = player2placeCenter.x + playerPlaceSize / 2;
                    p2b.z = player2placeCenter.z + playerPlaceSize / 2;

                    p5.x = player2placeCenter.x;
                    p5.z = player2placeCenter.z - playerPlaceSize / 2;

                }

                else
                {
                    p2a.x = player2placeCenter.x - playerPlaceSize / 2;
                    p2a.z = player2placeCenter.z + playerPlaceSize / 2;
                    p2b.x = player2placeCenter.x + playerPlaceSize / 2;
                    p2b.z = player2placeCenter.z - playerPlaceSize / 2;

                    p5.x = player2placeCenter.x - playerPlaceSize / 2;
                    p5.z = player2placeCenter.z - playerPlaceSize / 2;
                }
            }

            else
            {
                p2a.x = player2placeCenter.x - playerPlaceSize / 2;
                p2a.z = player2placeCenter.z + playerPlaceSize / 2;
                p2b.x = player2placeCenter.x + playerPlaceSize / 2;
                p2b.z = player2placeCenter.z - playerPlaceSize / 2;

                p5.x = player2placeCenter.x - playerPlaceSize / 2;
                p5.z = player2placeCenter.z - playerPlaceSize / 2;

                if (column.transform.position.x > player1placeCenter.x)
                {

                    p1a.x = player1placeCenter.x - playerPlaceSize / 2;
                    p1a.z = player1placeCenter.z + playerPlaceSize / 2;
                    p1b.x = player1placeCenter.x + playerPlaceSize / 2;
                    p1b.z = player1placeCenter.z - playerPlaceSize / 2;

                    p4.x = player1placeCenter.x - playerPlaceSize / 2;
                    p4.z = player1placeCenter.z - playerPlaceSize / 2;

                }
                else
                {
                    p1a.x = player1placeCenter.x - playerPlaceSize / 2;
                    p1a.z = player1placeCenter.z + playerPlaceSize / 2;
                    p1b.x = player1placeCenter.x + playerPlaceSize / 2;
                    p1b.z = player1placeCenter.z + playerPlaceSize / 2;

                    p4.x = player1placeCenter.x;
                    p4.z = player1placeCenter.z - playerPlaceSize / 2;
                }
            }

            Point p3 = new Point(column.transform.position.x, column.transform.position.z);
            Point c = new Point(columnPosition.x, columnPosition.z);
            
            if (CheckIfOnSide(p1a, p1b, p3, c))
                return false;
            if (CheckIfOnSide(p2a, p2b, p3, c))
                return false;
            if (CheckIfInside(p1a, p1b, p3, c, p4))
                return false;
            if (CheckIfInside(p2a, p2b, p3, c, p5))
                return false;
              
        }

        return true;
    }

    bool CheckIfOnSide(Point p1, Point p2, Point p3, Point c)
    {
        Vector3 lineParameters = new Vector3(0, 0, 0);
        float d = 0f;

        lineParameters = CalculateLine(p1, p2);
        d = Mathf.Abs((lineParameters.x * c.x + lineParameters.y * c.z + lineParameters.z) / (Mathf.Sqrt(Mathf.Pow(lineParameters.x, 2) + Mathf.Pow(lineParameters.y, 2))));
        
        if (d <= columnRadius || d == 0)
            return true;

        lineParameters = CalculateLine(p1, p3);
        d = Mathf.Abs((lineParameters.x * c.x + lineParameters.y * c.z + lineParameters.z) / (Mathf.Sqrt(Mathf.Pow(lineParameters.x, 2) + Mathf.Pow(lineParameters.y, 2))));
      
        if (d <= columnRadius || d == 0)
            return true;

        lineParameters = CalculateLine(p2, p3);
        d = Mathf.Abs((lineParameters.x * c.x + lineParameters.y * c.z + lineParameters.z) / (Mathf.Sqrt(Mathf.Pow(lineParameters.x, 2) + Mathf.Pow(lineParameters.y, 2))));
       
        if (d <= columnRadius || d == 0)
            return true;

        return false;
    }

    bool CheckIfInside(Point p1, Point p2, Point p3, Point c, Point p4)
    {
        Vector3 lineParameters = new Vector3(0, 0, 0);
        Vector3 lineParameters1 = new Vector3(0, 0, 0);

        int crossCount = 0;

        lineParameters = CalculateLine(c, p4);
        lineParameters1 = CalculateLine(p1, p2);

        if (CheckIfLinesCross(lineParameters, lineParameters1))
            crossCount += 1;

        lineParameters1 = CalculateLine(p1, p3);

        if (CheckIfLinesCross(lineParameters, lineParameters1))
            crossCount += 1;

        lineParameters1 = CalculateLine(p2, p3);

        if (CheckIfLinesCross(lineParameters, lineParameters1))
            crossCount += 1;
        
        if (crossCount == 1)
            return true;
        else
            return false;
        
    }

    bool CheckIfLinesCross(Vector3 parameters1, Vector3 parameters2)
    {

        if ((parameters1.x * parameters2.y - parameters1.y * parameters2.x) != 0)
            return true;
        else
            return false;
    }

    Vector3 CalculateLine(Point p1, Point p2)
    {
        Vector3 parameters = new Vector3(0, 0, 0);

        
        parameters.x = -(p1.z - p2.z) / (p1.x - p2.x);
        parameters.y = 1;
        parameters.z = -(p1.z + parameters.x * p1.x);
       
        return parameters;
    }

    public float GetColumnHeight()
    {
        return columnHeight;
    }

    public bool IsSet()
    {
    
        return !notSet;
    }

    public List<GameObject> GetColumnsList()
    {
        return columnsList;
    }

    public float GetColumnRadius()
    {
        return columnRadius;
    }

    public int GetColumnPoints(string column, int player)
    {
        if (player == 1)
        {
            if (columnColor[column] == 1)
                return -1;
            else
                return 2;
        }
        else
        {
            if (columnColor[column] == 2)
                return -1;
            else
                return 2;

        }
    }

    public int GetColumnColor(string column)
    {

        return columnColor[column];

    }

    public Texture GetColumnTexture(string column)
    {


        if (columnColor[column] == 1)
        {
            return columnTextureGreen;
        }
        else
            return columnTextureRed;
    }

    public Material GetColumnMaterial(string column)
    {

        if (columnColor[column] == 1)
        {
            return materialGreen;
        }
        else
            return materialRed;
    }

    public void ClearStates()
    {
        columnsList.Clear();
        columnsList = new List<GameObject>();
        init = false;
        notSet = true;
        columnColor.Clear();
        columnColor = new Dictionary<string, int>();
        color1 = 0;
        color2 = 0;
        startDelay = false;
        yPosition = 0;
        movedTowards = false;
}

    struct Point
    {
        public float x, z;

        public Point(float px, float pz)
        {
            x = px;
            z = pz;
        }
    }
}
