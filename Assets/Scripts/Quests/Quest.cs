public class Quest
{
    public QuestData Data;

    public QuestStatus Status;

    public int CurrentKills;

    public Quest(QuestData data)
    {
        Data = data;

        Status = QuestStatus.Active;

        CurrentKills = 0;
    }

    public void AddKill()
    {
        if (Status != QuestStatus.Active)
            return;

        CurrentKills++;

        if (CurrentKills >= Data.requiredKills)
        {
            Status = QuestStatus.Complete;
        }
    }
}