using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.Payments;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PaymentDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] Guid? tenantId = null, [FromQuery] string? status = null)
    {
        var result = await _service.GetAllAsync(page, pageSize, tenantId, status);
        return Ok(ApiResponse<PagedResult<PaymentDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<PaymentDto>.Fail("Payment not found."));
        return Ok(ApiResponse<PaymentDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> Create([FromBody] CreatePaymentRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.PaymentId }, ApiResponse<PaymentDto>.Ok(result, "Payment created."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<PaymentDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> Update(Guid id, [FromBody] UpdatePaymentRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<PaymentDto>.Fail("Payment not found."));
        return Ok(ApiResponse<PaymentDto>.Ok(result, "Payment updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Payment not found."));
        return Ok(ApiResponse<object>.Ok(new { }, "Payment deleted."));
    }
}
