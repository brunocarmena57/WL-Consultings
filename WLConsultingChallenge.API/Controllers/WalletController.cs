using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WLConsultingChallenge.Core.DTOs;
using WLConsultingChallenge.Core.Services;

namespace WLConsulting_Challenge.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly IUserService _userService;

    public WalletController(IWalletService walletService, IUserService userService)
    {
        _walletService = walletService;
        _userService = userService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new Exception("User not authenticated");

        return int.Parse(userIdClaim.Value);
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        try
        {
            var userId = GetCurrentUserId();
            var balance = await _walletService.GetBalance(userId);
            
            return Ok(new { balance });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (model.Amount <= 0)
            return BadRequest(new { message = "Amount must be greater than zero" });

        try
        {
            var userId = GetCurrentUserId();
            var transaction = await _walletService.Deposit(userId, model.Amount, model.Description);
            
            return Ok(new { message = "Deposit successful", newBalance = await _walletService.GetBalance(userId) });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] CreateTransactionDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (model.Amount <= 0)
            return BadRequest(new { message = "Amount must be greater than zero" });

        try
        {
            var fromUserId = GetCurrentUserId();
            
            if (fromUserId == model.ToUserId)
                return BadRequest(new { message = "Cannot transfer to yourself" });

            var toUser = await _userService.GetUserById(model.ToUserId);
            if (toUser == null)
                return BadRequest(new { message = "Recipient user not found" });

            var transaction = await _walletService.Transfer(fromUserId, model.ToUserId, model.Amount, model.Description);
            
            return Ok(new { message = "Transfer successful", newBalance = await _walletService.GetBalance(fromUserId) });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] TransactionFilterDto filter)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transactions = await _walletService.GetTransactionsByUserId(userId, filter.StartDate, filter.EndDate);
            
            var transactionDtos = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                FromUsername = t.FromWallet?.User?.Username,
                ToUsername = t.ToWallet.User.Username,
                Amount = t.Amount,
                Type = t.Type.ToString(),
                CreatedAt = t.CreatedAt,
                Description = t.Description
            });
            
            return Ok(transactionDtos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
