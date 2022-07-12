namespace BPAD
{
    internal class ConditionalOr : Conditional
    {
        public ConditionalOr(Element left, Element right) : base(left, right)
        {
        }

        internal override bool IsCompleted()
        {
            if (Occurances != null)
            {
                return Occurances.Contains(count);
            }

            return _left.IsCompleted() || _right.IsCompleted();
        }

        protected override EvaluationResults OnEvaluate(EvaluationArgs args)
        {
            return _left.Evaluate(args).Min(_right.Evaluate(args));
        }

        protected override string GetString()
        {
            return $"{Constants.GROUP_OPEN}{_left} | {_right}{Constants.GROUP_CLOSE}";
        }
    }
}
