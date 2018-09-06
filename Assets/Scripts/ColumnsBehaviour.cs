using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LZWPlib;


public class ColumnsBehaviour : MonoBehaviour {


    //bool devMode = true;
    public GameObject brokenColumnPrefab;
    ColumnsCreator columnsCreator;
    GameController gameController;
    bool startLevel = false;
    bool levelPrepared = false;
    List<GameObject> columnsList = new List<GameObject>();
    List<GameObject> brokenColumnsList = new List<GameObject>();
    List<GameObject> orderedColumns = new List<GameObject>();
    //System.Random random = new System.Random();
    int delay = 30;
    int delaySelectedColumn = 20;
    int columnsCount;
    int rotationDir = 1;
    bool turn = false;
    bool endOfTurn = false;
    bool calculateNewTurn = true;
    bool columnsStopped = false;
    bool lastColumnFall = false;
    bool end = false;
    bool nextTurn = false;
    public Camera cameraX;
    int playerTurn = 1;
    Vector3 velocity;
    bool startNewLevelBool = false;
    bool moveToNew = false;
    bool invoke = true;
    bool invoke1 = false;
    bool invoke2 = false;
    public Texture columnTextureGreen;
    public Texture columnTextureRed;
    bool endFinished = false;
    float addZ;
    float addX;
    bool columnBroken = false;

    //public 

    Dictionary<GameObject, bool> hitDict = new Dictionary<GameObject, bool>();
    Dictionary<GameObject, bool> collisionDict = new Dictionary<GameObject, bool>();
    Dictionary<GameObject, Vector3> positionDict = new Dictionary<GameObject, Vector3>();
    Dictionary<GameObject, Quaternion> rotationDict = new Dictionary<GameObject, Quaternion>();
    Dictionary<GameObject, bool> moveDict = new Dictionary<GameObject, bool>();
    Dictionary<GameObject, bool> velocityChanged = new Dictionary<GameObject, bool>();
    List<string> turnedColumns = new List<string>();
    GameObject currentColumn;
    RotPoint rotationPoint;
    RotAxis rotationAxis;
    List<RotPoint> rotationPointsFinal = new List<RotPoint>();
    List<RotAxis> rotationAxisFinal = new List<RotAxis>();
    GameObject nextColumn = null;
    GameObject selectedColumn;
	Vector3 lastColumn;
    GameObject endColumn;
    public GameObject FlyStick;
    public Material materialYellow;
    int xxxxxxxxxxx = 100;
    int yyyyyyyyy = 100;
    CustomSettings customSettings;
    List<GameObject> currentFinalColumns = new List<GameObject>();
    List<GameObject> nextFinalColumns = new List<GameObject>();

    void Start () {
        if (Core.Instance.isServer){
            columnsCreator = GameObject.Find("Columns").GetComponent<ColumnsCreator>();
			gameController = GameObject.Find("GameController").GetComponent<GameController>();
            customSettings = GameObject.Find("CustomSettings").GetComponent<CustomSettings>();
        }
    }


