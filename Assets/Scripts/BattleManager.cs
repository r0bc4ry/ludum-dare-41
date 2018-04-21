using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private bool _isPlayersTurn = true;

    public Monster FriendlyMonster;
    public Monster EnemyMonster;

    public GameObject BattleGUI;
    public GameObject ActionSelectionGUI;
    public GameObject AttackSelectionGUI;
    public GameObject TriviaQuestionGUI;
    public GameObject TriviaResultGUI;

    // TODO Make MonsterPanel prefab showing stats about monster(s)
    public GameObject FriendlyMonsterPanel;
    public GameObject EnemyMonsterPanel;

    private string _questionDifficulty;
    private int _correctAnswer;

    // ActionSelection -> AttackSelection -> TriviaQuestion -> Attack or Miss

    private void Start() {
        OnAttackAction();
    }


    private bool _isBeginningOfTurn = true;

    void Update() {
        if (_isPlayersTurn) {
            if (_isBeginningOfTurn) {
                BattleGUI.SetActive(true);
                OnAttackAction();
                _isBeginningOfTurn = false;
            }
        } else {
            // TODO Handle enemy AI
            OnEnemyTurn();
        }
    }

    void OnAttackAction() {
        // TODO AttackSelection GUI
        AttackSelectionGUI.SetActive(true);
    }

    public void OnAttack(string difficulty) {
        _questionDifficulty = difficulty;
        // TODO Grab question based on difficulty and update GUI

        // TODO Fix this
        Text questionText = TriviaQuestionGUI.transform.Find("Question").GetComponent<Text>();
        Text answerA = TriviaQuestionGUI.transform.Find("Answers").Find("AnswerAButton").Find("Text")
            .GetComponent<Text>();
        Text answerB = TriviaQuestionGUI.transform.Find("Answers").Find("AnswerBButton").Find("Text")
            .GetComponent<Text>();
        Text answerC = TriviaQuestionGUI.transform.Find("Answers").Find("AnswerCButton").Find("Text")
            .GetComponent<Text>();
        Text answerD = TriviaQuestionGUI.transform.Find("Answers").Find("AnswerDButton").Find("Text")
            .GetComponent<Text>();

        // TODO After getting the question from "somewhere" - need to track what the correct answer is
        switch (_questionDifficulty) {
            case "easy":
                questionText.text = "This is the EASY question?";
                answerA.text = "Right Answer";
                answerB.text = "Wrong Answer";
                answerC.text = "Wrong Answer";
                answerD.text = "Wrong Answer";
                _correctAnswer = 0;
                break;
            case "medium":
                questionText.text = "This is the MEDIUM question?";
                answerA.text = "Wrong Answer";
                answerB.text = "Wrong Answer";
                answerC.text = "Right Answer";
                answerD.text = "Wrong Answer";
                _correctAnswer = 2;
                break;
            case "hard":
                questionText.text = "This is the HARD question?";
                answerA.text = "Wrong Answer";
                answerB.text = "Wrong Answer";
                answerC.text = "Wrong Answer";
                answerD.text = "Right Answer";
                _correctAnswer = 3;
                break;
        }

        AttackSelectionGUI.SetActive(false);
        TriviaQuestionGUI.SetActive(true);
    }

    public void OnAnswer(int choice) {
        Text resultText = TriviaResultGUI.transform.Find("Text").GetComponent<Text>();

        if (choice == _correctAnswer) {
            // TODO Call Attack() and TakeDamage() on monsters
            resultText.text = "Correct! Attack!";

            float damageAmount = 0f;
            switch (_questionDifficulty) {
                case "easy":
                    damageAmount = 10f;
                    break;
                case "medium":
                    damageAmount = 25f;
                    break;
                case "hard":
                    damageAmount = 50f;
                    break;
            }

            FriendlyMonster.OnAttack();
            EnemyMonster.OnTakeDamage(damageAmount);

            if (EnemyMonster.Health <= 0f) {
                Debug.Log("Dead!");
            }
        } else {
            // TODO Show missed attack
            resultText.text = "Wrong! Miss!";
            FriendlyMonster.OnMiss();
        }

        TriviaQuestionGUI.SetActive(false);
        TriviaResultGUI.SetActive(true);

        StartCoroutine(SwitchTurn(3f));
    }

    private IEnumerator SwitchTurn(float waitTime) {
        yield return new WaitForSeconds(waitTime);

        TriviaResultGUI.SetActive(false);
        BattleGUI.SetActive(true);
        _isPlayersTurn = false;
        _isBeginningOfTurn = true;
    }

    private void OnEnemyTurn() {
        int difficulty = Random.Range(0, 3);
        bool attackHits = Random.value >= 0.5;

        if (attackHits) {
            float damageAmount = 0f;
            switch (difficulty) {
                case 0:
                    damageAmount = 10f;
                    break;
                case 1:
                    damageAmount = 25f;
                    break;
                case 2:
                    damageAmount = 50f;
                    break;
            }

            EnemyMonster.OnAttack();
            FriendlyMonster.OnTakeDamage(damageAmount);

            if (FriendlyMonster.Health <= 0f) {
                Debug.Log("Dead!");
            }
        } else {
            EnemyMonster.OnMiss();
        }

        // TODO Let player know if enemy hit or nah

        _isPlayersTurn = true;
    }
}
