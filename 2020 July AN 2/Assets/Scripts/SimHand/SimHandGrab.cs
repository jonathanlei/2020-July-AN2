﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimHandGrab : MonoBehaviour
{
    public GameObject collidingObject;      // what we're touching
    public GameObject heldObject;           // what we're holding
    public Transform snapPosition;

    [Range(1, 100)]
    public float throwForce = 1f;

    private Vector3 handVelocity;
    private Vector3 previousPosition;

    private Vector3 handAngularVelocity;
    private Vector3 previousAngularRotation;

    private void OnTriggerEnter(Collider otherThingWeTouched)
    {
        collidingObject = otherThingWeTouched.gameObject;       // saving what we're touching
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == collidingObject)                 // checking we've exited the "collidingObject" and not some other
        {
            collidingObject = null;                             // we are no longer touching an object
        }
    }

    void Start()
    {
        
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (collidingObject)
            {
                heldObject = collidingObject;

                // do the grab!
                //Grab();
                AdvGrab();
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (heldObject)
            {
                //Release();
                AdvRelease();
            }
        }

        #region Interaction Method 1

        //if (Input.GetKeyDown(KeyCode.Mouse0) && heldObject)
        //{
        //    heldObject.BroadcastMessage("Interaction");
        //}

        #endregion

        handVelocity = (this.transform.position - previousPosition) / Time.deltaTime;
        previousPosition = this.transform.position;

        handAngularVelocity = (this.transform.eulerAngles - previousAngularRotation) / Time.deltaTime;
        previousAngularRotation = this.transform.eulerAngles;
    }

    private void Grab()
    {
        heldObject.transform.SetParent(snapPosition);
        heldObject.transform.localPosition = Vector3.zero;                  // (0,0,0)
        heldObject.transform.localRotation = Quaternion.identity;           // (0,0,0,0)
        heldObject.GetComponent<Rigidbody>().isKinematic = true;

        #region Interaction Method 2

        var grabbable = heldObject.GetComponent<GrabbableObjectSimHand>();
        if (grabbable)
        {
            grabbable.simHandGrab = this;
            grabbable.isBeingHeld = true;
        }

        #endregion
    }

    private void Release()
    {
        #region Interaction Method 2

        var grabbable = heldObject.GetComponent<GrabbableObjectSimHand>();
        if (grabbable)
        {
            grabbable.simHandGrab = null;
            grabbable.isBeingHeld = false;
        }

        #endregion

        // throw
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        rb.velocity = handVelocity * throwForce;
        rb.angularVelocity = handAngularVelocity * throwForce;

        // reset held object
        heldObject.transform.SetParent(null);
        rb.isKinematic = false;
        heldObject = null;
    }

    private void AdvGrab()
    {
        // create the fixed joint!
        FixedJoint fixedJoint = this.gameObject.AddComponent<FixedJoint>();
        fixedJoint.breakForce = 2000;
        fixedJoint.breakTorque = 2000;
        fixedJoint.connectedBody = heldObject.GetComponent<Rigidbody>();

        #region Interaction Method 2

        var grabbable = heldObject.GetComponent<GrabbableObjectSimHand>();
        if (grabbable)
        {
            grabbable.simHandGrab = this;
            grabbable.isBeingHeld = true;
        }

        #endregion
    }

    private void AdvRelease()
    {
        #region Interaction Method 2

        var grabbable = heldObject.GetComponent<GrabbableObjectSimHand>();
        if (grabbable)
        {
            grabbable.simHandGrab = null;
            grabbable.isBeingHeld = false;
        }

        #endregion

        if (GetComponent<FixedJoint>())
        {
            Destroy(GetComponent<FixedJoint>());

            // throw
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            rb.velocity = handVelocity * throwForce;
            rb.angularVelocity = handAngularVelocity * throwForce;
        }

        heldObject = null;
    }
}
