using UnityEngine;

public class Monster : MonoBehaviour
{
    public string Name = "My Monster";
    public float Health = 100f;

    private TriviaCategory _triviaCategory;
    private int _level; // TODO Update this later to include damage/health multipliers

    public void OnAttack() {
        Debug.Log("Attack");
    }

    public void OnMiss() {
        Debug.Log("Attack");
    }

    public void OnTakeDamage(float damage) {
        Debug.Log("TakeDamage");
        Health = Mathf.Min(0f, Health - damage);
    }
}

public enum TriviaCategory
{
    Geography,
    Entertainment,
    History,
    ArtsAndLiterature,
    ScienceAndNature,
    SportAndLeisure
}
