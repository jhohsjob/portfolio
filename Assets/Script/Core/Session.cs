public static class Session
{
    public static GameContext Current { get; private set; }

    public static GameContext Begin(AppContext app)
    {
        End();
        Current = new GameContext(app);
        return Current;
    }

    public static void End()
    {
        Current?.Dispose();
        Current = null;
    }
}