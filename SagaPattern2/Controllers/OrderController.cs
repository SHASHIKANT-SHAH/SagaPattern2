using Microsoft.AspNetCore.Mvc;
using SagaPattern2.Services;

namespace SagaPattern2.Controllers
{
    // Controllers/OrderController.cs
    public class OrderController : Controller
    {
        private readonly ISagaStateMachine _sagaStateMachine;

        public OrderController(ISagaStateMachine sagaStateMachine)
        {
            _sagaStateMachine = sagaStateMachine;
        }

        //[HttpPost]
        public async Task<IActionResult> ProcessOrder(int orderId)
        {
           var startStatus = await _sagaStateMachine.StartSagaAsync(orderId); // Start the saga
            if (startStatus == false) return View("ProcessOrderFailed");

           var processStatus =  await _sagaStateMachine.ExecuteNextStepAsync(orderId); // Proceed with the next step (Payment, Inventory, Shipping)
           if (processStatus == false) return View("PaymentFailed");

            return View();
        }
    }

}
