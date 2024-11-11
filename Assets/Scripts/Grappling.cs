using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    private Vector3 currentGrapplePosition;

    [Header("Input")]
    public KeyCode swingKey = KeyCode.E;

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
    


    void Update() {
        if (Input.GetKeyDown(swingKey)) StartSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();
    }

    void LateUpdate() {
        DrawRope();
    }

    private void StartSwing() {
        pm.grappling = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, isGrappleable)) {
            swingPoint = hit.point;
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
}
