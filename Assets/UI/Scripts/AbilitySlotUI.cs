using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;

    [SerializeField] private Image cooldownOverlay;

    [SerializeField] private TextMeshProUGUI keybindText;

    private AbilityData ability;

    private AbilityController abilityController;

    public void Setup(
        AbilityData newAbility,
        AbilityController controller
    )
    {
        ability = newAbility;

        abilityController = controller;

        icon.sprite = ability.icon;

        keybindText.text =
            ability.keybind.ToString().Replace("Alpha", "");
    }

    private void Update()
    {
        UpdateCooldownVisual();
    }

    private void UpdateCooldownVisual()
    {
        if (ability == null || abilityController == null)
            return;

        if (!abilityController.CooldownTimers
            .ContainsKey(ability))
        {
            cooldownOverlay.gameObject.SetActive(false);

            return;
        }

        float cooldownEndTime =
            abilityController.CooldownTimers[ability];

        float remainingTime =
            cooldownEndTime - Time.time;

        if (remainingTime <= 0)
        {
            cooldownOverlay.gameObject.SetActive(false);

            return;
        }

        cooldownOverlay.gameObject.SetActive(true);

        cooldownOverlay.fillAmount =
            remainingTime / ability.cooldown;
    }

    public AbilityData GetAbility()
    {
        return ability;
    }
}