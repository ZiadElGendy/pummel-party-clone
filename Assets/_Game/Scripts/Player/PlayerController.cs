
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.DemiLib;
// UnityEngine
using UnityEngine;
using UnityEngine.InputSystem;
// Game
using Sandbox.TileBehaviourDemo.Scripts;
using UnityEngine.Serialization;

// Third Party


namespace PummelPartyClone
{
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        // Player ID
        private int _playerId;
        public int PlayerId { get; set; }

        // Instances
        private TurnManager _turnManager => TurnManager.Instance;
        private BoardManager _boardManager => BoardManager.Instance;

        // Components
        private Rigidbody _rigidBody;
        private CapsuleCollider _capsuleCollider;
        private LayerMask _groundLayer;
        public Inventory Inventory;

        private int _health;

        public int Health
        {
            get => _health;
            set
            {
                _health = Mathf.Clamp(value, 0, 100);
                if (_health == 0)
                {
                    OnPlayerDeath.Raise(_playerId);
                    Died();
                }
            }
        }
        public GameEvent OnInventoryUpdate;
        public IntGameEvent OnPlayerDeath;

        // Player movement variables
        [SerializeField] private float _jumpDistance = 5f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _groundCheckDistance = 0.1f;
        [SerializeField] private float _rotationSpeed = 0.6f;
        [SerializeField] private float _stepHeight = 0.3f;
        private Vector2 _moveInput;
        public bool IsMovingToPosition = false;
        private Vector3 _targetPosition;
        private TaskCompletionSource<bool> _moveCompletionSource;

        // Player Input Variables
        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _cameraTransitionAction;
        private InputAction _cameraMoveAction;

        // Placeholder, will be replaced a trigger on switching to minigame
        [SerializeField] private InputAction _switchActionMap = new InputAction("SwitchActionMap", InputActionType.Button, "<Keyboard>/p");

        // Events

        // Event for camera transition
        public delegate void CameraTransitionEvent();
        public static event CameraTransitionEvent OnCameraTransition;

        // Event for player animation state change
        public delegate void AnimationStateChangeEvent(PlayerAnimationState newState);
        public static event AnimationStateChangeEvent OnPlayerStateChanged;
        #endregion

        #region Unity Methods
        void Awake()
        {
            // Get the components
            _rigidBody = GetComponent<Rigidbody>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _playerInput = GetComponent<PlayerInput>();

            // Set the ground layer
            _groundLayer = LayerMask.GetMask("Ground");

            // Get the input actions
            _moveAction = _playerInput.actions["Move"];
            _jumpAction = _playerInput.actions["Jump"];
            _cameraTransitionAction = _playerInput.actions["CameraTransition"];
            _cameraMoveAction = _playerInput.actions["CameraMove"];

            // Initialize variables
            Inventory = new Inventory(20, new List<Item>(), OnInventoryUpdate);
            Health = 100;
        }

        void OnEnable()
        {
            // Subscribe to the input actions
            _jumpAction.performed += OnJump;
            _moveAction.performed += OnMove;
            _moveAction.canceled += OnMoveCanceled;
            _cameraTransitionAction.performed += OnCameraTransitionAction;

            _switchActionMap.Enable(); // Enable the switch action map
            _switchActionMap.performed += OnSwitchActionMap; // Subscribe to the switch action map
        }

        private void OnDisable()
        {
            // Unsubscribe from the input actions
            _jumpAction.performed -= OnJump;
            _moveAction.performed -= OnMove;
            _moveAction.canceled -= OnMoveCanceled;
            _cameraTransitionAction.performed -= OnCameraTransitionAction;

            _switchActionMap.Disable(); // Disable the switch action map
            _switchActionMap.performed -= OnSwitchActionMap; // Unsubscribe from the switch action map
        }

        void Start()
        {
            // Set the player state to idle
            SetAnimationState(PlayerAnimationState.Idle);
        }

        void FixedUpdate()
        {
            HandleMovement();
        }

        #endregion
        
        #region Movement Methods
        
        private void HandleMovement()
        {
            // If the player is moving to a position
            if (IsMovingToPosition)
            {
                Move(GetMoveForPosition());
            }
            else
            {
                Move(_moveInput);
            }
        }
        
