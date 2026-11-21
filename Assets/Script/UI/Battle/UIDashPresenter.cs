using System;


public class UIDashPresenter : IDisposable
{
    private readonly UIDash _view;
    private readonly DashAbility _dash;
    private readonly Action _onDashAction;

    public UIDashPresenter(UIDash view, DashAbility dash, Action onDashAction)
    {
        _view = view;
        _dash = dash;
        _onDashAction = onDashAction;

        Bind();
    }

    public void Dispose()
    {
        Unbind();
    }

    private void Bind()
    {
        _view.onClickDash += HandleClickDash;
        _dash.onCooldownChanged += UpdateDashUI;
    }

    private void Unbind()
    {
        _view.onClickDash -= HandleClickDash;
        _dash.onCooldownChanged -= UpdateDashUI;
    }

    public void Initialize()
    {
        _view.SetCooldown(0f);
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