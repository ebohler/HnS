using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    private Vector3 currentGrapplePosition;

    [Header("Input")]
    public KeyCode swingKey = KeyCode.E;
    public KeyCode retractKey = KeyCode.LeftShift;

    [Header("References")]
    public LineRenderer lr;
    public Transform gunTip, cam, player;
    public LayerMask isGrappleable;
    public PlayerMovement pm;

    [Header("Swinging")]
    public float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;

    [Header("Joint")]
    public float maxDistanceMult = 0.8f;
    public float minDistanceMult = 0.25f;
    public float jointSpring = 4.5f;
    public float jointDamper = 7f;
    public float jointMassScale = 4.5f;

    [Header("Air Movement")]
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;
    


    void Update() {
        if (Input.GetKeyDown(swingKey)) StartSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();

        CheckForSwingPoints();

        if (joint != null) AirMovement();
    }

    void LateUpdate() {
        DrawRope();
    }

    private void StartSwing() {
        // Return if predictionHit not found
        if (predictionHit.point == Vector3.zero) return;

        // Deactivate active grapple
        //if(GetComponent<Grappling>() != null)
        //    GetComponent<Grappling>().StopSwing();
        //pm.ResetRestrictions();

        pm.grappling = true;

        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        // Distance grapple will try to keep from grapple point
        joint.maxDistance = distanceFromPoint * maxDistanceMult;
        joint.minDistance = distanceFromPoint * minDistanceMult;

        joint.spring = jointSpring;
        joint.damper = jointDamper;
        joint.massScale = jointMassScale;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;
    }

    void StopSwing() {
        pm.grappling = false;

        lr.positionCount = 0;
        Destroy(joint);
    }

    void DrawRope() {
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }

    private void AirMovement() {
        // Right
        if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        // Left
        if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

        // Forward
        if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * horizontalThrustForce * Time.deltaTime);

        // Shorten cable
        if (Input.GetKey(retractKey))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * maxDistanceMult;
            joint.minDistance = distanceFromPoint * minDistanceMult;
        }
    }

    private void CheckForSwingPoints() {
        if (joint != null) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit, maxSwingDistance, isGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, isGrappleable);

        Vector3 realHitPoint;

        // Direct hit
        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;
        // Indirect hit
        else if (sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point;
        // Miss
        else
            realHitPoint = Vector3.zero;

        // realHitPoint found
        if (realHitPoint != Vector3.zero) {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        // realHitPoint not found
        else {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }
}
