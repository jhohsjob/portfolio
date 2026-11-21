using System;

// 게임 세션(접속~로그아웃)의 수명을 관리한다. 환경 묶음은 GameContext가 맡는다.
public sealed class GameSession : IDisposable
{
    public static GameSession Current { get; private set; }

    public GameContext context { get; private set; }

    private bool _disposed;

    private GameSession(AppContext app)
    {
        context = new GameContext(app);
    }

    public static GameSession Begin(AppContext app)
    {
        End();
        Current = new GameSession(app);
        return Current;
    }

    public static void End()
    {
        Current?.Dispose();
        Current = null;
    }

    public void Dispose()
    {
        if (_disposed == true)
        {
            return;
        }
        _disposed = true;

        context?.Dispose();
        context = null;
    }
}