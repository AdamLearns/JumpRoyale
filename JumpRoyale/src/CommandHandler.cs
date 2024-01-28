public class CommandHandler
{
#pragma warning disable S2933 // Fields that are only assigned in the constructor should be "readonly"
    private Arena _arena;
#pragma warning restore S2933 // Fields that are only assigned in the constructor should be "readonly"

    public CommandHandler(ref Arena arena)
    {
        _arena = arena;
    }

    public ref Arena Arena => ref _arena;
}
