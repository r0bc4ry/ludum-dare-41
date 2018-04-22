using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public GameObject ExplorationGameplayParent;
    public GameObject ExplorationUiParent;
    public GameObject CombatGameplayParent;
    public GameObject CombatUiParent;

    private Queue<Monster> _enemyMonsters = new Queue<Monster>();
    public Transform PlayerMonsterSpawnPosition;
    public Transform EnemyMonsterSpawnPosition;
    private MonsterController _playerMonsterController;
    private MonsterController _enemyMonsterController;
    private float _playerMonsterOriginalHealth;
    private float _enemyMonsterOriginalHealth;

    // ActionSelection -> AttackSelection -> TriviaQuestion -> Attack or Miss
    public GameObject BattlePanelUi;
    public GameObject ActionSelectionUi;
    public GameObject AttackDifficultySelectionUi;
    public GameObject AttackQuestionUi;
    public Text AttackQuestionText;
    public Text AttackQuestionAnswerAText;
    public Text AttackQuestionAnswerBText;
    public Text AttackQuestionAnswerCText;
    public Text AttackQuestionAnswerDText;
    public GameObject AttackResultUi;

    // TODO Make MonsterPanel prefab showing stats about monster(s)
    public MonsterPanel PlayerMonsterPanel;
    public MonsterPanel EnemyMonsterPanel;

    private bool _isPlayersTurn = true;
    private bool _isBeginningOfPlayersTurn = true;

    private string _attackDifficulty;
    private int _correctAnswer;

    public void StartBattle(Monster[] monsters) {
        _enemyMonsters.Clear();
        foreach (Monster monster in monsters) {
            _enemyMonsters.Enqueue(monster);
        }

        // Spawn enemy's monster
        Monster enemyMonster = _enemyMonsters.Dequeue();
        InstantiateMonster(enemyMonster, false);
        _enemyMonsterOriginalHealth = enemyMonster.Health;

        // TODO Let player choose monster to fight with

        // Spawn player's monster
        Monster playerMonster = FindObjectOfType<InventoryManager>().Monsters[0];
        InstantiateMonster(FindObjectOfType<InventoryManager>().Monsters[0], true);
        _playerMonsterOriginalHealth = playerMonster.Health;

        EnemyMonsterPanel.Monster = enemyMonster;
        PlayerMonsterPanel.Monster = playerMonster;

        ExplorationGameplayParent.SetActive(false);
        ExplorationUiParent.SetActive(false);
        CombatGameplayParent.SetActive(true);
        CombatUiParent.SetActive(true);

        OnAttackAction();
    }

    private void InstantiateMonster(Monster monster, bool isFriendly) {
        GameObject instance = Instantiate(Resources.Load<GameObject>(monster.ResourcePath), isFriendly ? PlayerMonsterSpawnPosition : EnemyMonsterSpawnPosition);
        if (isFriendly) {
            _playerMonsterController = instance.GetComponent<MonsterController>();
        } else {
            _enemyMonsterController = instance.GetComponent<MonsterController>();
        }
    }

    void Update() {
        if (_isPlayersTurn) {
            if (_isBeginningOfPlayersTurn) {
                BattlePanelUi.SetActive(true);
                OnAttackAction(); // TODO Move to after selection of action
                _isBeginningOfPlayersTurn = false;
            }
        } else {
            // TODO Handle enemy AI
            OnEnemyTurn();
        }
    }

    void OnAttackAction() {
        // TODO AttackSelection UI
        AttackDifficultySelectionUi.SetActive(true);
    }

    public void OnAttack(string difficulty) {
        _attackDifficulty = difficulty;
        switch (_attackDifficulty) {
            case "easy":
                StartCoroutine(GetTriviaQuestion("easy"));
                break;
            case "medium":
                StartCoroutine(GetTriviaQuestion("medium"));
                break;
            case "hard":
                StartCoroutine(GetTriviaQuestion("hard"));
                break;
        }
    }

    private IEnumerator GetTriviaQuestion(string difficulty) {
        // TODO Show loading icon while doing web request

        int category = _playerMonsterController.GetCategoryInt();
        using (UnityWebRequest www = UnityWebRequest.Get("https://opentdb.com/api.php?amount=1&category=" + category + "&difficulty=" + difficulty + "&type=multiple")) {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                // TODO Save list of questions with game when offline
                // TODO Handle web request failures and edge cases

                JSONObject json = new JSONObject(www.downloadHandler.text);

                string question = json["results"][0]["question"].str;

                // Create a List of answers, including the correct answer, and shuffle them
                List<string> answers = new List<string>();
                var jsonAnswers = json["results"][0]["incorrect_answers"];
                foreach (JSONObject obj in jsonAnswers.list) {
                    answers.Add(obj.str);
                }

                answers.Add(json["results"][0]["correct_answer"].str);
                ShuffleAnswers(answers);

                int correctAnswerIndex = answers.FindIndex(x => x == json["results"][0]["correct_answer"].str);

                UpdateAttackQuestion(question, answers, correctAnswerIndex);
            }
        }
    }

    private void ShuffleAnswers(List<string> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, list.Count);
            string value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void UpdateAttackQuestion(string question, List<string> answers, int correctAnswerIndex) {
        AttackQuestionText.text = question;
        AttackQuestionAnswerAText.text = answers[0];
        AttackQuestionAnswerBText.text = answers[1];
        AttackQuestionAnswerCText.text = answers[2];
        AttackQuestionAnswerDText.text = answers[3];
        _correctAnswer = correctAnswerIndex;

        AttackDifficultySelectionUi.SetActive(false);
        AttackQuestionUi.SetActive(true);
    }

    public void OnAnswer(int selectedAnswer) {
        Text resultText = AttackResultUi.transform.Find("Text").GetComponent<Text>();

        if (selectedAnswer == _correctAnswer) {
            // TODO Show better UI for attack hit and miss
            resultText.text = "Correct! Attack!";

            // TODO Update damage calculation based on question difficulty
            float damageAmount = 0f;
            switch (_attackDifficulty) {
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

            // Determine if critical hit
            float criticalHitChange = 0.0625f;
            float randValue = Random.value;

            if (randValue < 1f - criticalHitChange) {
                Debug.Log("Critical Hit!"); // TODO Add visual feedback for critical hit
                damageAmount *= 1.625f;
            }

            _playerMonsterController.OnAttack();
            _enemyMonsterController.OnTakeDamage(damageAmount);

            if (_enemyMonsterController.Monster.Health <= 0f) {
                OnMonsterDeath(true);
            }
        } else {
            // TODO Show missed attack
            resultText.text = "Wrong! Miss!";
            _playerMonsterController.OnMiss();
        }

        AttackQuestionUi.SetActive(false);
        AttackResultUi.SetActive(true);

        StartCoroutine(SwitchTurn(2f));
    }

    private IEnumerator SwitchTurn(float waitTime) {
        yield return new WaitForSeconds(waitTime);

        AttackResultUi.SetActive(false);
        BattlePanelUi.SetActive(false);
        _isPlayersTurn = false;
        _isBeginningOfPlayersTurn = true;
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

            _enemyMonsterController.OnAttack();
            _playerMonsterController.OnTakeDamage(damageAmount);

            if (_playerMonsterController.Monster.Health <= 0f) {
                OnMonsterDeath(false);
            }
        } else {
            _enemyMonsterController.OnMiss();
        }

        // TODO Let player know if enemy hit or nah

        _isPlayersTurn = true;
    }

    private void OnMonsterDeath(bool wonFight) {
        if (wonFight) {
            // TODO Give player new monster/category in GameManager
            FindObjectOfType<InventoryManager>().Monsters.Add(_enemyMonsterController.Monster);
        }

        EndBattle();
    }

    public void EndBattle() {
        // Reset monster health
        _enemyMonsterController.Monster.Health = _enemyMonsterOriginalHealth;
        _playerMonsterController.Monster.Health = _playerMonsterOriginalHealth;

        CombatGameplayParent.SetActive(false);
        CombatUiParent.SetActive(false);
        ExplorationGameplayParent.SetActive(true);
        ExplorationUiParent.SetActive(true);
    }
}
