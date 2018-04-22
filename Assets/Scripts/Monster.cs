using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Monster")]
public class Monster : ScriptableObject
{
    public string Name = "Monster";
    public float Health = 100f;
    public TriviaCategory TriviaCategory = TriviaCategory.GeneralKnowledge;
    public string ResourcePath;
}
