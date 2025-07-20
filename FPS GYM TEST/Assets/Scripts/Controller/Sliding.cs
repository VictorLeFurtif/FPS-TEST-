using System;
using System.Threading;
using UnityEngine;

namespace Controller
{
    public class Sliding : MonoBehaviour
    {
        #region Fields

        [Header("References")]
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform playerObj;

        private Rigidbody rb;
        private PlayerController pc;

        [Header("Sliding")] 
        [SerializeField] private float maxSlideTime;
        [SerializeField] private float slideForce;
        private float slideTimer;
        
        [SerializeField] private float slideYScale;
        private float startYScale;

        
        [Header("Input")] 
        [SerializeField] private KeyCode slideKey = KeyCode.LeftControl;
        private float horizontalInput;
        private float verticalInput;
        
        #endregion

        #region Unity Methods

        private void Start()
        {
            InitComponent();
        }

        private void Update()
        {
            InputMovement();
        }

        private void FixedUpdate()
        {
            if (pc.GetSliding())
            {
                SlidingMovement();
            }
        }

        #endregion

        #region Init

        private void InitComponent()
        {
            rb = GetComponent<Rigidbody>();
            pc = GetComponent<PlayerController>();
            startYScale = playerObj.localScale.y;
        }

        #endregion

        #region Slide Methods
        
        
        
        private void InputMovement()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0) && !pc.GetWallRunning())
            {
                StartSlide();
            }

            if (Input.GetKeyUp(slideKey) && pc.GetSliding())
            {
                StopSlide();
            }
        }
        
        private void StartSlide()
        {
            pc.SetterBoolSliding(true);
            playerObj.localScale = new Vector3(playerObj.localScale.x,slideYScale, playerObj.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            slideTimer = maxSlideTime;
        }

        private void SlidingMovement()
        {
            Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (!pc.OnSlope() || rb.linearVelocity.y > -0.1f)
            {
                rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

                slideTimer -= Time.deltaTime;
            }
            else
            {
                rb.AddForce(pc.GetSlopeMoveDirection(inputDirection) * slideForce,ForceMode.Force);
            }
            
            if (slideTimer <= 0)
            {
                StopSlide();
            }
        }
        
        private void StopSlide()
        {
            pc.SetterBoolSliding(false);
            playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
        }

        #endregion
    }
}