    void Update()
    {

        /*
         * Opis zmiennych odpowiadających za etapy rozgrywki
         * startLevel = true - inicjuje początek rozgrywki dla danego poziomu
         * levelPrepared = true - ustawienia dla poziomu przygotowane, zmiana na false, gdy kolumny zaczną się przewracać
         * turn = true - trwa tura = przewracanie kolumn, zmiana na false, gdy "domino" postrzymane, lub wszystkie kolumny przewrócone
         * endOfTurn = true - koniec tury
         */


        /* INICJALIZACJA
         * Jeżeli zakończono przygotowanie kolumn do rozgrywki [ColumnsCreator.cs -> IsSet()]:
         * startLevel = true
         */

        if (Core.Instance.isServer)
        {
            if (columnsCreator.IsSet() && !startLevel && !levelPrepared && !turn && !endOfTurn)
            {
              
                if (columnsCreator.startDelay)
                {
                    {
                    Invoke("StartLevelBool", 3f);
                    columnsCreator.startDelay = false;
                    }
                }
                
            }

            /* PRZYGOTOWANIE DO ROZGRYWKI
             * Po inicjalizacji następuje przygotowanie do rozgrywki - ustawienia kolumn dla poziomu:
             * startLevel = false
             * levelPrepared = true
             */
            if (startLevel && !levelPrepared)
            {

                // lista kolumn przygotowanych dla poziomu
                columnsList = columnsCreator.GetColumnsList();
                // kierunek - pierwsza kolumna do przewrócenia
                int directionStart = CalculateFirstColumn(columnsList);
                Debug.Log("LVL: " + gameController.GetLevel() + "  num " + gameController.levelNum);
                if (gameController.GetLevel() <= gameController.levelNum)
                    CalculateOrderOfColumns(directionStart, columnsList);
      
                else
                {
                    
                    FinalOrderOfColumns(columnsList);
                    
                }
                columnsCount = orderedColumns.Count;
                // początkowe ustawienia słownika kolizji
                foreach (GameObject column in orderedColumns)
                    collisionDict[column] = false;
                foreach (GameObject column in orderedColumns)
                    velocityChanged[column] = false;


                startLevel = false;
                levelPrepared = true;
            }

            /* POCZĄTEK ROZGRYWKI
             * Po przygotowaniach następuje opóźnienie (aby gracze zapoznali się z mapą) i początek rozgrywki:
             * turn = true
             * levelPrepared = false
             */

            if (gameController.GetLevel() <= gameController.levelNum || columnsCreator.IsFinalColumnToBeTurned(nextColumn))
            {

                if (levelPrepared && !startLevel)
                {

                    if (delay == 0)
                    {
                        // pierwsza kolumna do przewrócenia
                        currentColumn = orderedColumns[0];
                        turn = true;
                        levelPrepared = false;

                        foreach (GameObject column in orderedColumns)
                        {
                            positionDict[column] = column.transform.position;
                            rotationDict[column] = column.transform.rotation;

                        }
                    }


                    if (delay > 0)
                        delay -= 1;
                }


                /* ROZGRYWKA */
                if (turn)
                {
                    //Sprawdzenie czy kolumny zostały powstrzymane przez gracza (wycelowanie w kolumnę i wciśnięcie przycisku)
                    CheckIfColumnsStoppedByPlayer();

                    // akcje podejmowane na postawie kolizji
                    foreach (GameObject column in orderedColumns)
                    {

                        if (collisionDict[column])
                        {
                            // jeżeli kolumna została uderzona
                            if (column == nextColumn)
                            {
                                // kolumna ma się poruszać
                                moveDict[column] = true;
                                // ma być obliczone kolejne przewrócenie
                                calculateNewTurn = true;
                                currentColumn = column;
                                collisionDict[column] = false;
                            }

                            // jeżeli kolumna uderzyła w inną
                            if (column == currentColumn)
                            {
                                // kolumna ma się nie poruszać
                                moveDict[column] = false;
                                collisionDict[column] = false;
                                hitDict[column] = true;
                            }
                        }
                    }

                    // obliczenia dla nowego przewrócenia
                    if (columnsCount > 1)
                    {

                        if (calculateNewTurn)
                        {

                            // następna kolumna do przewrócenia
                            nextColumn = orderedColumns[orderedColumns.IndexOf(currentColumn) + 1];
                            moveDict[currentColumn] = true;

                            // oblicz punkt rotacji
                            rotationPoint = GetRotationPoint(currentColumn, nextColumn.transform.position);
                            // oblicz oś rotacji
                            rotationAxis = GetRotAxis(currentColumn, nextColumn, rotationPoint);
                            calculateNewTurn = false;

                            columnsCount -= 1;

                            if (columnsCount == 1)
                            {

                                float dx = nextColumn.transform.position.x - currentColumn.transform.position.x;
                                float dz = nextColumn.transform.position.z - currentColumn.transform.position.z;
                                lastColumn = new Vector3(nextColumn.transform.position.x + dx, nextColumn.transform.position.y, nextColumn.transform.position.z + dz);
                            }

                            turnedColumns.Add(currentColumn.name);


                        }
                    }
                    //ostatnia kolumna
                    else if (columnsCount > 0)
                    {

                        moveDict[currentColumn] = true;

                        // oblicz punkt rotacji
                        rotationPoint = GetRotationPoint(currentColumn, lastColumn);
                        //oś rotacji zostaje jak poprzednia
                        calculateNewTurn = false;
                        lastColumnFall = true;

                    }

                    if (lastColumnFall)
                    {

                        if (currentColumn.transform.position.y <= 2.5)

                        {
                            lastColumnFall = false;
                            columnsCount -= 1;
                            turnedColumns.Add(currentColumn.name);
                        }

                    }

                    // poruszanie się kolumn
                    foreach (GameObject column in orderedColumns)
                    {
                        // podtrzymywana kolumna
                        if (selectedColumn != null)
                        {
                            if (column == selectedColumn)
                            {
                                column.GetComponent<Rigidbody>().velocity = Vector3.zero;
                                float x = positionDict[column].x;
                                float y = column.transform.position.y;
                                float z = positionDict[column].z;
                                positionDict[column] = new Vector3(x, y, z);
                                column.transform.position = positionDict[column];
                                column.transform.rotation = rotationDict[column];

                            }

                        }



                        if (column != selectedColumn)
                        {


                            // pozostałe kolumny
                            if (moveDict[column] != false)
                            {

                                column.transform.RotateAround(new Vector3(rotationPoint.x, rotationPoint.y, rotationPoint.z), new Vector3(rotationAxis.x, rotationAxis.y, rotationAxis.z), rotationDir * 50f * Time.deltaTime);
                                positionDict[column] = column.transform.position;
                                rotationDict[column] = column.transform.rotation;
                            }
                            else
                            {
                                if (hitDict[column])
                                {

                                    {
                                        velocity = column.GetComponent<Rigidbody>().velocity;
                                        column.GetComponent<Rigidbody>().velocity = velocity;
                                        velocityChanged[column] = true;
                                    }

                                }

                            }
                        }


                    }

                    /* KONIEC ROZGRYWKI
                         * Gdy "domino" postrzymane, lub wszystkie kolumny są przewrócone:
                         * turn = false
                         * endOfTurn = true
                     */
                    if (columnsStopped || columnsCount == 0)
                    {
                        turn = false;
                        endOfTurn = true;
                    }
                }

                if (endOfTurn && !end)
                {



                    int points = 0;

                    foreach (string column in turnedColumns)
                    {
                        points += columnsCreator.GetColumnPoints(column, playerTurn);
                    }

                    if (selectedColumn != null)
                    {
                        selectedColumn.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        selectedColumn.transform.position = positionDict[selectedColumn];

                        selectedColumn.transform.rotation = rotationDict[selectedColumn];

                    }

                    gameController.UpdatePoints(playerTurn, points);
                    end = true;
                }

                if (end && !endFinished)
                {

                    GameObject brokenColumn;
                    Vector3 brokenRotation;
                    Vector3 brokenPosition;
                    foreach (GameObject column in columnsList)
                    {

                        if (column != null)
                        {

                            brokenRotation.x = column.transform.eulerAngles.x + 90;
                            brokenRotation.y = column.transform.eulerAngles.y;
                            brokenRotation.z = 0;

                            brokenPosition = column.transform.position;
                            //brokenPosition.y -= 3.0f;

                            brokenColumn = Network.Instantiate(brokenColumnPrefab, brokenPosition, column.transform.rotation, 0) as GameObject;
                            brokenColumn.transform.eulerAngles = brokenRotation;

                            //Debug.Log("column: " + column.transform.position + " " + "broken: " + brokenColumn.transform.position);
                            //Debug.Log("column: " + column.transform.eulerAngles + " broken " + brokenColumn.transform.eulerAngles);

                            Renderer rend;

                            foreach (Transform child in brokenColumn.transform)
                                child.GetComponent<ColorChange>().SetColor(columnsCreator.GetColumnColor(column.name));

                            brokenColumn.name = "broken" + column.name;
                            brokenColumnsList.Add(brokenColumn);

                            Network.Destroy(column);
                        }
                    }

                    if (invoke)
                    {

                        Invoke("MoveToNew", 3.0f);
                        endFinished = true;
                        invoke = false;
                    }
                }

                if (end && endFinished)
                {

                    addZ = customSettings.addedPosition.z;
                    addX = customSettings.addedPosition.x;

                    if (moveToNew)
                    {

                        columnsCreator.moveToNewLocation(addX, addZ);
                        gameController.UpdateLevel();
                        invoke1 = true;
                        moveToNew = false;
                    }

                    if (invoke1)
                    {
                        Invoke("StartNewLevelBool", 3f);
                        invoke1 = false;
                    }

                    if (startNewLevelBool)
                    {
                        startNewLevel();
                        startNewLevelBool = false;
                    }
                }

            }

            /*********************************************************/
            else
            {
                

                if (currentColumn != null)
                {
                    if (columnsCreator.IsColumnToBeBroken(currentColumn))
                    { 
                        //Debug.Log("BUUUUUM");
                        GameObject brokenColumn;
                        Vector3 brokenRotation;
                        Vector3 brokenPosition;
                        brokenRotation.x = currentColumn.transform.eulerAngles.x + 90;
                        brokenRotation.y = currentColumn.transform.eulerAngles.y;
                        brokenRotation.z = 0;

                        brokenPosition = currentColumn.transform.position;

                        brokenColumn = Network.Instantiate(brokenColumnPrefab, brokenPosition, currentColumn.transform.rotation, 0) as GameObject;
                        brokenColumn.transform.eulerAngles = brokenRotation;

                        Renderer rend;

                        foreach (Transform child in brokenColumn.transform)
                        {

                            child.GetComponent<ColorChange>().SetColor(columnsCreator.GetColumnColor(currentColumn.name));
                        }

                        Debug.Log("NUM:  " + columnsCount);
                        Network.Destroy(currentColumn);

                        

                        currentFinalColumns.Add(orderedColumns[orderedColumns.IndexOf(currentColumn) + 1]);
                        currentFinalColumns.Add(orderedColumns[orderedColumns.IndexOf(currentColumn) + 2]);
                        currentColumn = null;
                        nextFinalColumns.Add(orderedColumns[orderedColumns.IndexOf(currentFinalColumns[0]) + 2]);
                        nextFinalColumns.Add(orderedColumns[orderedColumns.IndexOf(currentFinalColumns[1]) + 2]);
                        turn = true;
                        moveDict[currentFinalColumns[0]] = true;
                        moveDict[currentFinalColumns[1]] = true;
                        rotationPointsFinal.Clear();
                        rotationPointsFinal.Add(GetRotationPoint(currentFinalColumns[0], nextFinalColumns[0].transform.position));
                        rotationPointsFinal.Add(GetRotationPoint(currentFinalColumns[1], nextFinalColumns[1].transform.position));
                        //Debug.Log("AAAAAa" + rotationPointsFinal);
                        // oblicz oś rotacji
                        //rotationAxis = GetRotAxis(currentColumn, nextColumn, rotationPoint);
                        rotationAxisFinal.Clear();
                        rotationAxisFinal.Add(GetRotAxis(currentFinalColumns[0], nextFinalColumns[0], rotationPointsFinal[0]));
                        rotationAxisFinal.Add(GetRotAxis(currentFinalColumns[1], nextFinalColumns[1], rotationPointsFinal[1]));
                        columnsCount -= 2;
                        turnedColumns.Add(currentFinalColumns[0].name);
                        turnedColumns.Add(currentFinalColumns[1].name);
                        Debug.Log("XXXXXX" + turnedColumns.Count);
                    }
                }
                else if (columnBroken)
                {

                    //if (currentFinalColumns[1] != orderedColumns[orderedColumns.Count - 1])
                    {

                        

                        /* ROZGRYWKA */
                        if (turn)
                        {

                            
                            //Sprawdzenie czy kolumny zostały powstrzymane przez gracza (wycelowanie w kolumnę i wciśnięcie przycisku)
                            CheckIfColumnsStoppedByPlayer();

                            // akcje podejmowane na postawie kolizji
                            foreach (GameObject column in orderedColumns)
                            {

                                if (collisionDict[column])
                                {
                                    // jeżeli kolumna została uderzona
                                    if (nextFinalColumns.Contains(column))
                                    { 
                                        Debug.Log("NNN" + column.name);
                                        foreach (GameObject nColumn in nextFinalColumns)
                                        {
                                            // kolumna ma się poruszać
                                            moveDict[column] = true;
                                            // ma być obliczone kolejne przewrócenie
                                            calculateNewTurn = true;
                                            
                                            collisionDict[column] = false;
                                        }
                                        currentFinalColumns.Clear();
                                        currentFinalColumns.Add(orderedColumns[orderedColumns.IndexOf(nextFinalColumns[0])]);
                                        currentFinalColumns.Add(orderedColumns[orderedColumns.IndexOf(nextFinalColumns[1])]);


                                    }

                                    // jeżeli kolumna uderzyła w inną
                                    if (currentFinalColumns.Contains(column))
                                    {
                                        foreach (GameObject cColumn in currentFinalColumns)
                                        {
                                            // kolumna ma się nie poruszać
                                            moveDict[cColumn] = false;
                                            collisionDict[cColumn] = false;
                                            hitDict[cColumn] = true;
                                        }
                                    }
                                }
                            }

                            // obliczenia dla nowego przewrócenia
                            if (turnedColumns.Count < 19 && !lastColumnFall)
                            {
                                //Debug.Log("LLL" + columnsCount + " " + currentFinalColumns[0].name);
                                if (calculateNewTurn)
                                {

                                    // następna kolumna do przewrócenia
                                    //nextColumn = orderedColumns[orderedColumns.IndexOf(currentColumn) + 1];
                                    Debug.Log("SSSS" + currentFinalColumns[0].name + " " + columnsCount);
                                    nextFinalColumns.Clear();
                                    nextFinalColumns.Add(orderedColumns[orderedColumns.IndexOf(currentFinalColumns[0]) + 2]);
                                    nextFinalColumns.Add(orderedColumns[orderedColumns.IndexOf(currentFinalColumns[1]) + 2]);
                                    foreach (GameObject cColumn in currentFinalColumns)
                                        moveDict[cColumn] = true;

                                    // oblicz punkt rotacji
                                    //rotationPoint = GetRotationPoint(currentColumn, nextColumn.transform.position);
                                    rotationPointsFinal.Clear();
                                    rotationPointsFinal.Add(GetRotationPoint(currentFinalColumns[0], nextFinalColumns[0].transform.position));
                                    rotationPointsFinal.Add(GetRotationPoint(currentFinalColumns[1], nextFinalColumns[1].transform.position));
                                    //Debug.Log("AAAAAa" + rotationPointsFinal);
                                    // oblicz oś rotacji
                                    //rotationAxis = GetRotAxis(currentColumn, nextColumn, rotationPoint);
                                    rotationAxisFinal.Clear();
                                    rotationAxisFinal.Add(GetRotAxis(currentFinalColumns[0], nextFinalColumns[0], rotationPointsFinal[0]));
                                    rotationAxisFinal.Add(GetRotAxis(currentFinalColumns[1], nextFinalColumns[1], rotationPointsFinal[1]));
                                    calculateNewTurn = false;
                                    
                                    columnsCount -= 2;

                                    /*
                                    if (columnsCount == 2)
                                    {

                                        float dx = nextColumn.transform.position.x - currentColumn.transform.position.x;
                                        float dz = nextColumn.transform.position.z - currentColumn.transform.position.z;
                                        lastColumn = new Vector3(nextColumn.transform.position.x + dx, nextColumn.transform.position.y, nextColumn.transform.position.z + dz);
                                    }
                                    */

                                    turnedColumns.Add(currentFinalColumns[0].name);
                                    turnedColumns.Add(currentFinalColumns[1].name);

                                    Debug.Log("TTT" + turnedColumns.Count);
                                }
                            }
                            //ostatnie kolumny
                            else if ((turnedColumns.Count == 19) && !moveDict[orderedColumns[orderedColumns.Count - 3]])
                            {
                                currentFinalColumns.Clear();
                                currentFinalColumns.Add(orderedColumns[orderedColumns.Count - 2]);
                                currentFinalColumns.Add(orderedColumns[orderedColumns.Count - 1]);

                                Debug.Log("KK" + currentFinalColumns[0].name);

                                moveDict[currentFinalColumns[0]] = true;
                                moveDict[currentFinalColumns[1]] = true;

                                // oblicz punkt rotacji

                                //rotationPoint = GetRotationPoint(currentColumn, lastColumn);
                                rotationPointsFinal.Clear();
                                rotationPointsFinal.Add(GetRotationPoint(currentFinalColumns[0], customSettings.GetPlayerPosition(1)));
                                rotationPointsFinal.Add(GetRotationPoint(currentFinalColumns[1], customSettings.GetPlayerPosition(2)));
                                //oś rotacji zostaje jak poprzednia
                                rotationAxisFinal.Clear();
                                rotationAxisFinal.Add(GetRotAxis(currentFinalColumns[0], customSettings.GetPlayerPlace(1), rotationPointsFinal[0]));
                                rotationAxisFinal.Add(GetRotAxis(currentFinalColumns[1], customSettings.GetPlayerPlace(2), rotationPointsFinal[1]));
                                calculateNewTurn = false;
                                lastColumnFall = true;

                            }

                            if (lastColumnFall)
                            {
                                Debug.Log("PPPPPPPP" + currentFinalColumns[0].name + " " + columnsCount + " " + moveDict[currentFinalColumns[0]] + " " + currentFinalColumns[0].transform.position.y);

                                Debug.Log("aaaaaaa" + currentFinalColumns[0].name);

                                foreach (GameObject column in orderedColumns)
                                {
                                    if (column != null)
                                        Debug.Log(column.name + moveDict[column]);
                                }

                                if (currentFinalColumns[0].transform.position.y <= 1.5f)

                                {
                                    lastColumnFall = false;
                                    columnsCount -= 2;
                                    turnedColumns.Add(currentFinalColumns[0].name);
                                    turnedColumns.Add(currentFinalColumns[1].name);
                                    moveDict[currentFinalColumns[0]] = false;
                                    moveDict[currentFinalColumns[1]] = false;
                                }

                            }

                            if (columnsCount == 0)
                                Debug.Log("OOOOOOOOOO");



                            // poruszanie się kolumn
                            foreach (GameObject column in orderedColumns)
                            {
                                // podtrzymywana kolumna
                                /*if (selectedColumn != null)
                                {
                                    if (column == selectedColumn)
                                    {
                                        column.GetComponent<Rigidbody>().velocity = Vector3.zero;
                                        float x = positionDict[column].x;
                                        float y = column.transform.position.y;
                                        float z = positionDict[column].z;
                                        positionDict[column] = new Vector3(x, y, z);
                                        column.transform.position = positionDict[column];
                                        column.transform.rotation = rotationDict[column];

                                    }

                                }*/



                                if (column != selectedColumn)
                                {


                                    // pozostałe kolumny
                                    if (moveDict[column] != false)
                                    {
                                        //Debug.Log("xxxxxxxx" + rotationPointsFinal.Count);

                                        //Debug.Log(" collll " + column.name + " " + rotationPointsFinal[currentFinalColumns.IndexOf(column)].x + " " + rotationPointsFinal[currentFinalColumns.IndexOf(column)].y);
                                        //Debug.Log(rotationPointsFinal);
                                        if (column.name == "column8a")
                                            Debug.Log("?????");
                                        
                                        column.transform.RotateAround(new Vector3(rotationPointsFinal[currentFinalColumns.IndexOf(column)].x, rotationPointsFinal[currentFinalColumns.IndexOf(column)].y, rotationPointsFinal[currentFinalColumns.IndexOf(column)].z), new Vector3(rotationAxisFinal[currentFinalColumns.IndexOf(column)].x, rotationAxisFinal[currentFinalColumns.IndexOf(column)].y, rotationAxisFinal[currentFinalColumns.IndexOf(column)].z), rotationDir * 50f * Time.deltaTime);
                                        if (column.name == "column8a")
                                            Debug.Log(rotationPointsFinal[currentFinalColumns.IndexOf(column)].x + " " 
                                                + rotationPointsFinal[currentFinalColumns.IndexOf(column)].y + " " 
                                                + rotationPointsFinal[currentFinalColumns.IndexOf(column)].z + " " 
                                                + rotationAxisFinal[currentFinalColumns.IndexOf(column)].x + " " 
                                                + rotationAxisFinal[currentFinalColumns.IndexOf(column)].y + " " 
                                                + rotationAxisFinal[currentFinalColumns.IndexOf(column)].z);

                                        
                                        positionDict[column] = column.transform.position;
                                        rotationDict[column] = column.transform.rotation;
                                    }
                                    else
                                    {
                                        if (hitDict[column])
                                        {
                                            //Debug.Log("yyyyyy");
                                            //Debug.Log(" col " + column.name);
                                            {
                                                velocity = column.GetComponent<Rigidbody>().velocity;
                                                column.GetComponent<Rigidbody>().velocity = velocity;
                                                velocityChanged[column] = true;
                                            }

                                        }

                                    }
                                }


                            }




                        }
                         



                        }


                }
                
                    
            }
        }
        
            
        
    }

