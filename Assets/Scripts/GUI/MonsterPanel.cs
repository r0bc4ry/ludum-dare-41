using UnityEngine;
using UnityEngine.UI;

public class MonsterPanel : MonoBehaviour
{
    public Monster Monster;

    private Text _monsterNameText;
    private Text _monsterHealthText;

    private float _monsterOriginalHealth;

    private void Start() {
        _monsterNameText = transform.Find("MonsterName").GetComponent<Text>();
        _monsterHealthText = transform.Find("MonsterHealth").GetComponent<Text>();

        _monsterOriginalHealth = Monster.Health;
    }

    void OnGUI() {
        _monsterNameText.text = Monster.Name;
        _monsterHealthText.text = Monster.Health + "/" + _monsterOriginalHealth;
    }
}
