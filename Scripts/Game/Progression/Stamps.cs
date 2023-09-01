public static class Stamps
{
    private static int currentStamps;
    private static int totalStamps;

    public static int CurrentStamps
    {
        get
        {
            return currentStamps;
        }
        set
        {
            currentStamps = value;
        }
    }

    public static int TotalStamps
    {
        get
        {
            return totalStamps;
        }
        set
        {
            totalStamps = value;
        }
    }
}
