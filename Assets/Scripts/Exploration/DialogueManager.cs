using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject DialoguePanelUi;
    public Text DialogueNameText;
    public Text DialogueSentenceText;

    private NPC _npc;
    private Queue<string> _sentences;

    private void Start() {
        _sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue, NPC npc) {
        _npc = npc;

        _sentences.Clear();
        foreach (string sentence in dialogue.Sentences) {
            _sentences.Enqueue(sentence);
        }

        DialogueNameText.text = dialogue.Name;
        DialoguePanelUi.SetActive(true);
        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (_sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = _sentences.Dequeue();
        DialogueSentenceText.text = sentence;
    }

    private void EndDialogue() {
        DialoguePanelUi.SetActive(false);
        _npc.OnEndDialogue();
    }
}
