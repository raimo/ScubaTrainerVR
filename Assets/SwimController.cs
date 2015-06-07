using UnityEngine;
using System.Collections;

public class SwimController : MonoBehaviour {
    private HandController handController = null;
    private Rigidbody physicsBody = null;
    private Vector3 totalVelocity = Vector3.zero;

    public float massFudge = .01f;
    public float rightingForce = 100;
    public float glideFactor = .2f;

	void Start () {
        handController = GetComponentInChildren<HandController> ();
        physicsBody = GetComponent<Rigidbody> ();
	}
	
	void FixedUpdate () {
        if (handController && physicsBody) {
            HandModel[] hands = handController.GetAllPhysicsHands ();
            foreach (HandModel hand in hands) {
                Vector3 velocity = hand.GetPalmVelocity ();
                Vector3 normal = hand.GetPalmNormal ();
                Vector3 swimVelocity = -Vector3.Dot (velocity, normal) * normal.normalized * Time.deltaTime * massFudge;
                physicsBody.AddForceAtPosition (swimVelocity, hand.transform.position, ForceMode.Acceleration);
            }
        }
        if (Vector3.Dot (transform.up, Vector3.up) < 1.0) {
            Vector3 horizontal = new Vector3 (transform.up.x, 0, transform.up.z);
            physicsBody.AddRelativeTorque (horizontal * rightingForce * Time.deltaTime, ForceMode.Impulse);
        }
        physicsBody.AddForce (totalVelocity * Time.deltaTime, ForceMode.VelocityChange);
        totalVelocity = physicsBody.velocity * glideFactor;

        if (transform.position.y > 0) {
            physicsBody.velocity = new Vector3 (physicsBody.velocity.x, physicsBody.velocity.y * .6f, physicsBody.velocity.z);
        }
	}
}
