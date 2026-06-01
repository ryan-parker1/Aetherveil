using UnityEngine;

[CreateAssetMenu(
    fileName = "New Dialogue",
    menuName = "MMO/Dialogue"
)]
public class DialogueData : ScriptableObject
{
    [Header("NPC")]
    public string npcName;

    [Header("Pages")]
    [TextArea(3, 10)]
    public string[] pages;

    [Header("Quest (optional)")]
    [Tooltip("If set, Accept/Decline buttons appear on the last page.")]
    public QuestData questOffer;
}
