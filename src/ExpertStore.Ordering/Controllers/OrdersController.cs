using ExpertStore.Ordering.UseCases;
using ExpertStore.SeedWork.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpertStore.Ordering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ListOrdersOutputItem>))]
        public async Task<IActionResult> List([FromServices] IUseCase<List<ListOrdersOutputItem>> useCase)
        {
            try
            {
                var orders = await useCase.Handle();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
        [HttpGet("{OrderId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDetail))]
        public async Task<IActionResult> GetOrderDetails(
            [FromRoute] GetOrderInput input, 
            [FromServices] IUseCase<GetOrderInput, OrderDetail?> useCase)
        {
            try
            {
                var order = await useCase.Handle(input);
                if(order is null)
                    return NotFound(new ProblemDetails() { Title = $"Order {input.OrderId} not found", Status = 404 });
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateOrderOutput))]
        public async Task<IActionResult> Create(
            [FromBody] CreateOrderInput input, 
            [FromServices] IUseCase<CreateOrderInput, CreateOrderOutput> useCase)
        {
            try
            {
                return Ok(await useCase.Handle(input));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
