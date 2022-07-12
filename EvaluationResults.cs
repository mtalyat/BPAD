namespace BPAD
{
    internal enum EvaluationResults : int
    {
        Completed,
        SteppedAndTechnicallyComplete,
        TechnicallyComplete,
        Stepped,
        Skipped,
        BeforeTime,
        NotEnoughOccurances,
        Canceled,

        Invalid,
    }

    internal static class EvaluationResultsExtensions
    {
        public static EvaluationResults Max(this EvaluationResults results, EvaluationResults other)
        {
            return results > other ? results : other;
        }

        public static EvaluationResults Min(this EvaluationResults results, EvaluationResults other)
        {
            return results < other ? results : other;
        }
    }
}
