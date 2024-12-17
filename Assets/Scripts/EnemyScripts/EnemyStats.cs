public class EnemyStats
{
    public static int BaseHP = 100;
    public static int DamageIncrement = 5;

    public static int RunningEnemyBaseDamage = 20;
    public static int HidingEnemyBaseDamage = 10;

    public static void IncreaseStats(int hpIncrement)
    {
        BaseHP += hpIncrement;
        RunningEnemyBaseDamage += DamageIncrement;
        HidingEnemyBaseDamage += DamageIncrement;
    }
}
