using CryptoApi.Controllers.Models;
using CryptoApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static CryptoApi.Data.CryptoDbContext;

namespace CryptoApi.Controllers
{
    [Route("v1/wallet")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        CryptoDbContext _context = new CryptoDbContext();

        [Authorize]
        [ResponseCache(Duration = 60)]
        [HttpGet("")]
        public async Task<IActionResult> GetItemsSorted(string sortType)
        {
            IQueryable<Wallet> itemsOrdered;
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return BadRequest("Invalid action");
            }
            else
            {
                var itemsInWallet = _context.Wallets.Where(w => w.UserId == user.Id);  
                if (itemsInWallet == null)
                {
                    return BadRequest("Item not found");
                }
                else
                {
                    itemsOrdered = sortType switch
                    {
                       "individualPrice" => itemsInWallet.OrderBy(c => c.Price),
                       "totalPrice" => itemsInWallet.OrderBy(c => c.TotalPrice),
                       _ => itemsInWallet.OrderBy(c => c.Price),
                    };
                }
                return Ok(itemsOrdered);
            }
        }
        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> CreateItems([FromBody] CreateBuyRequest request) 
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Name == request.Name);

            if (crypto == null || user == null)
            {
                return NotFound("Invalid action");
            }
            else
            {
                var itemsInWallet = await _context.Wallets.FirstOrDefaultAsync(i => i.CryptoId == crypto.Id && i.UserId == user.Id);

                if (itemsInWallet != null)
                {
                     itemsInWallet.Quantity += request.Quantity;
                     itemsInWallet.TotalPrice = itemsInWallet.Price * itemsInWallet.Quantity;
                }
                else
                {
                    Wallet itemInWallet = new()
                    {
                        CryptoId = crypto.Id,
                        Price = crypto.Price,
                        Quantity = request.Quantity,
                        TotalPrice = crypto.Price,
                        UserId = user.Id,
                    };
                    _context.Wallets.Add(itemInWallet);

                }
                await _context.SaveChangesAsync();
                return Ok(_context.Wallets);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItems(int id)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            var itemInWallet = await _context.Wallets.FirstOrDefaultAsync(i => i.Id == id);

            if (itemInWallet == null)
            {
                return NotFound();
            }
            if (id < 0)
            {
                return BadRequest();
            }
            else
            {
                var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Id == itemInWallet.CryptoId);
                if (crypto == null)
                {
                    return NotFound();
                }
                else
                {
                    var cryptoName = crypto.Name;

                    if (user == null)
                    {
                        return NotFound("Invalid action");
                    }
                    else
                    {
                        if (itemInWallet.Quantity > 1)
                        {
                            itemInWallet.Quantity -= 1;
                            itemInWallet.TotalPrice -= crypto.Price;
                        }
                        else
                        {
                            _context.Wallets.Remove(itemInWallet);
                        }

                        await _context.SaveChangesAsync();
                        return Ok($"{cryptoName} deleted");
                    }
                }
            }
        }

    }


}
