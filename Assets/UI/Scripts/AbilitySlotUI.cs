using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;

    [SerializeField] private Image cooldownOverlay;

    [SerializeField] private TextMeshProUGUI keybindText;

    private AbilityData ability;

    public void Setup(AbilityData newAbility)
    {
        ability = newAbility;

        icon.sprite = ability.icon;

        keybindText.text =
            ability.keybind.ToString().Replace("Alpha", "");
    }

    public void SetCooldownVisible(bool visible)
    {
        cooldownOverlay.gameObject.SetActive(visible);
    }

    public AbilityData GetAbility()
    {
        return ability;
    }
}