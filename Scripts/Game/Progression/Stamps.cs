using System;

public static class Stamps
{
    private static int currentAmount;
    private static int totalAmount;

    private static Action<int> onCurrentQuantityChanged;

    public static int CurrentAmount
    {
        get
        {
            return currentAmount;
        }
        set
        {
            if (currentAmount != value)
            {
                onCurrentQuantityChanged?.Invoke(value);
            }
            currentAmount = value;
        }
    }

    public static int TotalAmount
    {
        get
        {
            return totalAmount;
        }
        set
        {
            totalAmount = value;
        }
    }

    public static Action<int> OnCurrentQuantityChanged
    {
        get
        {
            return onCurrentQuantityChanged;
        }
        set
        {
            onCurrentQuantityChanged = value;
        }
    }
}