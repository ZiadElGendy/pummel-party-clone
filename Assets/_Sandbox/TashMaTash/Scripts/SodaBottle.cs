using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace PummelPartyClone
{
    public enum SodaBottleState
    {
        WindingUp,
        Suspense,
        Launched
    }
    public class SodaBottle : MonoBehaviour
    {
        #region Public Variables
        public SodaBottleState State = SodaBottleState.WindingUp;
        public bool InUse;
        #endregion

        #region Private Variables
        private Rigidbody _rigidbody;
        private Vector3 _originalPosition;
        private NavMeshObstacle _navMeshObstacle;
        private float _rotationVelocity = 0f;
        private bool _raycastTimeOut = false;
        private ParticleSystem _particleSystem;
        #endregion

        #region Serialized Fields
        [SerializeField] private Preset _particleFoamPreset;
        [SerializeField] private Preset _particleColaPreset;
        [SerializeField] private Material _transparentMaterial;
        [SerializeField] private bool _willLaunch;
        [SerializeField] private float _timer;
        [SerializeField] private float _maxRotationVelocity;
        [SerializeField] private float _rotationAcceleration;
        [SerializeField] private float _launchForce;
        [SerializeField] private float _accelerationForce;
        [SerializeField] private float _suspenseTime;
        #endregion

        #region Constants
        private const float SHAKE_INTENSITY = 0.001f;
        private const float DESTROY_DELAY = 5f;
        private const float FADE_DURATION = 2f;
        private const float FAKE_LAUNCH_FORCE = 500f;
        private const float FAKE_LAUNCH_TORQUE = 100f;
        #endregion

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
            _particleSystem = GetComponent<ParticleSystem>();
            _originalPosition = transform.position;
        }

        private void Start()
        {
            //Randomize activation status
            float randomSeed = UnityEngine.Random.Range(-50, 50);
            _willLaunch = randomSeed < 0f;

            //Randomize how potent the bottle is
            //Different seed are used so players can't guess how it will launch
            randomSeed = UnityEngine.Random.Range(-50, 50);
            _timer = _timer + 2f * randomSeed;

            randomSeed = UnityEngine.Random.Range(-50, 50);
            _maxRotationVelocity = _maxRotationVelocity + 2f * randomSeed;
            _rotationAcceleration = _rotationAcceleration + 0.1f * randomSeed;
            _launchForce = _launchForce + 10f * randomSeed;

            randomSeed = UnityEngine.Random.Range(-50, 50);
            _suspenseTime = _suspenseTime + 0.01f * randomSeed;
        }

        public void IncreaseIntensity(float intensity)
        {
            _maxRotationVelocity += intensity;
            _launchForce += intensity * 10f;
        }

        void FixedUpdate()
        {
            switch (State)
            {
                case SodaBottleState.WindingUp:
                    WindUp();
                    break;
                case SodaBottleState.Launched:
                    AccelerateProjectile();
                    break;
            }
        }

        private void AccelerateProjectile()
        {
            _rigidbody.AddForce(transform.forward * _accelerationForce);
        }
        
        private void WindUp()
        {
            // Increase rotation speed exponentially
            _rotationVelocity += _rotationAcceleration * _rotationAcceleration * Time.fixedDeltaTime;
            _rotationVelocity = Mathf.Min(_rotationVelocity, _maxRotationVelocity);

            // Shake the bottle
            Shake();

            // Rotate the bottle
            transform.Rotate(Vector3.up, _rotationVelocity * Time.fixedDeltaTime);

            _timer--;
            if (_timer <= 0)
            {
                LaunchAtPlayer();
            }
        }

        private void LaunchAtPlayer()
        {
            // Check if the bottle is facing a player before launching
            RaycastHit hit;
            
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                Collider collider = hit.collider;
                TashMaTashPlayer player = collider.GetComponent<TashMaTashPlayer>();

                //Launch if player is detected
                if (player)
                {
                    State = SodaBottleState.Suspense;
                    StartCoroutine(AttemptLaunch());
                }
            }
        }

        private void Shake()
        {
            float shakeIntensity = SHAKE_INTENSITY * _rotationVelocity;
            Vector3 shakeOffset = new Vector3(
                UnityEngine.Random.Range(-shakeIntensity, shakeIntensity),
                0,
                UnityEngine.Random.Range(-shakeIntensity, shakeIntensity)
            );
            transform.position = _originalPosition + shakeOffset;
        }

        private IEnumerator AttemptLaunch()
        {
            // Make path of bottle dangerous to AI
            _navMeshObstacle.carving = true;

            //BUG: Particle system not working, and seems to slow game down
            //Activate particle system
            //_particleFoamPreset.ApplyTo(_particleSystem);

            // Pauses for suspense before launching
            yield return new WaitForSeconds(_suspenseTime);

            // Remove position constraints
            _rigidbody.constraints = RigidbodyConstraints.None;

            if (_willLaunch)
            {
                TrueLaunch();
            }
            else
            {
                FakeLaunch();
            }

        }

        private void TrueLaunch()
        {
            //BUG: Particle system not working, and seems to slow game down
            //_particleColaPreset.ApplyTo(_particleSystem);

            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _rigidbody.AddForce(transform.forward * _launchForce);
            State = SodaBottleState.Launched;
            gameObject.tag = "Dangerous";

            StartCoroutine(DestroyAfterTime(DESTROY_DELAY));
        }

        private void FakeLaunch()
        {
            // Make path of bottle no longer dangerous to AI
            _navMeshObstacle.carving = false;

            // Make the bottle flip pathetically in the air
            _rigidbody.constraints = RigidbodyConstraints.None;
            _rigidbody.useGravity = true;

            // Apply a vertical force to make the bottle go up
            _rigidbody.AddForce(Vector3.up * FAKE_LAUNCH_FORCE);

            // Apply a random torque to make the bottle rotate in the air
            Vector3 randomTorque = new Vector3(
                UnityEngine.Random.Range(-0.5f, 0.5f),
                UnityEngine.Random.Range(-0.5f, 0.5f),
                UnityEngine.Random.Range(-0.5f, 0.5f)
            ) * FAKE_LAUNCH_TORQUE;
            _rigidbody.AddTorque(randomTorque);

            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = _transparentMaterial;
            Color color = renderer.material.color;
            float fadeSpeed = 1f / FADE_DURATION;

            for (float t = 0; t < 1; t += Time.deltaTime * fadeSpeed)
            {
                color.a = Mathf.Lerp(1, 0, t);
                renderer.material.color = color;
                yield return null;
            }

            color.a = 0;
            renderer.material.color = color;

            //TODO: Pool the object instead of destroying it
            Destroy(gameObject);
        }

        private IEnumerator DestroyAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            //TODO: Pool the object instead of destroying it
            Destroy(gameObject);
        }
    }
}
