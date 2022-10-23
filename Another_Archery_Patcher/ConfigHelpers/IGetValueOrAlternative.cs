namespace Another_Archery_Patcher.ConfigHelpers
{
    public interface IGetValueOrAlternative<T>
    {
        abstract T GetValueOrAlternative(T defaultValue, out bool changed);
    }
}
