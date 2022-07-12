namespace BPAD
{
    internal class OperatorToken : ParsingToken
    {
        public string Operator { get; private set; }

        public override int Precedence => 1;

        public OperatorToken(string o)
        {
            Operator = o;
        }

        public override string ToString()
        {
            return $"[Operator Token: {Operator}]";
        }
    }
}
