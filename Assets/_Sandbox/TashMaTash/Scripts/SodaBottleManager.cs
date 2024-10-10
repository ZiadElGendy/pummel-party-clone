using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PummelPartyClone
{
    public class SodaBottleManager : Singleton<SodaBottleManager>
    {
        [SerializeField] private float _spawnRadius = 25f;
        [SerializeField] private SodaBottle _sodaBottlePrefab;
        [SerializeField] private float _initialSpawnInterval = 5f;
        [SerializeField] private float _spawnIntervalDecreaseRate = 0.1f;
        [SerializeField] private float _minSpawnInterval = 0.1f;
        [SerializeField] private float _intensity = 1f;
        [SerializeField] private float _intensityIncreaseRate = 0.1f;

        private float _currentSpawnInterval;
        private float _spawnTimer;
        
        private void Start()
        {
            _currentSpawnInterval = _initialSpawnInterval;
            _spawnTimer = _currentSpawnInterval;
        }

        private void SpawnSodaBottle()
        {
            // Create a 3D spawn position along the circumference of the circle
            float angle = Random.Range(0f, 360f);
            float x = _spawnRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = _spawnRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 spawnPosition3D = new Vector3(x, transform.position.y, z);
            Quaternion spawnRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    
            // Instantiate the soda bottle at the calculated position with the specified rotation
            SodaBottle sodaBottle = Instantiate(_sodaBottlePrefab, spawnPosition3D, spawnRotation);
            sodaBottle.IncreaseIntensity(_intensity);
        }


        public void Update()
        {
            _spawnTimer -= Time.deltaTime;
            _intensity += _intensityIncreaseRate * Time.deltaTime;
            if (_spawnTimer <= 0)
            {
                SpawnSodaBottle();
                _currentSpawnInterval = Mathf.Max(_minSpawnInterval, _currentSpawnInterval - _spawnIntervalDecreaseRate);
                _spawnTimer = _currentSpawnInterval;
            }
        }
    }
}