#if DEBUG_MODE

public class UIDebugContext
{
    public User user;
}

public class UIDebugPresenter
{
    private UIDebug _view;
    private UIDebugContext _context;

    public UIDebugPresenter(UIDebug view)
    {
        _view = view;

        view.onClickAddUserExp += OnClickUserAddExp;
        view.onClickAddGold += OnClickAddGold;
    }

    public void initialize(UIDebugContext context)
    {
        _context = context;
    }

    private async void OnClickUserAddExp()
    {
        await _context.user.DebugAddExp();
    }

    private async void OnClickAddGold()
    {
        await _context.user.DebugAddGold();
    }
}
#endif