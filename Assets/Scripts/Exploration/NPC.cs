public class NPC : Interactable
{
    public Dialogue Dialogue;
    public Monster[] EnemyMonsters;

    public override void Interact() {
        FindObjectOfType<DialogueManager>().StartDialogue(Dialogue, this);
    }

    public void OnEndDialogue() {        
        if (EnemyMonsters.Length == 0) {
            return;
        }
        FindObjectOfType<CombatManager>().StartBattle(EnemyMonsters);
    }
}
