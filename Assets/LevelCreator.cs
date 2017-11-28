using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour {
    public GameObject floorPrefab,
        sawPrefab,
        boxPrefab;

    public const float MAX_JUMP_SIZE = 4f,
        MIN_JUMP_SIZE = 2f;

    Player player;
    Transform playerTransform;
    float currentXCoordinate = 20,
        aheadClearance = 30,
        behindClearance = 15;
   

    Queue<GameObject> placedPieces;

    Queue<Vector2> jumpInformation;
    Queue<float> hazardPositions;
    Queue<float> cubePositions;
    ScoreHandler scoreHandler;

    public Vector2 getNextJumpInfo() {
        if (jumpInformation.Count > 0) { 
            return jumpInformation.Peek();
        }
        return new Vector2(float.MaxValue, 0);
    }

    public void restart() {
        currentXCoordinate = 20;
        foreach (GameObject p in placedPieces) {
            Destroy(p);
        }

        placedPieces = new Queue<GameObject>();
        jumpInformation = new Queue<Vector2>();
        hazardPositions = new Queue<float>();
        cubePositions = new Queue<float>();
    }

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerTransform = player.GetComponent<Transform>();
        placedPieces = new Queue<GameObject>();
        jumpInformation = new Queue<Vector2>();
        hazardPositions = new Queue<float>();
        cubePositions = new Queue<float>();
        scoreHandler = GameObject.Find("GameMasterCanvas").GetComponent<ScoreHandler>();
    }

    void Update() {
        if (playerTransform.position.x + aheadClearance > currentXCoordinate) {
            placeSection();
        }
    
        if (placedPieces.Peek().transform.position.x < playerTransform.position.x - behindClearance) {
            Destroy(placedPieces.Dequeue());
        }

        if (jumpInformation.Count > 0) { 
            Vector2 nextJump = jumpInformation.Peek();
            if (nextJump.x + nextJump.y < playerTransform.position.x) {
                jumpInformation.Dequeue();
                scoreHandler.addScore(5);
            }
        }

        if (hazardPositions.Count > 0) {
            float nextHazard = hazardPositions.Peek();
            if (nextHazard < playerTransform.position.x - 1 && player.isAlive) {
                hazardPositions.Dequeue();
                scoreHandler.addScore(5);
            }
        }

        if (cubePositions.Count > 0) {
            float nextCube = cubePositions.Peek();
            if (nextCube - 0.5f < playerTransform.position.x && player.isAlive) {
                cubePositions.Dequeue();
                scoreHandler.addScore(-1);
            }
        }
    }

    void placeSection() {
        GameObject newFloor = Instantiate(floorPrefab) as GameObject;
        float xSize = newFloor.GetComponent<Renderer>().bounds.size.x;

        if (Random.value < 0.4f) {
            float jumpSize = Random.Range(MIN_JUMP_SIZE, MAX_JUMP_SIZE);
            jumpInformation.Enqueue(new Vector2(currentXCoordinate - xSize / 2, jumpSize));
            currentXCoordinate += jumpSize;
        }

        newFloor.transform.position = new Vector3(currentXCoordinate, -5.5f, -0.01f);

        placeProp(newFloor.transform, xSize);


        currentXCoordinate += xSize;
        placedPieces.Enqueue(newFloor);
    }

    void placeProp(Transform parent, float bounds) {
        float start = currentXCoordinate - bounds / 2 + 2.5f * bounds / 10;
        for (int i = 0; i < 2; i++) {
            GameObject prop;
            float yPos,
                xPos = Random.value * 1.5f * bounds / 10;

            if (i == 1) {
                xPos = xPos * 3.5f / 5 + 4 * bounds / 10;
            }
            
            if (Random.value < 0.334f) {
                prop = sawPrefab;
                hazardPositions.Enqueue(start + xPos);
                yPos = -2.2f;
            } else {
                prop = boxPrefab;
                if (Random.value < 0.5f) {
                    yPos = -2.3f;
                    hazardPositions.Enqueue(start + xPos);
                } else {
                    yPos = 1.5f;
                    cubePositions.Enqueue(start + xPos);
                }
            }

            prop = Instantiate(prop, parent) as GameObject;

            prop.transform.position = new Vector2(start + xPos, yPos);
        }
    }
}
