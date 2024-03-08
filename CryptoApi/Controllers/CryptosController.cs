using CryptoApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CryptoApi.Data.CryptoDbContext;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CryptoApi.Controllers
{
    [Route("v1/cryptos")]
    [ApiController]
    public class CryptosController : ControllerBase
    {
        CryptoDbContext _dbContext = new CryptoDbContext();
        // GET: api/<CyrptosController>
        [HttpGet("")]
        [ResponseCache(Duration = 60)]
        public IActionResult QueryCryptos()
        {
            var cryptos = _dbContext.Cryptos;

            if(cryptos == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(cryptos);
            }
        }

        // POST api/<CyrptosController>
        [HttpPost("")]
        public async Task<IActionResult> CreateCryptos([FromBody] Crypto crypto)
        {
            if (crypto == null)
            {
                return BadRequest();
            }
            {
                _dbContext.Cryptos.Add(crypto);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
        }

        // GET api/<CyrptosController>/5
        [HttpGet("{id}")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetCrypto(int id)
        {
            var crypto = await _dbContext.Cryptos.FirstOrDefaultAsync(c => c.Id == id);
            if (id < 0)
            {
                return BadRequest();   
            }
            if (crypto == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(crypto);
            }

        }

        [HttpGet]
        [Route("paging-cryptos")]
        [ResponseCache(Duration = 60)]
        public IActionResult PagingCryptos(int pageNumber, int pageSize)
        {
            if (pageNumber < 0 || pageSize < 0)
            {
                return BadRequest();
            }
            else
            {
                var cryptos = _dbContext.Cryptos.OrderBy(c => c.Price);

                if(cryptos == null )
                {
                    return NotFound();
                }
                else
                {
                    return Ok(cryptos.Skip((pageNumber - 1) * pageSize).Take(pageSize));
                }
            }
        }

        // PUT api/<CyrptosController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCryptos(int id, [FromBody] Crypto crypto)
        {
            if (crypto == null || crypto.Price < 0 || id < 0 || string.IsNullOrWhiteSpace(crypto.Name))
            {
                return BadRequest();
            }

            var _crypto = await _dbContext.Cryptos.FirstOrDefaultAsync(crypto => crypto.Id == id);

            if (_crypto == null)
            {
                return NotFound();
            }
            _crypto.Name = crypto.Name;
            _crypto.Price = crypto.Price;
            _dbContext.SaveChanges();
            return Ok();
        }

        // DELETE api/<CyrptosController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCryptos(int id)
        {
            var _crypto = await _dbContext.Cryptos.FirstOrDefaultAsync(C => C.Id == id);

            if (_crypto == null)
            {
                return NotFound();
            }
            if(id < 0)
            {
                return BadRequest();
            }

            _dbContext.Cryptos.Remove(_crypto);
            _dbContext.SaveChanges();
            return Ok();

        }
    }
}

