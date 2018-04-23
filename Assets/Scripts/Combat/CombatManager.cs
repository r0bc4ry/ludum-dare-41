using System.Collections;
using System.Collections.Generic;
using RestSharp.Contrib;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CombatManager : MonoBehaviour
{
    public GameObject ExplorationGameplayParent;
    public GameObject ExplorationUiParent;
    public GameObject CombatGameplayParent;
    public GameObject CombatUiParent;

    public AudioSource AudioSource;
    public AudioClip StartCombatAudioClip;
    public AudioClip HitAudioClip;
    public AudioClip MissAudioClip;
    public AudioClip NewBadgeClip;

    private Queue<Monster> _enemyMonsters = new Queue<Monster>();
    public Transform PlayerMonsterSpawnPosition;
    public Transform EnemyMonsterSpawnPosition;
    private MonsterController _playerMonsterController;
    private MonsterController _enemyMonsterController;

    // ActionSelection -> AttackSelection -> TriviaQuestion -> Attack or Miss
    public GameObject BattlePanelUi;
    public GameObject MonsterSelectionUi;
    public GameObject MonsterSelectionListUi;
    public GameObject AttackDifficultySelectionUi;
    public GameObject AttackQuestionUi;
    public Text AttackQuestionText;
    public Text AttackQuestionAnswerAText;
    public Text AttackQuestionAnswerBText;
    public Text AttackQuestionAnswerCText;
    public Text AttackQuestionAnswerDText;
    public GameObject AttackResultUi;

    public Image FadeCameraInOutImage;
    public MonsterPanel PlayerMonsterPanel;
    public MonsterPanel EnemyMonsterPanel;

    private bool _isPlayersTurn = true;

    private string _attackDifficulty;
    private int _correctAnswer;

    public Monster DefaultMonster;
    private InventoryManager _inventoryManager;

    private GameObject _playerMonsterMesh;
    private GameObject _enemyMonsterMesh;

    private NPC _activeNpc;

    private void Start() {
        _inventoryManager = FindObjectOfType<InventoryManager>();
    }

    // TODO This is for testing - remove
    public void Win() {
        EndBattle(true);
    }

    public void StartBattle(NPC npc) {
        _activeNpc = npc;

        // Play battle sound effect
        AudioSource.clip = StartCombatAudioClip;
        AudioSource.Play();

        // Enqueue monsteres
        _enemyMonsters.Clear();
        foreach (Monster monster in npc.EnemyMonsters) {
            _enemyMonsters.Enqueue(monster);
        }

        // Spawn enemy's monster
        Monster enemyMonster = _enemyMonsters.Dequeue();
        InstantiateMonster(enemyMonster, false);

        // Fade out and in on battle gameplay
        StartCoroutine(SwitchToCombatGameplay());
    }

    private IEnumerator SwitchToCombatGameplay() {
        float fadeTime = 0.5f;
        StartCoroutine(FadeCameraInOut(true, fadeTime));

        yield return new WaitForSeconds(fadeTime);

        ExplorationGameplayParent.SetActive(false);
        ExplorationUiParent.SetActive(false);
        CombatGameplayParent.SetActive(true);
        CombatUiParent.SetActive(true);

        BattlePanelUi.SetActive(true);

        if (_inventoryManager.Monsters.Count == 0) {
            InstantiateMonster(DefaultMonster, true);
            AttackDifficultySelectionUi.SetActive(true);
        } else if (_inventoryManager.Monsters.Count == 1) {
            InstantiateMonster(_inventoryManager.Monsters[0], true);
            AttackDifficultySelectionUi.SetActive(true);
        } else {
            ShowMonsterSelection();
        }

        StartCoroutine(FadeCameraInOut(false, fadeTime));
    }

    private IEnumerator FadeCameraInOut(bool fadeToBlack, float waitTime) {
        float elapsedTime = 0;
        while (elapsedTime < waitTime) {
            Color color = FadeCameraInOutImage.color;
            color.a = Mathf.Lerp(fadeToBlack ? 0f : 1f, fadeToBlack ? 1f : 0f, elapsedTime / waitTime);
            FadeCameraInOutImage.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void ShowMonsterSelection() {
        foreach (Monster monster in _inventoryManager.Monsters) {
            for (int i = 0; i < MonsterSelectionListUi.transform.childCount; i++) {
                GameObject button = MonsterSelectionListUi.transform.GetChild(i).gameObject;
                if (monster.Name == button.transform.Find("Text").GetComponent<Text>().text) {
                    button.SetActive(true);
                }
            }
        }

        MonsterSelectionUi.SetActive(true);
    }

    public void OnMonsterSelected(string category) {
        // Spawn player's selected monster
        foreach (Monster monster in _inventoryManager.Monsters) {
            if (category == monster.TriviaCategory.ToString()) {
                InstantiateMonster(monster, true);
                PlayerMonsterPanel.Monster = monster;
                PlayerMonsterPanel.MonsterController = _playerMonsterController;
                PlayerMonsterPanel.gameObject.SetActive(true);
            }
        }

        // Clean-up UI after selection
        for (int i = 0; i < MonsterSelectionListUi.transform.childCount; i++) {
            GameObject button = MonsterSelectionListUi.transform.GetChild(i).gameObject;
            button.SetActive(false);
        }

        MonsterSelectionUi.SetActive(false);

        // Move to attack difficulty selectiond
        AttackDifficultySelectionUi.SetActive(true);
    }

    private void InstantiateMonster(Monster monster, bool isFriendly) {
        GameObject instance = Instantiate(Resources.Load<GameObject>(monster.ResourcePath), isFriendly ? PlayerMonsterSpawnPosition : EnemyMonsterSpawnPosition);
        if (isFriendly) {
            _playerMonsterController = instance.GetComponent<MonsterController>();
            PlayerMonsterPanel.Monster = monster;
            PlayerMonsterPanel.MonsterController = _playerMonsterController;
            PlayerMonsterPanel.gameObject.SetActive(true);
            _playerMonsterMesh = instance;
        } else {
            _enemyMonsterController = instance.GetComponent<MonsterController>();
            EnemyMonsterPanel.Monster = monster;
            EnemyMonsterPanel.MonsterController = _enemyMonsterController;
            EnemyMonsterPanel.gameObject.SetActive(true);
            _enemyMonsterMesh = instance;
        }
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

                string question = HttpUtility.HtmlDecode(json["results"][0]["question"].str);

                // Create a List of answers, including the correct answer, and shuffle them
                List<string> answers = new List<string>();
                var jsonAnswers = json["results"][0]["incorrect_answers"];
                foreach (JSONObject obj in jsonAnswers.list) {
                    answers.Add(HttpUtility.HtmlDecode(obj.str));
                }

                answers.Add(HttpUtility.HtmlDecode(json["results"][0]["correct_answer"].str));
                ShuffleAnswers(answers);

                int correctAnswerIndex = answers.FindIndex(x => x == HttpUtility.HtmlDecode(json["results"][0]["correct_answer"].str));

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

    private List<string> _answers;

    private void UpdateAttackQuestion(string question, List<string> answers, int correctAnswerIndex) {
        _answers = answers;
        
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
            float damageAmount = DetermineDamageAmount(_attackDifficulty);

            resultText.text = "Correct! Your monster attacks for " + damageAmount + " damage!";

            // Play hit sound
            AudioSource.clip = HitAudioClip;
            AudioSource.Play();

            // Deal damage to enemy
            _enemyMonsterController.OnTakeDamage(damageAmount);
            if (_enemyMonsterController.MonsterHealth <= 0f) {
                EndBattle(true);
            }
        } else {
            // Play miss sound
            AudioSource.clip = MissAudioClip;
            AudioSource.Play();

            // TODO Show missed attack
            resultText.text = "Wrong! The correct answer was " + _answers[_correctAnswer] + ".";
            _playerMonsterController.OnMiss();
        }

        AttackQuestionUi.SetActive(false);
        AttackResultUi.SetActive(true);

        StartCoroutine(SwitchTurn(false, 3f));
    }

    private IEnumerator SwitchTurn(bool isPlayerNext, float waitTime) {
        yield return new WaitForSeconds(waitTime);

        if (isPlayerNext) {
            AttackResultUi.SetActive(false);
            AttackDifficultySelectionUi.SetActive(true);
        } else {
            OnEnemyTurn();
        }

        _isPlayersTurn = isPlayerNext;
    }

    private void OnEnemyTurn() {
        Text resultText = AttackResultUi.transform.Find("Text").GetComponent<Text>();

        if (Random.value >= 0.33333333333333333333333333333333f) {
            string difficulty = "medium";
            switch (Random.Range(0, 3)) {
                case 0:
                    difficulty = "easy";
                    break;
                case 1:
                    difficulty = "medium";
                    break;
                case 2:
                    difficulty = "hard";
                    break;
            }

            float damageAmount = DetermineDamageAmount(difficulty);

            resultText.text = "Enemy hits for " + damageAmount + " damage!";

            // Play hit sound
            AudioSource.clip = HitAudioClip;
            AudioSource.Play();

            // Deal damage to player
            _playerMonsterController.OnTakeDamage(damageAmount);

            if (_playerMonsterController.MonsterHealth <= 0f) {
                EndBattle(false);
            }
        } else {
            resultText.text = "Enemy misses!";

            // Play miss sound
            AudioSource.clip = MissAudioClip;
            AudioSource.Play();

            _enemyMonsterController.OnMiss();
        }

        StartCoroutine(SwitchTurn(true, 3f));
    }

    private float DetermineDamageAmount(string difficulty) {
        float damageAmount = 0f;
        switch (difficulty) {
            case "easy":
                damageAmount = 25f;
                break;
            case "medium":
                damageAmount = 37.5f;
                break;
            case "hard":
                damageAmount = 50f;
                break;
        }

        float criticalHitChance = 6.25f;
        float randomValue = Random.Range(0f, 100f);
        if (randomValue < criticalHitChance) {
            damageAmount *= 1.625f;
            StartCoroutine(ShowCriticalHit());
        }

        // Determine if critical hit
        // float criticalHitChange = 0.0625f;
        // float randValue = Random.value;

        // if (randValue < 1f - criticalHitChange) {
        //     Debug.Log("Critical Hit!"); // TODO Add visual feedback for critical hit
        //     damageAmount *= 1.625f;
        // }

        return damageAmount;
    }
    
    public Text WinText;
    public Text CriticalHitText;

    private IEnumerator ShowCriticalHit() {
        CriticalHitText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        CriticalHitText.gameObject.SetActive(false);
    }

    private void EndBattle(bool wonFight) {
        WinText.text = wonFight ? "You Won!" : "You Lost!";
        WinText.gameObject.SetActive(true);

        if (wonFight) {
            AudioSource.clip = NewBadgeClip;
            AudioSource.Play();

            _inventoryManager.Monsters.Add(_enemyMonsterController.Monster);
        }

        StartCoroutine(EndFightWait(wonFight));
    }

    private IEnumerator EndFightWait(bool wonFight) {
        yield return new WaitForSeconds(3f);

        if (_activeNpc != null && wonFight) {
            _activeNpc.OnEndBattle();
        }

        _activeNpc = null;

        StartCoroutine(SwitchToExplorationGameplay());
    }

    private IEnumerator SwitchToExplorationGameplay() {
        float fadeTime = 0.5f;
        StartCoroutine(FadeCameraInOut(true, fadeTime));

        yield return new WaitForSeconds(fadeTime);

        EnemyMonsterPanel.gameObject.SetActive(false);
        PlayerMonsterPanel.gameObject.SetActive(false);

        CombatGameplayParent.SetActive(false);
        CombatUiParent.SetActive(false);
        ExplorationGameplayParent.SetActive(true);
        ExplorationUiParent.SetActive(true);

        MonsterSelectionUi.SetActive(false);
        AttackDifficultySelectionUi.SetActive(false);
        AttackQuestionUi.SetActive(false);
        AttackResultUi.SetActive(false);

        WinText.gameObject.SetActive(false);

        Destroy(_enemyMonsterMesh);
        Destroy(_playerMonsterMesh);

        StartCoroutine(FadeCameraInOut(false, fadeTime));
    }
}