    void CheckIfColumnsStoppedByPlayer()
    {
        if (gameController.devMode)
            CheckForColumnSelectDev();

        else
            CheckForColumnSelect();

        if (selectedColumn != null)
        {
            if (hitDict[selectedColumn])

                columnsStopped = true;
        }
    }

    void CheckForColumnSelect()
    {
        if (LzwpTracking.Instance.flysticks[0].fire.isActive)
        {


            RaycastHit hit;
            if (Physics.Raycast(FlyStick.transform.position, FlyStick.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                if (hit.transform.gameObject.name.Contains("column"))
                {
                    selectedColumn = hit.transform.gameObject;
                    moveDict[selectedColumn] = false;
                    if (delaySelectedColumn > 0)
                        delaySelectedColumn -= 1;


                    if (delaySelectedColumn == 0 && hitDict[selectedColumn])
                        columnsStopped = true;
                }

            }
            else
            {
                if (!columnsStopped)
                {
                    delaySelectedColumn = 10;
                    selectedColumn = null;
                }
            }
        }
    }

    void CheckForColumnSelectDev()
    {
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;
            Ray ray = cameraX.ScreenPointToRay(Input.mousePosition);

            if ((Physics.Raycast(ray, out hit)))
            {
                if (hit.transform.gameObject.name.Contains("column"))
                {

                    selectedColumn = hit.transform.gameObject;
                    moveDict[selectedColumn] = false;
                    if (delaySelectedColumn > 0)
                        delaySelectedColumn -= 1;
                }

            }
            else
            {
                if (!columnsStopped)
                {
                    delaySelectedColumn = 10;
                    selectedColumn = null;
                }
            }
        }
    }

