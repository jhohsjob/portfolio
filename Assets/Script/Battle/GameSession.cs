public class GameSession : MonoSingleton<GameSession>
{
    public Stage currentStage { get; private set; }
    public Mercenary currentMercenary { get; private set; }

    protected override void OnAwake()
    {
    }

    public void SetStage(Stage stage)
    {
        currentStage = stage;
    }

    public void SetMercenary(Mercenary mercenary)
    {
        currentMercenary = mercenary;

        Client.user.SetMercenary(mercenary.id);
    }
}