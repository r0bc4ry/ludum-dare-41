using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


[RequireComponent(typeof(PlayerController))]
public class PlayerInputManager : MonoBehaviour
{
    private PlayerController _playerController; // A reference to the PlayerController on the object
    private Transform _camera; // A reference to the main camera in the scenes transform
    private Vector3 _cameraForward; // The current forward direction of the camera
    private Vector3 _move; // The world-relative desired move direction, calculated from the _cameraForward and user input

    private void Start() {
        // get the transform of the main camera
        if (Camera.main != null) {
            _camera = Camera.main.transform;
        } else {
            Debug.LogWarning("Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // We use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        // Get the third person character (this should never be null due to require component)
        _playerController = GetComponent<PlayerController>();
    }

    private void Update() {
        // Read inputs
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");

        // Calculate move direction to pass to character
        if (_camera != null) {
            // Calculate camera relative direction to move:
            _cameraForward = Vector3.Scale(_camera.forward, new Vector3(1, 0, 1)).normalized;
            _move = v * _cameraForward + h * _camera.right;
        } else {
            // We use world-relative directions in the case of no main camera
            _move = v * Vector3.forward + h * Vector3.right;
        }

        bool interact = Input.GetKeyDown(KeyCode.E);

        // Pass all parameters to the PlayerController script
        _playerController.Move(_move, interact);
    }
}
