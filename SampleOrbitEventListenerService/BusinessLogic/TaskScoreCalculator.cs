using System;
using System.Diagnostics;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.BusinessLogic
{
    abstract class TaskScoreCalculator
    {
        public abstract int CalculateScore(TaskResource task);
        public abstract int ReadScore(TaskResource task);
        public abstract void WriteScore(TaskResource task, int score);

        protected static void AssertSameLengthArrays(int[] ratings, int[] weights)
        {
            Debug.Assert(ratings.Length == weights.Length, "The ratings and weights arrays must have equal length");
        }
    }
}