        private void Move(Vector2 input)
        {
            if (input.magnitude > 0.1f) // If the player is moving
            {
                // Rotate the player only if input is significant
                RotateTowardsMovement(input);

                // Move the player using AddForce
                SetAnimationState(PlayerAnimationState.Running);
                Vector3 moveDirection = new Vector3(input.x, 0, input.y).normalized; // Get the move direction and normalize it

                // Apply force to the Rigidbody to move the player
                _rigidBody.AddForce(moveDirection * _moveSpeed, ForceMode.Force); // Use AddForce for physics-based movement
                HandleSteps();
            }
            else
            {
                _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0); // Stop horizontal movement
                SetAnimationState(PlayerAnimationState.Idle); // Set the animation state to idle when not moving
            }
        }

        private void RotateTowardsMovement(Vector2 input)
        {
            // Only rotate if there's significant input, to prevent getting stuck when input is very small
            if (input.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg; // Get rotation angle in degrees
                Quaternion rotationQuaternion = Quaternion.Euler(0, angle, 0); // Create a quaternion from the angle
                transform.rotation = Quaternion.Slerp(transform.rotation, rotationQuaternion, _rotationSpeed * Time.deltaTime); // Use Slerp to rotate the player smoothly 
            }
        }
        
        private void HandleSteps()
        {
            // Cast a ray slightly in front of the player to detect steps or obstacles
            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.forward * 0.5f, Vector3.down, out hit, _stepHeight + 0.1f))
            {
                // Check if the hit point is within the step distance and below the step height
                if (hit.distance < _stepHeight)
                {
                    // Calculate the step correction
                    Vector3 stepCorrection = new Vector3(0, _stepHeight - hit.distance, 0);
                    _rigidBody.position += stepCorrection; // Move the player up to handle the step

                    // Optionally, add a small force to keep moving forward smoothly
                    _rigidBody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
                }
            }
        }

        
        public Task MoveToPosition(Vector3 targetPosition)
        {
            // Reset the TaskCompletionSource
            _moveCompletionSource = new TaskCompletionSource<bool>();

            // Set the movement flag and target position
            IsMovingToPosition = true;
            _targetPosition = targetPosition;

            // Return the task, so other parts of your code can await it
            return _moveCompletionSource.Task;
        }

        private Vector2 GetMoveForPosition()
        {

            Vector3 moveDirection = _targetPosition - transform.position; // Get the move direction
            moveDirection.y = 0; // Ignore the y axis
            
            if (Vector3.Distance(transform.position, _targetPosition) > 1.0f)
            {
                // Generate input for the Move method
                return new Vector2(moveDirection.x, moveDirection.z);
            }
            else
            {
                IsMovingToPosition = false;
                _rigidBody.velocity = Vector3.zero;
                _moveCompletionSource.TrySetResult(true); // Complete the task to signal that movement is done
                return Vector2.zero;
            }
        }
        
        private bool IsGrounded()
        {
            // using a raycast to check if the player is grounded
            return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, _groundLayer);
        }
        
        private void Jump()
        {
            SetAnimationState(PlayerAnimationState.Jumping);
            // _rigidBody.AddForce(Vector3.up * _jumpDistance, ForceMode.Impulse); // We don't need to actually jump, we just need to play the animation
        }
        
        // Called by input system when player presses jump button
        void OnJump(InputAction.CallbackContext context)
        {
            if (IsGrounded())
            {
                Jump();
            }
        }

        // WaSD Move for testing purposes
        void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        // Called by input system when player releases move button
        void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        // Called by input system when player presses camera transition button
        void OnCameraTransitionAction(InputAction.CallbackContext context)
        {
            OnCameraTransition?.Invoke();
        }

        // Switch between board and minigame action maps
        // ONLY FOR TESTING AND REFERENCE PURPOSES
        void OnSwitchActionMap(InputAction.CallbackContext context)
        {
            if (_playerInput.currentActionMap.name == "Board") // If the current action map is board, switch to minigame
            {
                _playerInput.SwitchCurrentActionMap("MiniGame"); // Switch to minigame action map
            }
            else // If the current action map is minigame, switch to board
            {
                _playerInput.SwitchCurrentActionMap("Board"); // Switch to board action map
            }
        }
        #endregion

        #region Animation Methods

        private void SetAnimationState(PlayerAnimationState newState)
        {
            OnPlayerStateChanged?.Invoke(newState);
        }
        #endregion

        #region Game Methods
        public int RollDice()
        {
            int diceRoll = UnityEngine.Random.Range(1, 7);
            return diceRoll;
        }

        public void UseItem(Item item)
        {
            //TODO: Move this logic to UI
            if (_turnManager.IsUsedItem)
            {
                Debug.Log("Item already used this turn");
                return;
            }
            item.OnUse(this);
            Inventory.RemoveItem(item);
            //TODO: Replace this with event
            _turnManager.IsUsedItem = true;
        }
        
        public void Heal(int amount)
        {
            Health += amount;
        }
        
        private void CoinCollected()
        {
            SetAnimationState(PlayerAnimationState.Happy);
        }

        private void Hit()
        {
            SetAnimationState(PlayerAnimationState.Hit);
        }

        private void Died()
        {
            Debug.Log("Player Died");
            Inventory.AddSubCoins(-10);
            Health = 50;
        }

        #endregion

        private void OnDrawGizmos()
        {
            // // Draw point at player position
            // Gizmos.color = Color.blue;
            // Gizmos.DrawSphere(transform.position, 0.1f);

            // // Draw raycast for ground check
            // Gizmos.color = Color.red;
            // Gizmos.DrawRay(transform.position, Vector3.down * _groundCheckDistance);
        }
    }
}
