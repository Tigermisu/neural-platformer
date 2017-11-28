using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathCollider : MonoBehaviour {
    public bool isDeaded = false;

    private void OnCollisionEnter2D(Collision2D collision) {
        isDeaded = true;
    }

    private void OnCollisionExit2D(Collision2D collision) {
        isDeaded = false;
    }
}
