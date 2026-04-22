using System.Diagnostics;
using Bph;
using Cartheur.Demo;
using Xunit;

namespace Cartheur.Demo.Tests;

public class KpiAssertionsTests
{
    [Fact]
    public void DecisionPath_IsConsistentAcrossRuns()
    {
        const int runs = 50;
        var baseline = ExecuteDecisionPath();

        for (var i = 0; i < runs; i++)
        {
            var current = ExecuteDecisionPath();
            Assert.Equal(baseline, current);
        }
    }

    [Fact]
    public void DecisionPath_LatencyUnder250ms()
    {
        var stopwatch = Stopwatch.StartNew();
        _ = ExecuteDecisionPath();
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 250,
            $"Decision path latency exceeded threshold: {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void RestState_DoesNotTriggerAgentActions()
    {
        ResetSharedState();

        for (var i = 0; i < 25; i++)
        {
            RueTheWhirl.CurrentState = "Zero";
            _ = RueTheWhirl.ActionController('0');
        }

        Assert.False(Tasks.AgentRunning);
        Assert.False(Tasks.AgentTaskRunning);
    }

    private static string ExecuteDecisionPath()
    {
        ResetSharedState();

        RueTheWhirl.CurrentState = "Zero";
        var s0 = RueTheWhirl.ActionController('0');

        RueTheWhirl.CurrentState = "One";
        var s1 = RueTheWhirl.ActionController(1);

        RueTheWhirl.CurrentState = "Two";
        var s2 = RueTheWhirl.ActionController('0');

        RueTheWhirl.CurrentState = "Three";
        var s3 = RueTheWhirl.ActionController('0');

        return string.Join("|", s0, s1, s2, s3);
    }

    private static void ResetSharedState()
    {
        RueTheWhirl.NumberOfWhirls = 0;
        RueTheWhirl.CurrentState = "Zero";

        Tasks.ComputationTaskRunning = false;
        Tasks.PrometheusPodTaskRunning = false;
        Tasks.AgentRunning = false;
        Tasks.AgentTaskRunning = false;
    }
}
