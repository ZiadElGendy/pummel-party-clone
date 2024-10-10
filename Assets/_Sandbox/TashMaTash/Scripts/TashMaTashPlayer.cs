using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PummelPartyClone
{
    public class TashMaTashPlayer : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private PlayerController _playerController;
        private bool _isRagdolled;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerController = GetComponent<PlayerController>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isRagdolled) return;
            if (collision.gameObject.CompareTag("Dangerous"))
            {
                Ragdoll(collision);
                PlayerController player = GetComponent<PlayerController>();
                TashMaTashGameManager.Instance.PlayerHit(player);
            }
        }
        
        private void Ragdoll(Collision collision)
        {
            Rigidbody projectileRb = collision.collider.GetComponent<Rigidbody>();
            if (projectileRb == null) return;
            Vector3 directionAwayFromProjectile = (transform.position - collision.transform.position).normalized;
            
            gameObject.layer = 8; // Set to projectile layer to ignore invisible walls
            _isRagdolled = true;
            _playerController.enabled = false;
            _rigidbody.constraints = RigidbodyConstraints.None; //Send them flying
            _rigidbody.AddForce(directionAwayFromProjectile * 1000 + Vector3.up * 250, ForceMode.Impulse);
        }

    }
}
