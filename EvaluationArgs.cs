namespace BPAD
{
    internal class EvaluationArgs
    {
        public int eIndex;
        
        public Event e;

        public float lastTime;

        public float currentTime => e.Time;

        public List<int> indices { get; private set; }

        public EvaluationArgs(float time, List<int> inds)
        {
            eIndex = -1;
            e = new Event("", time);
            lastTime = time;
            indices = inds;
        }
    }
}