    public void DestroyBrokenColumns()
    {
        foreach (GameObject brokenColumn in brokenColumnsList)
        {

            Network.Destroy(brokenColumn);

        }

        brokenColumnsList.Clear();
        brokenColumnsList = new List<GameObject>();
    }

    void StartNewLevelBool()
    {
        startNewLevelBool = true;
    }

    void MoveToNew()
    {
        moveToNew = true;
    }

    void StartLevelBool()
    {
        startLevel = true;
    }

    RotPoint GetRotationPoint (GameObject currCol, Vector3 nextCol)
    {
        float d = Mathf.Sqrt(Mathf.Pow((currCol.transform.position.x - nextCol.x),2) + Mathf.Pow((currCol.transform.position.z - nextCol.z), 2));
        float x, z;
        x = columnsCreator.GetColumnRadius() * Mathf.Abs(currCol.transform.position.x - nextCol.x) / d;


        z = Mathf.Sqrt(Mathf.Pow(columnsCreator.GetColumnRadius(), 2) + Mathf.Pow(x, 2));


        if (nextCol.x < currCol.transform.position.x)
            x = currCol.transform.position.x - x;
        else
            x = currCol.transform.position.x + x;

        if (nextCol.z < currCol.transform.position.z)
            z = currCol.transform.position.z - z;
        else
            z = currCol.transform.position.z + z;

        RotPoint rotPoint = new RotPoint(x, 0.0f, z);
        return rotPoint;
    }

