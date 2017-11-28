using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour {
    Transform playerTransform;

    // Use this for initialization
    void Start() {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void FixedUpdate() {
        transform.position = new Vector3(playerTransform.position.x + 7.5f, 0, -10);
    }
}


