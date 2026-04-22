using Bph;
using Cartheur.Demo;
using Xunit;

namespace Cartheur.Demo.Tests;

public class WhirlAndTasksTests
{
    public WhirlAndTasksTests()
    {
        ResetSharedState();
    }

    [Fact]
    public void ActionController_ZeroOnFirstRun_StartsSequence()
    {
        RueTheWhirl.CurrentState = "Zero";

        var result = RueTheWhirl.ActionController('0');

        Assert.Equal("Beginning my sequences.", result);
        Assert.Equal(1, RueTheWhirl.NumberOfWhirls);
    }

    [Fact]
    public void ActionController_ZeroAfterFirstRun_RestsInZeroState()
    {
        RueTheWhirl.NumberOfWhirls = 1;
        RueTheWhirl.CurrentState = "Zero";

        var result = RueTheWhirl.ActionController('0');

        Assert.Equal("Resting in my zeroth state.", result);
        Assert.Equal(1, RueTheWhirl.NumberOfWhirls);
    }

    [Fact]
    public void ActionController_UnknownState_FallsBackToZeroState()
    {
        RueTheWhirl.CurrentState = "Unknown";

        var result = RueTheWhirl.ActionController('0');

        Assert.Equal("Beginning my sequences.", result);
        Assert.Equal(1, RueTheWhirl.NumberOfWhirls);
    }

    [Fact]
    public void SpawnMatrix_WithPositiveLoops_StartsComputationTask()
    {
        var result = Tasks.SpawnMatrix(1);

        Assert.Equal("Computation running with 1 matrix iterations.", result);
        Assert.True(Tasks.ComputationTaskRunning);
    }

    [Fact]
    public void SpawnMatrix_WhenAlreadyRunning_IsIdempotent()
    {
        Tasks.ComputationTaskRunning = true;

        var result = Tasks.SpawnMatrix(1);

        Assert.Equal("Matrix computation pod already running.", result);
    }

    [Fact]
    public void ActionController_Two_SpawnsAgent()
    {
        RueTheWhirl.CurrentState = "Two";

        var result = RueTheWhirl.ActionController('0');

        Assert.Equal("An modal has been spawned.", result);
        Assert.True(Tasks.AgentRunning);
    }

    [Fact]
    public void ActionController_Three_CreatesAgentTask()
    {
        RueTheWhirl.CurrentState = "Three";

        var result = RueTheWhirl.ActionController('0');

        Assert.Equal("Agent modal completed.", result);
        Assert.True(Tasks.AgentTaskRunning);
    }

    [Fact]
    public void ActionController_Four_AdvancesWhirlCount()
    {
        RueTheWhirl.CurrentState = "Four";

        var result = RueTheWhirl.ActionController('0');

        Assert.Equal("I am now in state four on the sequence.", result);
        Assert.Equal(1, RueTheWhirl.NumberOfWhirls);
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
