using System;


public abstract class UILobbyMiddleBasePresenter : IDisposable
{
    protected readonly UILobbyPresenter.Deps _deps;

    protected UILobbyMiddleBasePresenter(UILobbyPresenter.Deps deps)
    {
        _deps = deps;
    }

    public abstract void Initialize();
    public abstract void Dispose();
}

public abstract class UILobbyMiddlePresenter<TView> : UILobbyMiddleBasePresenter where TView : UILobbyMiddleBase
{
    protected readonly TView _view;

    protected UILobbyMiddlePresenter(TView view, UILobbyPresenter.Deps deps) : base(deps)
    {
        _view = view;
        Bind();
    }

    public override void Dispose()
    {
        Unbind();
    }

    protected abstract void Bind();
    protected abstract void Unbind();
}