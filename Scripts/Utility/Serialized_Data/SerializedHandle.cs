namespace FrigidBlackwaters.Utility
{
    public abstract class SerializedHandle<T>
    {
        public abstract bool TryGetValue(out T value);
    }
}
