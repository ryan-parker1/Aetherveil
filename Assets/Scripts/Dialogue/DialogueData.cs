using UnityEngine;

[CreateAssetMenu(
    fileName = "New Dialogue",
    menuName = "MMO/Dialogue"
)]
public class DialogueData : ScriptableObject
{
    [TextArea(3, 10)]
    public string dialogueText;
}