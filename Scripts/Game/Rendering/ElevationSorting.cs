namespace FrigidBlackwaters.Game
{
    public static class ElevationSorting
    {
        public static int SortingOrderFromElevation(SortElevation elevation)
        {
            return (int)SortElevation.Base - (int)elevation;
        }

        public static SortElevation ElevationFromSortingOrder(int sortingOrder)
        {
            return (SortElevation)((int)SortElevation.Base - sortingOrder);
        }
    }
}
