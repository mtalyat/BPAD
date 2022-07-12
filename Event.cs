using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAD
{
    internal class Event
    {
        public Behavior Behavior { get; private set; }

        public float Time { get; private set; }

        public Event(string name, float time)
        {
            Behavior = new Behavior(name);
            Time = time;
        }

        public Event(string name, string time) : this(name, float.Parse(time))
        {

        }
    }
}
