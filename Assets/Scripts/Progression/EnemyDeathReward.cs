using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(ExperienceReward))]
public class EnemyDeathReward : MonoBehaviour
{
    private Health health;

    private ExperienceReward reward;

    private void Awake()
    {
        health = GetComponent<Health>();

        reward = GetComponent<ExperienceReward>();
    }

    private void OnEnable()
    {
        health.OnDeath += GiveExperience;
    }

    private void OnDisable()
    {
        health.OnDeath -= GiveExperience;
    }

    private void GiveExperience()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        Experience experience =
            player.GetComponent<Experience>();

        if (experience == null)
            return;

        experience.GainExperience(
            reward.XPReward
        );
    }
}