using UnityEngine;

public class Interactable : MonoBehaviour
{
	public float Radius = 2f;

	private PlayerController _playerController;

	private void Start() {
		SphereCollider collider = gameObject.AddComponent<SphereCollider>();
		collider.isTrigger = true;
		collider.radius = Radius;

		_playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
	}

	public virtual void Interact() {
		Debug.Log("Interacting with " + transform.name);
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			_playerController.Interactable = this;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) {
			_playerController.Interactable = null;
		}
	}

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, Radius);
	}
}
