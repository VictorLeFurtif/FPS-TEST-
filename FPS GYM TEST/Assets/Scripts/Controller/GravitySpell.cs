using System;
using UnityEngine;

namespace Controller
{
    public class GravitySpell : MonoBehaviour
    {
        #region Fields

        [Header("Parameters")]
        [SerializeField] private Camera fpsCam;
        [SerializeField] private float gravityStrengthMax;
        private bool shootingSpeel;
        [SerializeField] private float maxDistanceForSpeel;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Transform playerTransform;

        #endregion

        #region Unity Methods

        private void Start()
        {
            InitComponents();
        }

        private void Update()
        {
            InputSpell();
        }

        #endregion

        #region Init

        private void InitComponents()
        {
            fpsCam = Camera.main;

            if (playerController == null)
            {
                playerController = GetComponent<PlayerController>();
            }
        }

        #endregion

        #region Gravity Spell Methods

        private void InputSpell()
        {
            shootingSpeel = Input.GetButtonDown("Fire1");

            if (shootingSpeel)
            {
                GravityShoot();
            }
        }

        private void GravityShoot()
        {
            Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray,out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                return; //do nothing because in every case we want the projectile to be close
            }
            if (Vector3.Distance(targetPoint, playerTransform.position) < maxDistanceForSpeel)
            {
                //calcul direction de la force
                Vector3 directionForce = (playerTransform.position - targetPoint).normalized;
                float forceDeal = StrenghtForceByDistance(playerTransform.position, targetPoint);
                playerController.GetPlayerRigidbody().AddForce(directionForce * forceDeal, ForceMode.Impulse);
            }
            else
            {
                return; //not close enough so fail
            }
        }

        private float StrenghtForceByDistance(Vector3 playerPosition, Vector3 targetPosition)
        {
            float distance = (playerPosition - targetPosition).magnitude;
     
            if (distance < 0.1f)
            {
                return gravityStrengthMax;
            }
    
            float ratio = 1f - (distance / maxDistanceForSpeel);
            return ratio * gravityStrengthMax;

            Vector3 originalVector;
            Vector3 perpendicular = Vector3.Cross(originalVector, Vector3.right);

            if (perpendicular.y > 0 )
            {
                perpendicular = -perpendicular;
            }
        }
        #endregion
    }
}
