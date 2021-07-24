using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Order the entities in the scene by their Z position
public class ZOrder : MonoBehaviour {
    
	void Update () {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, pos.y);
	}
}
