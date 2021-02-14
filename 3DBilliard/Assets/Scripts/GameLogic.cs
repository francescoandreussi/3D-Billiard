using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    [System.Serializable]
    public class ShotData
    {
        public Vector3[] ballPositions = new Vector3[3];    // ordered as white-red-yellow
        public Vector3 shotForce;
    }

    public GameObject replayCamera;
    public GameObject timeText;
    public GameObject scoreText;
    public GameObject shotsText;
    public GameObject waitText;
    public GameObject replayButton;
    public GameObject cueBall;
    public GameObject redBall;
    public GameObject yellowBall;
    public GameObject arrowSprite;
    public Vector3 dirToBall;
    public float powerIncrease;
    public float maxPower;
    public float minArrowSize;

    public static int score = 0;
    public static int shots = 0;
    public static float gameTime = 0;
    public static bool replay = false;

    private ShotData latestShot = new ShotData();
    private GameObject gameUI;
    private float power = 0;
    private bool loadedShot = false;
    private bool shotExecuted = false;
    private bool isInCorrectPos = false;

    // Start is called before the first frame update
    void Start()
    {
        gameUI = GameObject.FindGameObjectWithTag("GameUI");

        waitText.SetActive(false);

        // camera placed at an angle w.r.t. the cue ball
        this.transform.position = cueBall.transform.position + dirToBall;

        arrowSprite.transform.rotation = Quaternion.Euler(-90, 0, 0);

        replayButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Keeping the camera at fixed height
        this.transform.position = new Vector3(this.transform.position.x, .525f, this.transform.position.z);

        // UI Updates
        shotsText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Shots: " + shots);
        scoreText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Score: " + score);
        timeText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Time: " + (int)gameTime + "s");

        // Camera + Sprite position controls BEFORE SHOT (during replay every control on the main camera is disabled)
        if (!shotExecuted && !replay)
        {
            // activate game UI
            if (!gameUI.activeInHierarchy)
            {
                gameUI.SetActive(true);
            }

            // if the white ball is rolling around (rare but possible due to collisions at spawning), follow it (leaving a range of tolerance for possible rounding errors)
            if (!isInCorrectPos)
            {
                arrowSprite.SetActive(false);
                this.transform.position = Vector3.MoveTowards(this.transform.position, cueBall.transform.position + dirToBall, .1f);
                this.transform.LookAt(cueBall.transform.position, Vector3.up);

                // as soon as the camera pos is recuperated correctly set isIn CorrectPos to true exiting this branch of the if-statement and enabling user controls
                isInCorrectPos = (this.transform.position - (cueBall.transform.position + dirToBall)) == Vector3.zero;
            }
            else // when the ball does not move allow the user to change POV and to hit the cue ball
            {
                // Game Win
                if (score == 3)
                {
                    SaveData.SaveToJSON(score, shots, (int)gameTime);
                    SceneManager.LoadScene("GameOverScene");
                }

                gameTime += Time.deltaTime; // gameTime is increased only during active phase of the game (when the user can interact)

                // when the ball is still, the arrow sprite should appear
                if (!arrowSprite.activeInHierarchy)
                {
                    arrowSprite.SetActive(true);
                }
                // make replay button appear
                if (!replayButton.activeInHierarchy)
                {
                    replayButton.SetActive(true);
                }

                //updating position, orientation and size of the arrow
                float arrowSize = minArrowSize + power;
                arrowSprite.transform.position = cueBall.transform.position + new Vector3(0f, -.12f, 0f) + 
                    -arrowSprite.transform.right * (arrowSize * .1f); // position below and in front of the ball (increasing its size, the arrow needs to be moved forward)
                arrowSprite.transform.rotation = Quaternion.Euler(-90, 0, this.transform.localRotation.eulerAngles.y + 90); //rotation aligned with camera and table plane
                arrowSprite.transform.localScale = new Vector3(arrowSize, arrowSprite.transform.localScale.y, arrowSprite.transform.localScale.z);
                
                // rotation around the cue ball
                this.transform.RotateAround(cueBall.transform.position, Vector3.up, Input.GetAxis("Mouse X"));

                // loading mechanic when the spacebar is down
                if (Input.GetKey(KeyCode.Space))
                {
                    power += powerIncrease;
                    power = Mathf.Min(power, maxPower); // limiting the power of the shot
                    loadedShot = true;
                }

                // specebar released and shot executed
                if (loadedShot && !(Input.GetKey(KeyCode.Space)))
                {
                    Vector3 shotForce = new Vector3(
                        (cueBall.transform.position.x - this.transform.position.x) * power,
                        0,
                        (cueBall.transform.position.z - this.transform.position.z) * power);

                    // saving data for replay
                    latestShot.shotForce = shotForce;
                    latestShot.ballPositions[0] = cueBall.transform.position;
                    latestShot.ballPositions[1] = redBall.transform.position;
                    latestShot.ballPositions[2] = yellowBall.transform.position;

                    // execute the shot
                    cueBall.GetComponent<Rigidbody>().AddForce(shotForce, ForceMode.Impulse);

                    // after the shot the arrow should disappear
                    arrowSprite.SetActive(false);
                    // after the shot the replay button should disappear
                    replayButton.SetActive(false);

                    shots++;
                    power = 0;
                    loadedShot = false;
                    shotExecuted = true;
                    
                }
                
            }
        }
        else if (shotExecuted)
        {
            // rotation world y-axis 
            this.transform.Rotate(this.transform.InverseTransformVector(Vector3.up), Input.GetAxis("Mouse X"));
            if (!waitText.activeInHierarchy)
            {
                waitText.SetActive(true);
            }

            // when the shot is executed, as soon as the ball is still, focus on the cue ball again
            if (cueBall.GetComponent<Rigidbody>().IsSleeping())
            {
                print("Still ball");
                waitText.SetActive(false);
                isInCorrectPos = false;
                shotExecuted = false;
            }
        }
        else if (replay)
        {
            if (arrowSprite.activeInHierarchy)
            {
                arrowSprite.SetActive(false);
            }

            if (replayButton.activeInHierarchy)
            {
                replayButton.SetActive(false);
            }

            if (gameUI.activeInHierarchy)
            {
                gameUI.SetActive(false);
            }

            // if replay enabled switch camera and execute the latest shot once again
            if (this.GetComponent<Camera>().enabled)
            {
                this.GetComponent<Camera>().enabled = false;
                replayCamera.GetComponent<Camera>().enabled = true;

                cueBall.transform.position      = latestShot.ballPositions[0];
                redBall.transform.position      = latestShot.ballPositions[1];
                yellowBall.transform.position   = latestShot.ballPositions[2];

                cueBall.GetComponent<Rigidbody>().AddForce(latestShot.shotForce, ForceMode.Impulse);
            }
        }
        
    }
}
