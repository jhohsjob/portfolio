public sealed class BattleRequest
{
    public Stage stage { get; }
    public Mercenary mercenary { get; }

    public BattleRequest(Stage stage, Mercenary mercenary)
    {
        this.stage = stage;
        this.mercenary = mercenary;
    }
}