    RotAxis GetRotAxis(GameObject currCol, GameObject nextCol, RotPoint rotPoint)
    {
        float x = Mathf.Abs(nextCol.transform.position.x - currCol.transform.position.x); // - Mathf.Abs(currCol.transform.position.x - rotPoint.x);
        float y = 0;
        float z = Mathf.Abs(nextCol.transform.position.z - currCol.transform.position.z);// - Mathf.Abs(currCol.transform.position.z - rotPoint.z);


        if (nextCol.transform.position.x >= currCol.transform.position.x)
        {
            z = -z;
            if (nextCol.transform.position.z <= currCol.transform.position.z)
                x = -x;
        }

        else 
        {
            if (nextCol.transform.position.z <= currCol.transform.position.z)
                z = -z;
        }

        RotAxis rotAxis = new RotAxis(z, 0, x);
        return rotAxis;

    }

    void turnColumn(GameObject currCol, GameObject nextCol)
    {

        Vector3 rotPoint = new Vector3(currCol.transform.position.x, currCol.transform.position.y - columnsCreator.GetColumnHeight()/2, currCol.transform.position.z);

        float x = Mathf.Abs(nextCol.transform.position.x - currCol.transform.position.x);
        float y = 0;
        float z = Mathf.Abs(nextCol.transform.position.z - currCol.transform.position.z);

        Vector3 rotAxis = new Vector3(z, y, x);
    }

