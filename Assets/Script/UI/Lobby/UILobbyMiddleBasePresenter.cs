using System;


public abstract class UILobbyMiddleBasePresenter : IDisposable
{
    protected readonly UILobbyContext _context;

    protected UILobbyMiddleBasePresenter(UILobbyContext context)
    {
        _context = context;
    }

    public abstract void Initialize();
    public abstract void Dispose();
}

public abstract class UILobbyMiddlePresenter<TView> : UILobbyMiddleBasePresenter where TView : UILobbyMiddleBase
{
    protected readonly TView _view;

    protected UILobbyMiddlePresenter(TView view, UILobbyContext context) : base(context)
    {
        _view = view;
        Bind();
    }

    protected abstract void Bind();
    protected abstract void Unbind();

    public override void Dispose()
    {
        Unbind();
    }
}