namespace BPAD
{
    internal class TimingToken : ParsingToken
    {
        public Range Timings { get; private set; }

        public override int Precedence => 1;

        public TimingToken(Range range)
        {
            Timings = range;
        }

        new public static TimingToken Parse(string[] stringTokens, ref int index)
        {
            if(stringTokens[index] != Constants.SECONDS_FLAG)
            {
                throw new ParsingException("Timing Token.");
            }

            //skip first bracket
            index++;

            Range range = Range.Parse(stringTokens, ref index);

            //if the range within the range is 0, we want to default to less, not minmax
            if(range.Distance == 0)
            {
                return new TimingToken(Range.Less(range.Maximum));
            } else
            {
                return new TimingToken(range);
            }
        }

        public override string ToString()
        {
            return $"[Timing Token: {Timings}]";
        }
    }
}
