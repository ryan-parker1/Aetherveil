using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetNameText;

    [SerializeField] private Slider healthBar;

    [SerializeField] private TargetingController targetingController;

    private void Update()
    {
        UpdateTargetUI();
    }

    private void UpdateTargetUI()
    {
        // TargetingController lives on the Player which is spawned by FishNet
        // — find it once it exists
        if (targetingController == null)
        {
            targetingController =
                FindAnyObjectByType<TargetingController>();
            return;
        }

        if (targetingController.CurrentTarget == null)
        {
            targetNameText.text = "No Target";

            healthBar.value = 0;

            return;
        }

        Targetable target =
            targetingController.CurrentTarget
                .GetComponent<Targetable>();

        Health health =
            targetingController.CurrentTarget
                .GetComponent<Health>();

        CombatStats stats =
            targetingController.CurrentTarget
                .GetComponent<CombatStats>();

        if (target != null)
        {
            targetNameText.text = target.TargetName;
        }

        if (health != null && stats != null)
        {
            healthBar.maxValue = stats.MaxHealth;

            healthBar.value = health.CurrentHealth;
        }
    }
}