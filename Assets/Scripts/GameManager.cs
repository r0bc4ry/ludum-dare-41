using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool HasNoMonsters;
    public bool HasAllMonsters;

    private InventoryManager _inventoryManager;

    void Start() {
        _inventoryManager = FindObjectOfType<InventoryManager>();
    }

    void Update() {
        HasNoMonsters = _inventoryManager.Monsters.Count == 0;
        HasAllMonsters = _inventoryManager.Monsters.Count == 7;
    }
}
