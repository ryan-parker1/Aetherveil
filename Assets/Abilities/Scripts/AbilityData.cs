using UnityEngine;

[CreateAssetMenu(
    fileName = "New Ability",
    menuName = "MMO/Ability"
)]
public class AbilityData : ScriptableObject
{
    [Header("Basic Info")]
    public string abilityName;

    [TextArea]
    public string description;

    public Sprite icon;

    [Header("Combat")]
    public int damage = 10;

    public float range = 2f;

    public float cooldown = 1f;

    [Header("Input")]
    public KeyCode keybind = KeyCode.Alpha1;
}