    void FinalOrderOfColumns(List<GameObject> columns)
    {

        for (int i=0; i < columns.Count; i++)
        {
            orderedColumns.Add(columns[i]);
            positionDict[columns[i]] = columns[i].transform.position;
            rotationDict[columns[i]] = columns[i].transform.rotation;

            moveDict[columns[i]] = false;
            hitDict[columns[i]] = false;
        }
    }

    void CalculateOrderOfColumns(int directionStart, List<GameObject> columns)
    {
        List<Point> positions = new List<Point>();

        for (int i = 0; i < columns.Count; i++)
        {
            positions.Add(new Point(columns[i].transform.position.x, columns[i].transform.position.z));
        }

        positions = positions.OrderByDescending(i => i.x).ToList();

        for (int i = 0; i < positions.Count; i++)
        {
            foreach (GameObject column in columns)
            {
                if (column.transform.position.x == positions[i].x && column.transform.position.z == positions[i].z)
                {
                    orderedColumns.Add(column);
                    positionDict[column] = column.transform.position;
                    rotationDict[column] = column.transform.rotation;

                    moveDict[column] = false;
                    hitDict[column] = false;

                    break;
                }
            }
        }
    }

    int CalculateFirstColumn(List<GameObject> columns)
    {
        //int x = random.Next(0, 2);
        int x = 1;

        return x;
    }

