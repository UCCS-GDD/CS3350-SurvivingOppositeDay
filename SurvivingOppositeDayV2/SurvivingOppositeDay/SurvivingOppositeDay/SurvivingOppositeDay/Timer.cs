using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class Timer
    {
        TimeSpan duration;
        TimeSpan elapsed;
        public bool Running { get; private set; }
        public event Action OnExpire;

        public void Update(TimeSpan timeStep)
        {
            // Updates the elapsed time passed
            if (Running)
            {
                elapsed += timeStep;
                if (elapsed >= duration)
                {
                    Running = false;
                    if (OnExpire != null)
                        OnExpire();
                } 
            }
        }

        public void Start(TimeSpan maxDuration)
        {
            // Sets duration
            duration = maxDuration;
            elapsed = TimeSpan.Zero;
            Running = true;
        }

        public void Extend(TimeSpan amount)
        {
            if (Running)
            {
                duration += amount;
            }
        }

        public void Shorten(TimeSpan amount)
        {
            if (Running)
            {
                duration -= amount;
                Update(TimeSpan.Zero);
            }
        }
    }
}
