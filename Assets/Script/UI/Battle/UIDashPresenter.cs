using System;


public class UIDashPresenter : IDisposable
{
    private readonly UIDash _view;
    private readonly DashController _dashController;
    private readonly Action _onDashAction;

    public UIDashPresenter(UIDash view, DashController dashController, Action onDashAction)
    {
        _view = view;
        _dashController = dashController;
        _onDashAction = onDashAction;

        Bind();
    }

    private void Bind()
    {
        _view.onClickDash += HandleClickDash;
        _dashController.onCooldownChanged += UpdateDashUI;
    }

    public void Initialize()
    {
        _view.SetCooldown(0f);
    }

    public void Dispose()
    {
        _view.onClickDash -= HandleClickDash;
        _dashController.onCooldownChanged -= UpdateDashUI;
    }

    private void UpdateDashUI(float timer, float duration)
    {
        if (timer > 0f)
        {
            _view.SetCooldown(timer / duration);
        }
        else
        {
            _view.SetCooldown(0f);
        }
    }

    private void HandleClickDash()
    {
        _onDashAction?.Invoke();
    }
}