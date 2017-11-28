using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public AudioClip coinClip,
        jumpClip,
        hazardClip;

    Rigidbody2D rb;
    AudioSource audioSource;
    ScoreHandler scoreHandler;

    PlayerDeathCollider deathCollider;
    GroundCollider groundCollider;

    LevelCreator creator;
    
    bool dead = false;

    float lastRecordedXPos = -100;

    float[] weights = new float[5];



    public bool isAlive {
        get { return !dead; }
    }

	// Use this for initialization
	void Start () {
        rb = transform.GetComponent<Rigidbody2D>();
        audioSource = transform.GetComponent<AudioSource>();
        deathCollider = GetComponentInChildren<PlayerDeathCollider>();
        groundCollider = GetComponentInChildren<GroundCollider>();
        creator = GameObject.Find("GameMasterCanvas").GetComponent<LevelCreator>();
        scoreHandler = GameObject.Find("GameMasterCanvas").GetComponent<ScoreHandler>();

        Application.runInBackground = true;

        StartCoroutine(watchForPlayerStuck());
        
        for (int i = 0; i < weights.Length; i++) {
            weights[i] = Random.value;
        }

        weights[0] = -1;

        scoreHandler.displayWeights(weights);

        StartCoroutine(learn());

    }

    // Update is called once per frame
    void Update() {
        if (!dead) {
            rb.velocity = new Vector2(4, rb.velocity.y);
            if (Input.GetKeyDown(KeyCode.Space)) {
                jump(7f);
            } else if (Input.GetKeyDown(KeyCode.LeftControl)) {
                jump(10f);
            }

            if (deathCollider.isDeaded || transform.position.y < -3f) {
                die();
            }

            float[] inputs = getInputs();

            scoreHandler.displayInputs(inputs);

        }

    }

    void jump(float force) {
        if (canJump()) {
            audioSource.PlayOneShot(jumpClip);
            rb.velocity = new Vector2(rb.velocity.x, force);
        }
    }

    bool canJump() {
        return groundCollider.isGrounded;
    }

    void die() {
        if (!dead) { 
            audioSource.PlayOneShot(hazardClip);
            dead = true;
            lastRecordedXPos = -100;
            StartCoroutine(respawnAfterDelay(1f));
        }
    }

    float[] getInputs() {
        float[] inputs = new float[4];
        float maxReportingDistance = 3f;
        RaycastHit2D overheadObstacleHit = Physics2D.Raycast(new Vector2(transform.position.x + 1, 1.5f), Vector2.right, maxReportingDistance),
            groundObstacleHit = Physics2D.Raycast(new Vector2(transform.position.x + 1, -2f), Vector2.right, maxReportingDistance);
        Vector2 nextJump = creator.getNextJumpInfo();
        float distanceToNextJump;

        if (overheadObstacleHit.collider != null) {
            inputs[0] = (maxReportingDistance - Mathf.Abs(overheadObstacleHit.point.x - transform.position.x - 1)) / maxReportingDistance;
        }

        if (groundObstacleHit.collider != null) {
            inputs[1] = (maxReportingDistance - Mathf.Abs(groundObstacleHit.point.x - transform.position.x - 1)) / maxReportingDistance;
        }

        distanceToNextJump = nextJump.x - transform.position.x;

        inputs[2] = distanceToNextJump < 0 ? 1 : distanceToNextJump > maxReportingDistance ? 0 : (maxReportingDistance - distanceToNextJump) / maxReportingDistance;

        inputs[3] = nextJump.y / LevelCreator.MAX_JUMP_SIZE;

        return inputs; 
    }

    IEnumerator respawnAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        dead = false;
        deathCollider.isDeaded = false;
        groundCollider.isGrounded = true;
        scoreHandler.saveMaxScore();
        transform.position = new Vector2(-7.58f, -2.1f);
        creator.restart();
    }

    IEnumerator watchForPlayerStuck() {
        while (true) { 
            yield return new WaitForSeconds(3f);
            if (!dead) { 
                if (transform.position.x < lastRecordedXPos + 3) {
                    die();
                } else {
                    lastRecordedXPos = transform.position.x;
              }
            }
        }
    }

    private IEnumerator learn() {
        bool jumpedPrevious = false;
        float previousScore = scoreHandler.Score;
        float[] previousInputs = new float[4];
        scoreHandler.displayWeights(weights);

        while (true) {
            yield return new WaitForSeconds(0.02f);
            if (dead || (previousScore > scoreHandler.Score && canJump())) {
                adjustWeights(jumpedPrevious? 1 : 0, previousInputs);
                scoreHandler.displayWeights(weights);
                if (dead) {
                    previousScore = 0;
                } else {
                    previousScore = scoreHandler.Score;
                }
                yield return new WaitWhile(() => dead);
            } else if (canJump()) {
                previousInputs = getInputs();
                jumpedPrevious = shouldJump(previousInputs);
                if (jumpedPrevious) {
                    jump(7.2f);
                    yield return new WaitWhile(() => canJump());
                }
                previousScore = scoreHandler.Score;
            }
            
        }
    }

    private bool shouldJump(float[] inputs) {
        float sum = 0;

        

        for (int i = 0; i < inputs.Length; i++) {
            sum += inputs[i] * weights[i + 1];
        }

        return sum + weights[0] > 0;
    }

    private void adjustWeights(float incorrectResult, float[] inputs) {
        float correctResult = 1 - incorrectResult,
            error = correctResult - incorrectResult,
            learnRate = 0.075f;

        weights[0] += learnRate * error;

        for (int i = 1; i < weights.Length; i++) {
            weights[i] += learnRate * error * inputs[i-1];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Coin") {
            audioSource.PlayOneShot(coinClip);
            scoreHandler.addScore(5f);
        } else if (collision.tag == "Hazard") {
            die();
        }
    }
}
