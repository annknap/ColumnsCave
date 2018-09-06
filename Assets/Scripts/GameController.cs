using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SceneManagement;
using LZWPlib;

public class GameController : MonoBehaviour {

    int player1Points = 0;
    int player2Points = 0;
    int level = 1;
    public GameObject player1Place;
    public GameObject player2Place;
    public bool devMode = true;
    public int levelNum = 1;

    // Use this for initialization
    void Start () {

        player1Points = 0;
        player2Points = 0;
        level = 1;

	}
	
	// Update is called once per frame
	void Update () {

        if (Core.Instance.isServer)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                Core.Instance.LoadScene("MainScene");
            }
        }

    }
	
	 public void UpdatePoints(int player, int points)
    {
        if (player == 1)
        {
            player1Points += points;
            player1Place.transform.GetChild(0).GetComponent<TextMesh>().text = player1Points.ToString();
        }
        else
        {
            player2Points += points;
            player2Place.transform.GetChild(0).GetComponent<TextMesh>().text = player2Points.ToString();
        }
        
    }


    public int GetLevel()
    {
        return level;
    }


    public void UpdateLevel()
    {
        level += 1;
    }
}
