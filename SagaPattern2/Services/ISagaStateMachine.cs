namespace SagaPattern2.Services
{
    // Services/ISagaStateMachine.cs
    public interface ISagaStateMachine
    {
        Task<bool> StartSagaAsync(int orderId);
        Task<bool> ExecuteNextStepAsync(int orderId);
        Task CompensateAsync(int orderId);
    }

}
