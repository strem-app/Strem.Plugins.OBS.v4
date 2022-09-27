using Microsoft.Extensions.Logging;
using Moq;
using Obs.v4.WebSocket.Reactive;
using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Processors;
using Strem.Flows.Types;
using Strem.OBS.v4.Flows.Tasks;

namespace Strem.Plugins.Obs.v4.UnitTests;

public class ChangeSceneTaskTests
{
    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public async Task should_correctly_report_if_it_can_run(bool IsConnected, bool expectedCanRunState)
    {
        var mockLogger = new Mock<ILogger<FlowTask<ChangeSceneTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var mockObsClient = new Mock<IObservableOBSWebSocket>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var dummyFlowVars = new Variables();
        
        mockObsClient.Setup(x => x.IsConnected).Returns(IsConnected);
        
        var task = new ChangeSceneTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockObsClient.Object);
        var canRun = task.CanExecute();
        Assert.Equal(expectedCanRunState, canRun);
    }
    
    [Fact]
    public async Task should_return_failure_if_data_is_missing()
    {
        var mockLogger = new Mock<ILogger<FlowTask<ChangeSceneTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var mockObsClient = new Mock<IObservableOBSWebSocket>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var dummyFlowVars = new Variables();
        var task = new ChangeSceneTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockObsClient.Object);

        var dummyData = new ChangeSceneTaskData();
        var result = await task.Execute(dummyData, dummyFlowVars);
        
        Assert.Equal(ExecutionResultType.Failed, result.ResultType);
    }
    
    [Fact]
    public async Task should_call_to_change_scene_if_data_is_valid()
    {
        var mockLogger = new Mock<ILogger<FlowTask<ChangeSceneTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var mockObsClient = new Mock<IObservableOBSWebSocket>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var dummyFlowVars = new Variables();
        var task = new ChangeSceneTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockObsClient.Object);

        var expectedScene = "expected-scene";
        var dummyData = new ChangeSceneTaskData();
        dummyData.NewScene = expectedScene;
        
        var result = await task.Execute(dummyData, dummyFlowVars);
        Assert.Equal(ExecutionResultType.Success, result.ResultType);
        mockObsClient.Verify(x => x.SetCurrentScene(expectedScene, default), Times.Once);
    }
}