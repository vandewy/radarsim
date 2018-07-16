using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisions : MonoBehaviour {

    BoxCollider col_right_goal;

	// Use this for initialization
	void Start () {
        col_right_goal = GameObject.Find("background_display/right_goal").GetComponent<BoxCollider>();
	}
	
	// Update is called once per 
	void Update () {
		
	}

    private void OnTriggerEnter(Collider aircraft)
    {
        print(aircraft.name);
    }
}
