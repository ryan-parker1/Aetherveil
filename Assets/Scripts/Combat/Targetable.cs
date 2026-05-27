using UnityEngine;

public class Targetable : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private string targetName = "Enemy";

    public string TargetName => targetName;
}