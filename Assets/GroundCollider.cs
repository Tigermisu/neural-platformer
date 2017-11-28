﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour {
    public bool isGrounded = true;

    private void OnTriggerEnter2D(Collider2D collision) {
        isGrounded = true;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        isGrounded = false;
    }
}
