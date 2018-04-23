using UnityEngine;

public class NPC : Interactable
{
    public Dialogue BeforeDialogue;
    public Dialogue AfterDialogue;
    public Monster[] EnemyMonsters;

    public override void Interact() {
        FindObjectOfType<DialogueManager>().StartDialogue(BeforeDialogue, this);
    }

    public void OnEndDialogue() {
        if (name == "Professor Mahogany" && FindObjectOfType<GameManager>().HasAllMonsters) {
            Application.LoadLevel("Credits");
        }
        
        if (EnemyMonsters.Length == 0) {
            return;
        }

        FindObjectOfType<CombatManager>().StartBattle(this);
    }

    public void OnEndBattle() {
        FindObjectOfType<DialogueManager>().StartDialogue(AfterDialogue, this);

        Thief thief = GetComponent<Thief>();
        if (thief != null) {
            thief.IsDefeated = true;
        }
    }
}
