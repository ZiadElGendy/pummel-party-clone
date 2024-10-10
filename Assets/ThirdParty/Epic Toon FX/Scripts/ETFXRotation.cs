using System.Collections;
using TMPro;
using UnityEngine;

namespace EpicToonFX
{
    public class ETFXRotation : MonoBehaviour
    {
 
        [Header("Rotate axises by degrees per second")]
        [SerializeField] private Vector3 _rotateVector = Vector3.zero;
        [SerializeField] private int _minRotationValue = 200;
        [SerializeField] private int _maxRotationValue = 500;

        private Camera _camera;
        public enum spaceEnum { Local, World };
        public spaceEnum rotateSpace;

        private bool _isRotating = false;
        private Dice _dice;

        private bool _canStopDice = false; //Gives and removes the ability for the player to stop the dice

        private float _timer = 0;           //Used to rerandomize the rotation of the dice every _timeFinalVlue seconds
        private int _timerFinalValue = 4;

        void Start()
        {
            _dice = GetComponentInParent<Dice>();
            _camera = Camera.main;
            StartCoroutine(Initialize());
        }

        // Update is called once per frame
        void Update()
        {
            if(_isRotating)
            {
                _timer += Time.deltaTime;
                if(_timer >= _timerFinalValue)
                {
                    _timer = 0;
                    _timerFinalValue = 3;
                    if(_minRotationValue + 50 <= _maxRotationValue)
                        _minRotationValue += 50;
                    RandomizeRotation();
                }

                if (rotateSpace == spaceEnum.Local)
                    transform.Rotate(_rotateVector * Time.deltaTime);
                if (rotateSpace == spaceEnum.World)
                    transform.Rotate(_rotateVector * Time.deltaTime, Space.World);
            }

            if(Input.GetKeyDown(KeyCode.Space) && _canStopDice)
            {
                _canStopDice = false;
                _isRotating = false;
                _dice.AssignVectors(); // <------------ The value of the side is returned here (for later use)
            }
        }

        private IEnumerator Initialize()
        {
            yield return new WaitForSeconds(1);
            _camera.transform.position = _dice.transform.position - new Vector3(0, 0, 2);
            yield return new WaitForSeconds(1);
            RandomizeRotation();
            _isRotating = true;
            yield return new WaitForSeconds(1);
            _canStopDice = true;

            
        }

        private void RandomizeRotation()
        {
            int x, y, z;
            do
            {
                x = Random.Range(-_maxRotationValue, _maxRotationValue);
            } while (x < _minRotationValue && x > -_minRotationValue);

            do
            {
                y = Random.Range(-_maxRotationValue, _maxRotationValue);
            } while (y < _minRotationValue && y > -_minRotationValue);

            do
            {
                z = Random.Range(-_maxRotationValue, _maxRotationValue);
            } while (z < _minRotationValue && z > -_minRotationValue);

            _rotateVector = new Vector3(x, y, z);
        }
    }
}