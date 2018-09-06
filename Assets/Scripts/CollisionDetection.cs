using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LZWPlib;

public class CollisionDetection : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        if (Core.Instance.isServer)
        {
            GameObject.Find("Columns").GetComponent<ColumnsBehaviour>().CollisionDetected(this.gameObject, collision.gameObject);
            
        }

    }
}
