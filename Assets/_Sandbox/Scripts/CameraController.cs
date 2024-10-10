using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PummelPartyClone
{
    public class CameraController : MonoBehaviour
{
    // Components
    private Transform _cameraTransform; // The camera object
    private Transform _groundTransform; // The ground object
    private Transform _targetTransform; // The target the camera is looking at
    private Renderer _groundRenderer; // The ground renderer

    // Camera variables
    private float _cameraDistance = 3f;
    private float _cameraHeight = 10f;
    private Vector3 _cameraPosition;
    private Quaternion _cameraRotation;

    // Camera transition variables
    private bool _isTransitioning = false;
    private bool _isFollowingPlayer = false;
    [SerializeField] private float _smoothTransitionDuration = 0.2f;


    // default camera position when viewing the board
    private Vector3 _boardViewPosition;
    private Quaternion _boardViewRotation;

    // Player objects
    private GameObject[] _players;

    void OnEnable()
    {
        PlayerController.OnCameraTransition += OnCameraTransition;
    }

    void OnDisable()
    {
        PlayerController.OnCameraTransition -= OnCameraTransition;
    }

    void Start()
    {
        // Find all player objects
        _players = GameObject.FindGameObjectsWithTag("Player"); // placeholder till turn system is implemented
        _targetTransform = _players[0].transform; // Find the player object

        _cameraTransform = GetComponent<Transform>(); // Get the main camera's transform

        _groundTransform = GameObject.Find("Ground").transform; // Find the ground object
        _groundRenderer = _groundTransform.GetComponent<Renderer>(); // Get the ground renderer


        // Set the camera position and rotation to default values
        _cameraPosition = new Vector3(0, _cameraHeight, _cameraDistance);
        _cameraRotation = Quaternion.Euler(45, 0, 0);

        if (_groundRenderer != null)
        {
            // Get the ground bounds
            Bounds groundBounds = _groundRenderer.bounds;
            Vector3 groundCenter = groundBounds.center;
            Vector3 groundSize = groundBounds.size;

            // Position camera at the edge of the ground
            _cameraPosition = new Vector3(
                groundCenter.x, // X position is the center of the ground
                groundCenter.y + _cameraHeight,  // Y position is the height above the ground
                groundCenter.z - (groundSize.z / 2 + _cameraDistance) // Z position is the back of the ground plus the camera distance 
            );


        }
        else
        {
            Debug.LogError("Ground renderer not found");
        }

        _boardViewPosition = _cameraPosition; // Set the board view position
        _boardViewRotation = _cameraRotation; // Set the board view rotation

        _cameraTransform.SetPositionAndRotation(_cameraPosition, _cameraRotation); // Set the camera position and rotation
    }

    // LateUpdate is called after Update each frame 
    void LateUpdate()
    {
        if (_isFollowingPlayer && !_isTransitioning) // If we are following the player and not transitioning
        {
            // Follow the player
            _cameraPosition = _targetTransform.position + new Vector3(0, _cameraHeight, -_cameraDistance);
            _cameraTransform.position = _cameraPosition;
            _cameraTransform.LookAt(_targetTransform);
        }
    }

    void OnCameraTransition()
    {
        ToggleCameraTransition();
    }

    // TODO: Will figure out how to implement Zoom using scroll wheel later
    // void OnZoom(InputValue value)
    // {
    //     float zoomAmount = value.Get<Vector2>().y;

    //     // Adjust the camera distance and height
    //     _cameraDistance += zoomAmount;
    //     _cameraHeight += zoomAmount;

    //     // Clamp the values to ensure they stay within a reasonable range
    //     _cameraDistance = Mathf.Clamp(_cameraDistance, 1f, 10f);
    //     _cameraHeight = Mathf.Clamp(_cameraHeight, 1f, 10f);
    // }

    void OnNextPlayer()
    {
        // placeholder till turn system is implemented
        int currentPlayerIndex = System.Array.IndexOf(_players, _targetTransform.gameObject);
        int nextPlayerIndex = (currentPlayerIndex + 1) % _players.Length;
        _targetTransform = _players[nextPlayerIndex].transform;
    }

    // Toggle between following the player and viewing the board
    async void ToggleCameraTransition()
    {
        _isTransitioning = true; // Set transitioning to true so we don't interrupt the transition
        if (_isFollowingPlayer) // If we are following the player
        {
            await SmoothCameraTransition( // Transition to board view
                _cameraTransform.position, _cameraTransform.rotation, // Initial position and rotation
                _boardViewPosition, _boardViewRotation, // Target position and rotation
                _smoothTransitionDuration // Duration of the transition
            );
        }
        else // If we are viewing the board
        {
            Vector3 targetPosition = _targetTransform.position + new Vector3(0, _cameraHeight, -_cameraDistance); // Calculate the target position
            Quaternion targetRotation = Quaternion.LookRotation(_targetTransform.position - targetPosition); // Calculate rotation needed to look at the target
            await SmoothCameraTransition( // Transition to player view
                _cameraTransform.position, _cameraTransform.rotation, // Initial position and rotation
                targetPosition, targetRotation, // Target position and rotation
                _smoothTransitionDuration // Duration of the transition
            );
        }
        _isFollowingPlayer = !_isFollowingPlayer;
        _isTransitioning = false;
    }

    // Smoothly transition the camera from the initial position and rotation to the target position and rotation over a specified duration
    private async Task SmoothCameraTransition(Vector3 initialPosition, Quaternion initialRotation, Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0; // Elapsed time since the start of the transition

        while (elapsedTime < duration) // While the transition is not complete
        {
            _cameraTransform.SetPositionAndRotation( // Lerp the position and rotation of the camera
                Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration), // Lerp because linear movement
                Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration) // Slerp because rotation
            );
            elapsedTime += Time.deltaTime; // Increment the elapsed time
            await Task.Yield(); // Wait for the next frame
        }
    }
}
}
