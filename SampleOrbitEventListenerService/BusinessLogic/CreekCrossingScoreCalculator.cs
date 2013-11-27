using System;
using System.Diagnostics;
using System.Linq;
using NLog;
using SampleOrbitEventListenerService.Extensions;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.BusinessLogic
{
    class CreekCrossingScoreCalculator : TaskScoreCalculator
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        static class PropertyName
        {
            public const string Score = "CC_SCORE";
            public const string CrossingType = "CrossingType";
        }

        public override int CalculateScore(TaskResource task)
        {
            int crossingType = task.Properties.Get<int>(PropertyName.CrossingType);

            int score = 0;
            switch (crossingType)
            {
                case 0:
                    // Do not calculate a score...
                    score = ReadScore(task);
                    Log.Debug("Creek Crossing has CrossingType = NA (Not calculating score)");
                    break;
                case 1: // Aerial
                    score = CalculateScoreForCrossingTypeAerial(task);
                    Log.Debug("Calculated aerial score = {0}", score);
                    break;
                case 2: // Buried
                    score = CalculateScoreForCrossingTypeBuried(task);
                    Log.Debug("Calculated buried score = {0}", score);
                    break;
            }
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

        int CalculateScoreForCrossingTypeAerial(TaskResource task)
        {
            var weights = new int[] { 20, 10, 10, 5, 5, 5, 30, 5, 15, 10, 25 };
            Debug.Assert(weights.Length == 11, "The CC_SCORE requires 11 weight values");

            var ratings = new int[]
            {
                task.Properties.Get<int>("MaxMGDPipeSize"),
                task.Properties.Get<int>("StreamBedComposition"),
                task.Properties.Get<int>("ChannelCurvatureAtCrossing"),
                task.Properties.Get<int>("UpstreamAndDownstreamConditions"),
                task.Properties.Get<int>("WatershedInfo"),
                task.Properties.Get<int>("BankFailurePotential"),
                task.Properties.Get<int>("StructuralElementEncasementTrussFlexural"),
                task.Properties.Get<int>("FlowImpactPotential"),
                task.Properties.Get<int>("Span"),
                task.Properties.Get<int>("ScourAroundSupports"),
                task.Properties.Get<int>("ConditionOfVerticalSupports")
            };
            Debug.Assert(ratings.Length == 11, "The CC_SCORE requires 11 rating values");

            AssertSameLengthArrays(ratings, weights);
            int score = ratings.Zip(weights, (r, w) => r * w).Sum();
            return score;
        }

        int CalculateScoreForCrossingTypeBuried(TaskResource task)
        {
            var weights = new int[] { 20, 20, 20, 10, 20, 10, 10, 10, 10, 5, 5 };
            Debug.Assert(weights.Length == 11, "The CC_SCORE requires 11 weight values");

            var ratings = new int[]
            {
                task.Properties.Get<int>("MaxMGDPipeSize"),
                task.Properties.Get<int>("EncasementCondition"),
                task.Properties.Get<int>("EncasementExposure"),
                task.Properties.Get<int>("PiersEncasementSpan"),
                task.Properties.Get<int>("StreamBedComposition"),
                task.Properties.Get<int>("ChannelCurvatureAtCrossing"),
                task.Properties.Get<int>("UpstreamAndDownstreamConditions"),
                task.Properties.Get<int>("WatershedInfo"),
                task.Properties.Get<int>("StabilizationStructureChannel"),
                task.Properties.Get<int>("DegradationRate"),
                task.Properties.Get<int>("BankFailurePotential")
            };
            Debug.Assert(ratings.Length == 11, "The CC_SCORE requires 11 rating values");

            AssertSameLengthArrays(ratings, weights);
            int score = ratings.Zip(weights, (r, w) => r * w).Sum();
            return score;
        }
    }
}