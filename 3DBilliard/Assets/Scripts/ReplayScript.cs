using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayScript : MonoBehaviour
{
    public Vector3 initialPosition;
    public GameObject mainCamera;
    public GameObject whiteBall;
    public GameObject redBall;
    public GameObject yellowBall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (GameLogic.replay)
        {
            // free horizontal rotation
            this.transform.RotateAround(Vector3.zero, Vector3.up, Input.GetAxis("Mouse X"));

            // when all the balls are still, end the replay, reset this camera position, and return to the main camera
            if (whiteBall.GetComponent<Rigidbody>().IsSleeping() && redBall.GetComponent<Rigidbody>().IsSleeping() && yellowBall.GetComponent<Rigidbody>().IsSleeping())
            {
                this.GetComponent<Camera>().enabled = false;
                mainCamera.GetComponent<Camera>().enabled = true;

                this.transform.position = initialPosition;
                this.transform.LookAt(Vector3.zero);

                GameLogic.replay = false;
            }
        }
    }
}
