using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<Item> Items = new List<Item>(); // TODO Add items?
    public List<Monster> Monsters = new List<Monster>();    
    public Monster DefaultMonster;

    private void Start() {
        Monsters.Add(DefaultMonster);
    }
}
