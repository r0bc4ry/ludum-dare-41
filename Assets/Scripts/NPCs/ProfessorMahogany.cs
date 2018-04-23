using UnityEngine;

public class ProfessorMahogany : MonoBehaviour
{
    public Monster DefaultMonster;

    private GameManager _gameManager;
    private NPC _npc;

    private string[] _noMonstersBeforeSentences = {
        "Welcome to the world of Trivia Monsters! I'm Professor Mahogany, nice to meet you!",
        "You came at the perfect time. A group of thieves have stolen all the Trivia Monsters!",
        "There are 7 monsters around this area, but I'm unable to get them myself. Would you please find them for me?",
        "But first... let's practice your battle skills with this General Knowledge Monster!"
    };

    private string[] _noMonstersAfterSentences = {
        "Wow! You're a natural! Please take the General Knowledge Monster and this badge.",
        "Now go find those missing monsters and come back to me when you've gotten them all!"
    };

    private string[] _gameplaySentences = {
        "What're you doing here?! Go find those monsters!"
    };

    private string[] _allMonstersSentences = {
        "I knew you would do it! You must be a genius!",
        "The monsters will now live safe and sound thanks to you!"
    };

    void Start() {
        _gameManager = FindObjectOfType<GameManager>();
        _npc = GetComponent<NPC>();
        _npc.BeforeDialogue.Name = "Professor Mahogany";
        _npc.AfterDialogue.Name = "Professor Mahogany";
    }

    void Update() {
        if (_gameManager.HasNoMonsters) {
            _npc.BeforeDialogue.Sentences = _noMonstersBeforeSentences;
            _npc.AfterDialogue.Sentences = _noMonstersAfterSentences;
            _npc.EnemyMonsters = new[] {DefaultMonster};
        } else if (_gameManager.HasAllMonsters) {
            _npc.BeforeDialogue.Sentences = _allMonstersSentences;
            _npc.AfterDialogue.Sentences = new string[] { };
            _npc.EnemyMonsters = new Monster[] { };
        } else {
            _npc.BeforeDialogue.Sentences = _gameplaySentences;
            _npc.AfterDialogue.Sentences = new string[] { };
            _npc.EnemyMonsters = new Monster[] { };
        }
    }
}
