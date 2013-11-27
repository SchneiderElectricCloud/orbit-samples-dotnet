using System;
using System.Diagnostics;
using System.Linq;
using NLog;
using SampleOrbitEventListenerService.Extensions;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.BusinessLogic
{
    class RapidAssessmentScoreCalculator : TaskScoreCalculator
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        static class PropertyName
        {
            public const string Score = "Score";
        }

        public override int CalculateScore(TaskResource task)
        {
            var weights = new int[] { 25, 20, 5, 10, 5, 1, 3, 3, 3, 5 };
            Debug.Assert(weights.Length == 10, "The score requires 10 weight values");

            var ratings = new int[]
            {
                task.Properties.Get<int>("PolesBroken"),
                task.Properties.Get<int>("PrimarySpans"),
                task.Properties.Get<int>("DamagedAnchors"),
                task.Properties.Get<int>("CrossArms"),
                task.Properties.Get<int>("Transformers"),
                task.Properties.Get<int>("Fuses"),
                task.Properties.Get<int>("GangedSwitches"),
                task.Properties.Get<int>("Reclosers"),
                task.Properties.Get<int>("CapacitorBanks"),
                task.Properties.Get<int>("Trees")
            };
            Debug.Assert(ratings.Length == 10, "The score requires 10 rating values");

            AssertSameLengthArrays(ratings, weights);
            int score = ratings.Zip(weights, (r, w) => r * w).Sum();
            return score;
        }

        public override int ReadScore(TaskResource task)
        {
            return task.Properties.Get<int>(PropertyName.Score);
        }

        public override void WriteScore(TaskResource task, int score)
        {
            task.Properties[PropertyName.Score] = score;
        }
    }
}