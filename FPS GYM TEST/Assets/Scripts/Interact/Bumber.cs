using System;
using Controller;
using UnityEngine;

namespace Interact
{
    public class Bumber : MonoBehaviour
    {
        [Header("Parameters")] 
        [SerializeField] private float strenghtBumperUp;
        [SerializeField] private float strenghtBumperForward;
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerController player = other.body.GetComponent<PlayerController>();
                Vector3 directionNormalBumber = (player.transform.position - transform.position).normalized;
                Vector3 forceToApply = directionNormalBumber * strenghtBumperUp + player.GetOrientation().forward * strenghtBumperForward;
                other.rigidbody.AddForce(forceToApply,ForceMode.Impulse);
            }
        }
    }
}
