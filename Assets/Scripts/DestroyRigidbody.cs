using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LZWPlib;

public class DestroyRigidbody : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (!Core.Instance.isServer)
        {
            Destroy(transform.GetComponent<Rigidbody>());    
        }

        Destroy(transform.GetComponent<DestroyRigidbody>());
    }
}
