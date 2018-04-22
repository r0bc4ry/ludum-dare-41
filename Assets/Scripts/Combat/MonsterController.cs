using UnityEngine;

public class MonsterController : MonoBehaviour
{    
    public Monster Monster;

    // TODO Add a "level" to the monster and include damage/health multipliers

    public void OnAttack() {
        Debug.Log("Attack");
    }

    public void OnMiss() {
        Debug.Log("Attack");
    }

    public void OnTakeDamage(float damage) {
        // TODO Multiply damage by category interactions
        Debug.Log("TakeDamage");
        Monster.Health = Mathf.Max(0f, Monster.Health - damage);
    }

    public int GetCategoryInt() {
        int[] array;

        switch (Monster.TriviaCategory) {
            case TriviaCategory.ArtsAndLiterature:
                return 0;
            case TriviaCategory.Entertainment:
                // Film: 11, Music: 12, Theater: 13, TV: 14, Video Games: 15, Board Games: 16, Celebrities: 26, Comics: 29, Anime: 31, Cartoons: 32
                array = new[] {11, 12, 13, 14, 15, 16, 26, 29, 31, 32};
                return array[Random.Range(0, array.Length)];
            case TriviaCategory.Geography:
                return 22;
            case TriviaCategory.History:
                return 23;
            case TriviaCategory.ScienceAndNature:
                // Science & Nature: 17, Computers: 18, Math: 19, Animals: 27, Gadgets: 30
                array = new[] {17, 18, 19, 27, 30};
                return array[Random.Range(0, array.Length)];
            case TriviaCategory.SportsAndLeisure:
                return 21;
            default:
                // General Knowledge
                return 9;
        }
    }
}
