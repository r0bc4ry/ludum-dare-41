using UnityEngine;
using UnityEngine.UI;

public class BadgeProgress : MonoBehaviour
{
    public Sprite ArtsAndLiteratureSprite;
    public Sprite EntertainmentSprite;
    public Sprite GeneralKnowledgeSprite;
    public Sprite GeographySprite;
    public Sprite HistorySprite;
    public Sprite ScienceAndNatureSprite;
    public Sprite SportsAndLeisureSprite;

    private InventoryManager _inventoryManager;

    void Start() {
        _inventoryManager = FindObjectOfType<InventoryManager>();
    }

    private void OnGUI() {
        foreach (Monster monster in _inventoryManager.Monsters) {
            TriviaCategory category = monster.TriviaCategory;
            Image image = transform.Find(category.ToString()).GetComponent<Image>();

            switch (category) {
                case TriviaCategory.ArtsAndLiterature:
                    image.sprite = ArtsAndLiteratureSprite;
                    break;
                case TriviaCategory.Entertainment:
                    image.sprite = EntertainmentSprite;
                    break;
                case TriviaCategory.Geography:
                    image.sprite = GeographySprite;
                    break;
                case TriviaCategory.GeneralKnowledge:
                    image.sprite = GeneralKnowledgeSprite;
                    break;
                case TriviaCategory.History:
                    image.sprite = HistorySprite;
                    break;
                case TriviaCategory.ScienceAndNature:
                    image.sprite = ScienceAndNatureSprite;
                    break;
                case TriviaCategory.SportsAndLeisure:
                    image.sprite = SportsAndLeisureSprite;
                    break;
            }
        }
    }
}
