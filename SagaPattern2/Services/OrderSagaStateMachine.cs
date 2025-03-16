using Microsoft.EntityFrameworkCore;
using SagaPattern2.Data;
using SagaPattern2.Models;

namespace SagaPattern2.Services
{
    // Services/OrderSagaStateMachine.cs
    public class OrderSagaStateMachine : ISagaStateMachine
    {
        private readonly ApplicationDbContext _context;

        public OrderSagaStateMachine(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> StartSagaAsync(int orderId)
        {
            try
            {
                var saga = new OrderSaga
                {
                    OrderId = orderId,
                    State = "PaymentPending", // Initial state
                    CreatedDate = DateTime.Now
                };
                _context.OrderSagas.Add(saga);
                await _context.SaveChangesAsync();
                return true;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return false;
            }
        }

        public async Task<bool> ExecuteNextStepAsync(int orderId)
        {
            try
            {

                //var saga = await _context.OrderSagas.FindAsync(orderId);
                var saga = await _context.OrderSagas.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (saga == null) return false;

                switch (saga.State)
                {
                    case "PaymentPending":
                        var paymentSuccess = await ProcessPaymentAsync(orderId);
                        if (paymentSuccess)
                        {
                            saga.State = "InventoryReserved"; // Move to next state
                            await _context.SaveChangesAsync();
                            await ExecuteNextStepAsync(orderId); // Move to next step
                        }
                        else
                        {
                            saga.State = "PaymentFailed"; // If payment fails
                            await _context.SaveChangesAsync();
                            await CompensateAsync(orderId); // Compensate
                            return false;
                        }
                        break;
                    case "InventoryReserved":
                        var inventorySuccess = await ReserveInventoryAsync(orderId);
                        if (inventorySuccess)
                        {
                            saga.State = "Shipped"; // Move to next step
                            await _context.SaveChangesAsync();
                            await ExecuteNextStepAsync(orderId);
                        }
                        else
                        {
                            saga.State = "InventoryFailed";
                            await _context.SaveChangesAsync();
                            await CompensateAsync(orderId); // Compensate
                            return false;
                        }
                        break;
                    case "Shipped":
                        return true;
                    // Add more steps for shipping, etc.

                }
                return false;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return false;
            }
        }

        public async Task CompensateAsync(int orderId)
        {
            //var saga = await _context.OrderSagas.FindAsync(orderId);
            var saga = await _context.OrderSagas.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (saga == null) return;

            switch (saga.State)
            {
                case "PaymentFailed":
                    await RefundPaymentAsync(orderId);
                    break;
                case "InventoryFailed":
                    await ReleaseInventoryAsync(orderId);
                    break;
            }
        }

        private Task<bool> ProcessPaymentAsync(int orderId) => Task.FromResult(true); // Simulated payment
        private Task<bool> ReserveInventoryAsync(int orderId) => Task.FromResult(true); // Simulated inventory reservation
        private Task RefundPaymentAsync(int orderId) => Task.CompletedTask; // Simulated refund
        private Task ReleaseInventoryAsync(int orderId) => Task.CompletedTask; // Simulated inventory release
    }

}
