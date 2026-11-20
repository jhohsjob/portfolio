#if DEBUG_MODE
public class UIDebugPresenter
{
    public class Deps
    {
        public User user;
    }


    private UIDebug _view;
    private Deps _deps;

    public UIDebugPresenter(UIDebug view)
    {
        _view = view;

        view.onClickAddUserExp += OnClickUserAddExp;
        view.onClickAddGold += OnClickAddGold;
    }

    public void initialize(Deps deps)
    {
        _deps = deps;
    }

    private async void OnClickUserAddExp()
    {
        await _deps.user.DebugAddExp();
    }

    private async void OnClickAddGold()
    {
        await _deps.user.DebugAddGold();
    }
}
#endif