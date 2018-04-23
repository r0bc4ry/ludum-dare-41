using UnityEngine;
using UnityEngine.UI;

public class MonsterPanel : MonoBehaviour
{
    public Monster Monster;
    public MonsterController MonsterController;

    private Text _monsterNameText;
    private Text _monsterCategoryText;
    private Text _monsterHealthText;

    private void Start() {
        _monsterNameText = transform.Find("MonsterName").GetComponent<Text>();
        _monsterCategoryText = transform.Find("MonsterCategory").GetComponent<Text>();
        _monsterHealthText = transform.Find("MonsterHealth").GetComponent<Text>();
    }

    void OnGUI() {
        _monsterNameText.text = Monster.Name;
        _monsterCategoryText.text = Monster.TriviaCategory.ToString();
        // TODO Create progress bar for monster health
        _monsterHealthText.text = "Health: " + MonsterController.MonsterHealth + "/" + Monster.Health;
    }
}
