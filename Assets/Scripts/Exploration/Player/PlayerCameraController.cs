using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public GameObject Player;

    // TODO Get offset programmatically on Start()
    private Vector3 cameraOffset;

    private void Start() {
        cameraOffset = transform.localPosition;
    }

    void LateUpdate() {
        transform.position = new Vector3(Player.transform.position.x + cameraOffset.x, Player.transform.position.y + cameraOffset.y, Player.transform.position.z + cameraOffset.z);
    }
}
