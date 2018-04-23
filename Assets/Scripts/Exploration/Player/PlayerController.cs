using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float Speed = 4f;
    public float GroundCheckDistance = 0.1f;
    public Interactable Interactable;
    public GameObject InteractablePanelGui;

    private Animator _animator;
    private CharacterController _characterController;

    private bool _isGrounded;
    private float _turnAmount;
    private float _forwardAmount;
    private Vector3 _groundNormal;

    void Start() {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    public void Move(Vector3 move, bool interactButtonDown) {
        if (Interactable != null) {
            Text interactableText = InteractablePanelGui.transform.Find("Text").GetComponent<Text>();
            interactableText.text = "Press <b><color=#C95641>E</color></b> To Talk To <b><color=#C95641>" + Interactable.name + "</color></b>";
            InteractablePanelGui.SetActive(true);
            if (interactButtonDown) {
                Interactable.Interact();
            }
        } else {
            InteractablePanelGui.SetActive(false);
        }

        // Convert the world relative move vector into a local-relative turn amount and forward amount required to head in the desired direction
        if (move.magnitude > 1f) move.Normalize();
        Vector3 localMove = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, _groundNormal);
        _turnAmount = Mathf.Atan2(localMove.x, localMove.z);
        _forwardAmount = localMove.z;

        ApplyTurnRotation();

        // Apply gravity on each frame to keep player grouned
        _characterController.Move((move * Speed + Physics.gravity) * Time.deltaTime);

        // Send input and other state parameters to the animator
        UpdateAnimator();
    }

    void UpdateAnimator() {
        // update the animator parameters
        _animator.SetFloat("Forward", _forwardAmount, 0.1f, Time.deltaTime);
        _animator.SetFloat("Turn", _turnAmount, 0.1f, Time.deltaTime);
        _animator.SetBool("OnGround", _isGrounded);
    }

    void ApplyTurnRotation() {
        transform.Rotate(0, _turnAmount * (2 * Mathf.PI), 0);
    }

    void CheckGroundStatus() {
        RaycastHit hitInfo;

        float offsetDistance = Vector3.Distance(transform.position, transform.position + (Vector3.up * 0.1f));
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, offsetDistance + GroundCheckDistance)) {
            _groundNormal = hitInfo.normal;
            _isGrounded = true;
        } else {
            _isGrounded = false;
            _groundNormal = Vector3.up;
        }
    }
}
