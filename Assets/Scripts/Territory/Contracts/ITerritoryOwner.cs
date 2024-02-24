namespace UnitWarfare.Territories
{
    public interface ITerritoryOwner
    {
        public string Name { get; }

        public bool IsNeutral { get; }
    }
}