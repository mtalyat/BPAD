namespace BPAD
{
    internal class BehaviorToken : ParsingToken
    {
        public string Name { get; private set; }

        public BehaviorToken(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"[Behavior Token: {Name}]";
        }
    }
}
