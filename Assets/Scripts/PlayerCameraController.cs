using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
	public GameObject Player;
	
	// TODO Get offset programmatically on Start()

	void LateUpdate () {
		transform.position = new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z - 1.5f);
	}
}
