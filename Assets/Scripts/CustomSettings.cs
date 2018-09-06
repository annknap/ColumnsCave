using UnityEngine;
using LZWPlib;


public class CustomSettings : MonoBehaviour {

    public float planeSize;
    public float caveSize;
    public float playerPlaceSize;
    public int fontSize;
    public float fontScale;
    public int columnsCount;
    public float columnsAreaX;
    public float columnsAreaZ;
    public float columnsAreaDistance;
    public float columnsDistanceMin;
    public float columnsDistance;

    float playerSize;
    bool allSet = false;
    
    public GameObject plane;
    public GameObject cave;
    public GameObject player1Place;
    public GameObject player2Place;
    public Camera camera;
    public GameObject glasses;
    public GameObject caveOrigin;
    public GameController gameController;
    public Vector3 addedPosition;

    public int finalColumnsCount = 8;

    void Awake()
    {

        if (Core.Instance.isServer)
        {
            SetFloor();
            //SetCamera();
            //SetGlasses();
        }
    }

    // Use this for initialization
    void Start () {

        

    }

    // Update is called once per frame
    void Update () {

        if (Core.Instance.isServer)
        {
            //SetGlasses();
        }
    }

    void SetFloor()
    {
        plane.transform.localScale = new Vector3(planeSize, planeSize, planeSize);
        cave.transform.localScale = new Vector3(caveSize, caveSize, caveSize);
        player1Place.transform.localScale = new Vector3(playerPlaceSize, playerPlaceSize, playerPlaceSize);
        player2Place.transform.localScale = new Vector3(playerPlaceSize, playerPlaceSize, playerPlaceSize);
        player1Place.transform.GetChild(0).transform.localScale = new Vector3(fontScale, fontScale, fontScale);
        player2Place.transform.GetChild(0).transform.localScale = new Vector3(fontScale, fontScale, fontScale);

        plane.transform.position = new Vector3(plane.transform.position.x, (float)-0.001, plane.transform.position.z);
        cave.transform.position = new Vector3(cave.transform.position.x, (float)-0.0001, cave.transform.position.z);

       
        Mesh caveMesh = cave.GetComponent<MeshFilter>().sharedMesh;

        player1Place.transform.position = new Vector3(cave.transform.position.x + caveMesh.bounds.size.x * caveSize / 4, 0, cave.transform.position.z - caveMesh.bounds.size.x * caveSize / 4);
       
        player2Place.transform.position = new Vector3(cave.transform.position.x - caveMesh.bounds.size.x * caveSize / 4, 0, cave.transform.position.z - caveMesh.bounds.size.x * caveSize / 4);
     

        Mesh player1Mesh = player1Place.GetComponent<MeshFilter>().sharedMesh;
        Mesh player2Mesh = player2Place.GetComponent<MeshFilter>().sharedMesh;

        playerSize = player1Mesh.bounds.size.x * playerPlaceSize;
        
        player1Place.transform.GetChild(0).transform.position = new Vector3(player1Place.transform.position.x, 0, player1Place.transform.position.z + player1Mesh.bounds.size.x * playerPlaceSize * 2 / 4);
        player2Place.transform.GetChild(0).transform.position = new Vector3(player2Place.transform.position.x, 0, player2Place.transform.position.z + player2Mesh.bounds.size.x * playerPlaceSize * 2 / 4);

        allSet = true;
    }

    void SetCamera()
    {
        camera.transform.position = new Vector3(player1Place.transform.position.x, 3.0f, player1Place.transform.position.z);
    }

    /*
    void SetGlasses()
    {
        glasses.transform.localPosition = new Vector3(player1Place.transform.position.x, 3.0f, player1Place.transform.position.z);
    }
    */

    public GameObject GetPlayerPlace(int num)
    {
        if (num == 1)
            return player1Place;
        else
            return player2Place;
    }

    public Vector3 GetPlayerPosition(int num)
    {
        if (num == 1)
            return player1Place.transform.position;
        else
            return player2Place.transform.position;
    }

    public float GetPlayerSize()
    {
        return playerSize;
    }

    public bool AllSet()
    {
        if (allSet)
            return true;
        else
            return false;
    }

    public void SetPlace(GameObject obj, float x, float z)
    {
        
        Vector3 position = new Vector3(obj.transform.position.x + x, obj.transform.position.y, obj.transform.position.z + z);
        obj.transform.position = position;

    }

    public void SetCameraPlace(float x, float z)
    {
        Vector3 position = new Vector3(camera.transform.position.x + x, camera.transform.position.y, camera.transform.position.z + z);
        camera.transform.position = position;

    }
}
