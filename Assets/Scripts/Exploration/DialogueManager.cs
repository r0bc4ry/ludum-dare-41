using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject DialoguePanelUi;
    public Text DialogueNameText;
    public Text DialogueSentenceText;

    public AudioSource AudioSource;
    public AudioSource AudioSource2;
    public AudioClip SelectAudioClip;
    public AudioClip TypingAudioClip;

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

    public void DisplayNextSentenceClick() {
        AudioSource.clip = SelectAudioClip;
        AudioSource.Play();

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (_sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = _sentences.Dequeue();
        StartCoroutine(AnimateText(sentence));
    }

    private IEnumerator AnimateText(string strComplete) {
        int i = 0;
        DialogueSentenceText.text = "";
        while (i < strComplete.Length) {
            AudioSource2.clip = TypingAudioClip;
            AudioSource2.Play();
            
            DialogueSentenceText.text += strComplete[i++];
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void EndDialogue() {
        DialoguePanelUi.SetActive(false);
        _npc.OnEndDialogue();
    }
}
