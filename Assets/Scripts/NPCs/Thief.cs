using UnityEngine;

public class Thief : MonoBehaviour
{
    public Monster DefaultMonster;
    public string[] UndefeatedBeforeSentences;
    public string[] UndefeatedAfterSentences;
    public string[] DefeatedBeforeSentences;
    public string Name;
    public bool IsDefeated;

    private NPC _npc;

    void Start() {
        _npc = GetComponent<NPC>();
        _npc.BeforeDialogue.Name = Name;
        _npc.AfterDialogue.Name = Name;
    }

    void Update() {
        if (IsDefeated) {
            _npc.BeforeDialogue.Sentences = DefeatedBeforeSentences;
            _npc.AfterDialogue.Sentences = new string[] { };
            _npc.EnemyMonsters = new Monster[] { };
        } else {
            _npc.BeforeDialogue.Sentences = UndefeatedBeforeSentences;
            _npc.AfterDialogue.Sentences = UndefeatedAfterSentences;
            _npc.EnemyMonsters = new[] {DefaultMonster};
        }
    }
}
