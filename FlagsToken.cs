namespace BPAD
{
    internal class FlagsToken : ParsingToken
    {
        public Range OccurancesRange { get; private set; }

        public override int Precedence => 2;

        public FlagsToken(Range range)
        {
            OccurancesRange = range;
        }

        new public static FlagsToken Parse(string[] stringTokens, ref int index)
        {
            //just parsing x and : for now
            string token = stringTokens[index];
            index++;

            FlagsToken ft;

            if (token == Constants.OCCURANCES_FLAG)
            {
                ft = new FlagsToken(Range.Parse(stringTokens, ref index));
            }
            else if (token == Constants.OCCURANCES_MORE_FLAG)
            {
                //check for corresponding number
                if (float.TryParse(stringTokens[index], out float n))
                {
                    index++;

                    //has a number
                    ft = new FlagsToken(Range.More(n));
                }
                else
                {
                    //no number, use default
                    ft = new FlagsToken(Range.More());
                }
            }
            else if (token == Constants.OCCURANCES_LESS_FLAG)
            {
                //check for corresponding number
                if (float.TryParse(stringTokens[index], out float n))
                {
                    index++;

                    //has a number
                    ft = new FlagsToken(Range.Less(n));
                }
                else
                {
                    //no number, use default
                    ft = new FlagsToken(Range.Less());
                }
            }
            else if (token == Constants.OPTIONAL_FLAG)//?
            {
                ft = new FlagsToken(new Range(0, 1));
            }
            else if (token == Constants.OPTIONAL_MORE_FLAG)//*
            {
                ft = new FlagsToken(Range.More(0));
            }
            else if (token == Constants.EXCEPT_FLAG)//!
            {
                ft = new FlagsToken(Range.Zero);
            }
            else
            {
                throw new NotImplementedException($"The flag \"{stringTokens[index]}\" has not been implemented yet.");
            }

            return ft;
        }

        public override string ToString()
        {
            return $"[Flags Token: {OccurancesRange}]";
        }
    }
}
