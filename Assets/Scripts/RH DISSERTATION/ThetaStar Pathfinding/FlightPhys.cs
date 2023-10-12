using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The flight physics system! All agent flight is controlled here in how he does the stuff. 
/// ONLY COLLISION STUFF IS CALLED HERE (flight is actually called somewhere else but this is where it resides)
/// </summary>
public class FlightPhys : MonoBehaviour
{
    [Tooltip("speed of the acceleration for the flight")]
    [SerializeField] float flightSpeed; 

    [Tooltip("Sensitivity of the directional rotation")]
    [SerializeField] float sensitivity;

    [Tooltip("the strenth of the aggresive turn angle")]
    [SerializeField] float aggressiveTurnAngle;

    [SerializeField] Vector3 turnTorque;

    [SerializeField] [Range(-1f, 1f)] private float pitchr = 0f;
    [SerializeField] [Range(-1f, 1f)] private float yawr = 0f;
    [SerializeField] [Range(-1f, 1f)] private float rollr = 0f;

    [SerializeField] Rigidbody flightRB;        // flight rigidbody 
    [SerializeField] LayerMask untraversable;  // Untraversable physics layer
    [SerializeField] CellMapping cellMapping; 

    private float forwardSpeed;
    private Vector3 collisionPosition; // the position of where the glider hits a wall (if it does.)

    private float LookaheadDistance { get { return cellMapping.cellRadius * 3; } } // getting the lookahead distance for the collision avoidance braking

    public float Pitch { set { pitchr = Mathf.Clamp(value, -1f, 1f); }  // getting the pitch debug for viewing how the agent is reacting (Extremely useful for fine tuning the flight physics)
        get { return pitchr; } }

    public float Yaw { set { yawr = Mathf.Clamp(value, -1f, 1f); }      // getting the pitch debug for viewing how the agent is reacting (Extremely useful for fine tuning the flight physics)
        get { return yawr; } }

    public float Roll { set { rollr = Mathf.Clamp(value, -1f, 1f); }    // getting the pitch debug for viewing how the agent is reacting (Extremely useful for fine tuning the flight physics)
        get { return rollr; } }


    private void OnCollisionEnter(Collision collision)
    {
        collisionPosition = -collision.collider.ClosestPoint(gameObject.transform.position); // grabs the inverse of the collision point 
        flightRB.AddForce(collisionPosition, ForceMode.Impulse); // applies the force away from the contact point.
    }

    /// <summary>
    /// The forward flight and braking is called here.
    /// </summary>
    public void flightForward()
    {
        Ray ray = new Ray(transform.position, transform.forward); // creating a ray direction

        if (Physics.Raycast(ray, LookaheadDistance, untraversable)) // shooting a ray that should prevent them from getting stuck on objects
        {
            Debug.DrawRay(transform.position, transform.forward.normalized * LookaheadDistance, Color.red);

            flightRB.AddForce(-transform.forward * forwardSpeed, ForceMode.Acceleration); // Reverse course if they are about to crash (braking)
            forwardSpeed = Mathf.Lerp(forwardSpeed, flightSpeed, 0.2f * Time.deltaTime);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward.normalized * LookaheadDistance, Color.blue);



            forwardSpeed = Mathf.Lerp(forwardSpeed, flightSpeed, 0.2f * Time.deltaTime); // interpolates the end speed from the flight speed from delta time
            flightRB.AddForce(transform.forward * forwardSpeed, ForceMode.Acceleration);// Go forth! fly through the sky in the forwards direction!
        }
    }
    /// <summary>
    /// rotates the agent towards the "flightDirection", a position in world space
    /// controls agents YAW, ROLL and PITCH and limits them to force natrual looking flying movement
    /// </summary>
    /// <param name="flightDirection"></param>
    public void flightRotate(Vector3 flightDirection)
    {
        Vector3 localFlyTarget = transform.InverseTransformPoint(flightDirection).normalized * sensitivity; // getting the flight target direction from worldspace and normalising it
                                                                                                           // adding sensitivity modifier to control how rigidly it pulls itself to that direction

        float angleOffTarget = Vector3.Angle(transform.forward, (flightDirection - transform.position)); // returns the strength of how off target the agent is
        float agressiveRoll = Mathf.Clamp(localFlyTarget.x, -1f, 1f);                                   // max angle of turn into target
        float wingsLevelRoll = transform.right.y;                                                      // the roll control to return the glider to facing up
        float wingsLevelInfluence = Mathf.InverseLerp(0f, aggressiveTurnAngle, angleOffTarget);       // level of influence the aggresive turn and angle off target have on the wings

        yawr = Mathf.Clamp(localFlyTarget.x, -1f, 1f);                                              // yaw of the glider
        pitchr = -Mathf.Clamp(localFlyTarget.y, -1f, 1f);                                          // pitch of the glider
        rollr = Mathf.Lerp(wingsLevelRoll, agressiveRoll, wingsLevelInfluence);                   // roll of the glider

        flightRB.AddRelativeTorque(new Vector3(turnTorque.x * pitchr, turnTorque.y * yawr, -turnTorque.z * rollr) * 1f, ForceMode.Force); // applying forces
    }


}
