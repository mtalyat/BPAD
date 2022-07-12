namespace BPAD
{
    internal class BracketToken : ParsingToken
    {
        public bool IsOpening { get; private set; }
        public bool IsClosing => !IsOpening;

        public BracketToken(bool isOpen)
        {
            IsOpening = isOpen;
        }

        public override string ToString()
        {
            return $"[Bracket Token: {(IsOpening ? "Open" : "Close")}]";
        }
    }
}
