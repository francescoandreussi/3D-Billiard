using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public float soundNormalisingFactor;

    private Rigidbody myRB;
    private bool redHit = false;
    private bool yellowHit = false;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 newPos = new Vector3(Random.Range(-4.9f, 4.9f), .5f, Random.Range(-2.4f, 2.4f));
        this.transform.position = newPos;

        myRB = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (redHit && yellowHit && !GameLogic.replay)
        {
            GameLogic.score++;
            redHit = false;
            yellowHit = false;
        }

        // when the ball stops reset hits
        if (myRB.IsSleeping())
        {
            redHit = false;
            yellowHit = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // calculate total velocity of impact and play hitting sound
        float mySpeed = myRB.velocity.magnitude;
        float collidedObjectSpeed = 0;
        if (collision.rigidbody != null) { collidedObjectSpeed = collision.rigidbody.velocity.magnitude; }
        float collisionSpeed = mySpeed + collidedObjectSpeed;
        this.GetComponent<AudioSource>().volume = collisionSpeed / soundNormalisingFactor;
        this.GetComponent<AudioSource>().Play();

        // check for collision with both red and yellow ball
        if(this.gameObject.name.Equals("WhiteBall") && collision.gameObject.name.Equals("YellowBall"))
        {
            yellowHit = true;
        }
        if (this.gameObject.name.Equals("WhiteBall") && collision.gameObject.name.Equals("RedBall"))
        {
            redHit = true;
        }
    }
}
