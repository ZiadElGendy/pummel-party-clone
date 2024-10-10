using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace PummelPartyClone
{
    public class TashMaTashAI : MonoBehaviour
    {
        private PlayerController _playerController;

        [SerializeField] private float moveRadius; // Radius for random points
        [SerializeField] private float moveRadiusRandomization; // Randomization for radius
        [SerializeField] private float delayBetweenMoves; // Delay between each move
        [SerializeField] private float delayBetweenMovesRandomization; // Randomization for delay between moves
        [SerializeField] private float staticProjectileAvoidanceRadius = 5f; // Small radius for static projectile detection
        [SerializeField] private float movingProjectileAvoidanceRadius = 10f; // Large radius for moving projectile detection
        [SerializeField] private LayerMask projectileLayer; // Set layer for projectiles (optional, if you want to optimize raycast performance)
        [SerializeField] private float detectionInterval = 0.5f; // Interval between each detection

        public bool _isWaiting = false;
        public bool _isMoving = false;

        void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        void Start()
        {
            InvokeRepeating("AvoidSodaBottles", 0f, detectionInterval);
        }


        private void AvoidSodaBottles()
        {
            Collider projectile = DetectNearbyProjectile();
            if (projectile != null)
            {
                Debug.Log("Moved to avoid projectile");
                MoveImmediately(projectile);
            }
        }


        private void Update()
        {
            if (_isWaiting) { return; }

            Vector3 randomDestination = GetRandomNavMeshLocation();
            AttemptMove(randomDestination);
        }

        private void AttemptMove(Vector3 destination)
        {
            if (_isMoving)
            {
                _isMoving = _playerController.IsMovingToPosition;
            }
            else
            {
                Debug.Log("Moved naturally");
                _playerController.MoveToPosition(destination);
                _isMoving = true;
                StartCoroutine(WaitBeforeMovingAgain());
            }
        }

        private IEnumerator WaitBeforeMovingAgain()
        {
            _isWaiting = true;
            float randomizedDelay = delayBetweenMoves + Random.Range(-delayBetweenMovesRandomization, delayBetweenMovesRandomization);
            yield return new WaitForSeconds(randomizedDelay);
            _isWaiting = false;
        }

        Vector3 GetRandomNavMeshLocation()
        {
            float radius = moveRadius + Random.Range(-moveRadiusRandomization, moveRadiusRandomization);
            Vector3 randomDirection = Random.insideUnitSphere * radius; // Get a random point inside a sphere
            randomDirection += transform.position; // Offset it by the AI's position

            // Check if there's a wall in the random direction using a raycast
            RaycastHit wallHit;
            if (Physics.Raycast(transform.position, randomDirection, out wallHit, 2f))
            {
                // If the ray hits a wall, reverse the direction
                randomDirection = -randomDirection;
            }

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
            {
                return hit.position; // Return the valid NavMesh position
            }

            return Vector3.zero; // Return zero if no valid position found
        }

        public void MoveImmediately(Collider projectile)
        {
            var destination = GetAvoidanceNavMeshLocation(projectile);

            // Move the AI to the calculated position
            _isWaiting = false;
            _isMoving = false;
            AttemptMove(destination);
        }

        private Vector3 GetAvoidanceNavMeshLocation(Collider projectile)
        {
            // Get the direction from the projectile to the player
            Vector3 directionAwayFromProjectile = (transform.position - projectile.transform.position).normalized;

            // Choose whether to move left or right, relative to the direction away from the projectile
            Vector3 moveDirection;
            if (Random.value > 0.5f)
            {
                // Move left relative to the direction away from the projectile
                moveDirection = Vector3.Cross(directionAwayFromProjectile, Vector3.up).normalized;
            }
            else
            {
                // Move right relative to the direction away from the projectile
                moveDirection = Vector3.Cross(Vector3.up, directionAwayFromProjectile).normalized;
            }

            // Add some distance to move away from the projectile
            Vector3 destination = transform.position + moveDirection * moveRadius;
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destination, out hit, moveRadius, NavMesh.AllAreas))
            {
                return hit.position; // Return the valid NavMesh position
            }
            
            return Vector3.zero;
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Dangerous"))
            {
                enabled = false;
            }
            if (collision.gameObject.CompareTag("Player"))
            {
                MoveImmediately(collision.collider);
            }
        }

        private Collider DetectNearbyProjectile()
        {
            Collider[] projectiles = Physics.OverlapSphere(transform.position, movingProjectileAvoidanceRadius, projectileLayer);
            foreach (var projectile in projectiles)
            {
                if (projectile.gameObject.layer == LayerMask.NameToLayer("Projectile"))
                {
                    float distance = Vector3.Distance(transform.position, projectile.transform.position);
                    if (distance <= staticProjectileAvoidanceRadius)
                    {
                        return projectile;
                    }

                    Rigidbody rb = projectile.GetComponent<Rigidbody>();
                    if (rb != null && rb.velocity.magnitude > 0.1f)
                    {
                        return projectile;
                    }
                }
            }

            return null; // No projectile detected
        }

    }
}