    public void CollisionDetected(GameObject obj1, GameObject obj2)
    {
      
        if (obj1.name.Contains("column") && obj2.name.Contains("column")){
            collisionDict[obj2] = true;
            collisionDict[obj1] = true;
        }

        if (obj1.name == "column1a")
            columnBroken = true;
        
        
    }



    void startNewLevel()
    {

        invoke = true;
        invoke1 = false;
        invoke2 = false;
        startLevel = false;
        levelPrepared = false;
        columnsList.Clear();
        columnsList = new List<GameObject>();
        orderedColumns.Clear();
        orderedColumns = new List<GameObject>();
        delay = 30;
        delaySelectedColumn = 20;
        rotationDir = 1;
        turn = false;
        endOfTurn = false;
        calculateNewTurn = true;
        columnsStopped = false;
        lastColumnFall = false;
        end = false;
        nextTurn = false;
        startNewLevelBool = false;
        moveToNew = false;
        endFinished = false;
        playerTurn = 1;


        hitDict.Clear();
        hitDict = new Dictionary<GameObject, bool>();
        collisionDict.Clear();
        collisionDict = new Dictionary<GameObject, bool>();
        positionDict.Clear();
        positionDict = new Dictionary<GameObject, Vector3>();

        rotationDict.Clear();
        rotationDict = new Dictionary<GameObject, Quaternion>();
        moveDict.Clear();
        moveDict = new Dictionary<GameObject, bool>();

        velocityChanged.Clear();
        velocityChanged = new Dictionary<GameObject, bool>();
        turnedColumns.Clear();
        turnedColumns = new List<string>();
        
        nextColumn = null;
        xxxxxxxxxxx = 1000;
        yyyyyyyyy = 1000;

    
        columnsCreator.ClearStates();
        
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

    struct RotAxis
    {
        public float x, y, z;

        public RotAxis(float px, float py, float pz)
        {
            x = px;
            y = py;
            z = pz;
        }
    }

    struct RotPoint
    {
        public float x, y, z;

        public RotPoint(float px, float py, float pz)
        {
            x = px;
            y = py;
            z = pz;
        }
    }
}
