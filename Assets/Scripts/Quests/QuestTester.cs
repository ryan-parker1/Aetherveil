using UnityEngine;

public class QuestTester : MonoBehaviour
{
    [SerializeField]
    private QuestData testQuest;

    private QuestLog questLog;

    private void Awake()
    {
        questLog =
            GetComponent<QuestLog>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            questLog.AcceptQuest(
                testQuest
            );
        }
    }
}