using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    int testInt = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A))
        {
            CardGameTriggerManager.StartListening("BeginTurn", TestTrigger);
        }
	}

    void TestTrigger()
    {
        ++testInt;
        Debug.Log(testInt);
    }
}
