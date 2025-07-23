using System;
using UnityEngine;

namespace Controller
{
    public class Grappling : MonoBehaviour
    {
        #region Fields
        
        //https://www.youtube.com/watch?v=8nENcDnxeVE&ab_channel=Affax
        
        [Header("References")]
        [SerializeField] private PlayerController playerControoller;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform gunTip;
        [SerializeField] private LayerMask canBeGrabbedLayer;
        [SerializeField] private LineRenderer lineRenderer;
        
        [Header("Grappling")]
        [SerializeField] private float maxGrappleDistance;
        [SerializeField] private float grappleDelayTime;
        [SerializeField] private float overShootYAxis;

        private Vector3 grapplePoint;
        
        [Header("CoolDown")]
        [SerializeField] private float grapplingCd;
        [SerializeField] private float grapplingCdTimer;

        [Header("Input")] 
        private bool grappling;
        [SerializeField] private KeyCode grappleKey = KeyCode.Mouse1;

        #endregion

        #region Unity Methods
        
        private void Update()
        {
            InputGrapple();
        }

        private void LateUpdate()
        {
            if (grappling)
            {
                lineRenderer.SetPosition(0, gunTip.position);
            }
        }

        #endregion


        #region Grappling Methods

        private void InputGrapple()
        {
            if (Input.GetKeyDown(grappleKey))
            {
                StartGrapple();
            }

            if (grapplingCdTimer > 0)
            {
                grapplingCdTimer -= Time.deltaTime;
            }
        }
        
        private void StartGrapple()
        {
            if (grapplingCdTimer > 0)
            {
                return;
            }

            grappling = true;
            
            playerControoller.SetterBoolFreezing(true);
            
            RaycastHit hit;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxGrappleDistance,canBeGrabbedLayer))
            {
                grapplePoint = hit.point;
                
                Invoke(nameof(ExecuteGrapple),grappleDelayTime);
            }
            else
            {
                grapplePoint = cameraTransform.position + cameraTransform.forward * maxGrappleDistance;
                Invoke(nameof(StopGrapple),grappleDelayTime);
            }
            
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(1, grapplePoint);
        }

        private void ExecuteGrapple()
        {
            playerControoller.SetterBoolFreezing(false);
            
            Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
            float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
            float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;

            if (grapplePointRelativeYPos < 0 )
            {
                highestPointOnArc = overShootYAxis;
            }
            
            playerControoller.JumpToPoint(grapplePoint, highestPointOnArc);
            Invoke(nameof(StopGrapple),1f);
        }

        public void StopGrapple()
        {
            playerControoller.SetterBoolFreezing(false);
            grappling = false;
            grapplingCdTimer = grapplingCd;
            lineRenderer.enabled = false;
        }

        
        
        

        #endregion
    }
}
