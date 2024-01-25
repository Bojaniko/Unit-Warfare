namespace UnitWarfare.Players
{
    public interface IPlayerHandler
    {
        public delegate void PlayerEventHandler(Player player);

        public event PlayerEventHandler OnActivePlayerChanged;
    }
}