namespace UnitWarfare.Players
{
    public interface IPlayersHandler
    {
        public event System.Action<Player> OnActivePlayerChanged;

        public Player LocalPlayer { get; }
        public Player OtherPlayer { get; }
        public Player NeutralPlayer { get; }
    }
}