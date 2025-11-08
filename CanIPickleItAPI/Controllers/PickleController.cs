using Microsoft.AspNetCore.Mvc;
using CanIPickleItAPI.Services;

namespace CanIPickleItAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PickleController : ControllerBase
    {
        private readonly IPickleCheckerService _pickleCheckerService;
        private readonly ILogger<PickleController> _logger;

        public PickleController(IPickleCheckerService pickleCheckerService, ILogger<PickleController> logger)
        {
            _pickleCheckerService = pickleCheckerService;
            _logger = logger;
        }

        /// <summary>
        /// Check if an item can be pickled
        /// </summary>
        /// <param name="request">The item to check</param>
        /// <returns>Whether the item can be pickled and the reason</returns>
        [HttpPost("can-pickle")]
        public async Task<ActionResult<PickleCheckResult>> CanPickle([FromBody] PickleCheckRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Item))
            {
                return BadRequest("Item cannot be empty or null");
            }

            _logger.LogInformation("Checking if {Item} can be pickled", request.Item);

            var result = await _pickleCheckerService.CanPickleAsync(request.Item);
            
            _logger.LogInformation("Pickle check result for {Item}: {CanPickle}", request.Item, result.CanPickle);

            return Ok(result);
        }

        /// <summary>
        /// Check if an item can be pickled using query parameter
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>Whether the item can be pickled and the reason</returns>
        [HttpGet("can-pickle")]
        public async Task<ActionResult<PickleCheckResult>> CanPickleGet([FromQuery] string item)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                return BadRequest("Item parameter cannot be empty or null");
            }

            _logger.LogInformation("Checking if {Item} can be pickled", item);

            var result = await _pickleCheckerService.CanPickleAsync(item);
            
            _logger.LogInformation("Pickle check result for {Item}: {CanPickle}", item, result.CanPickle);

            return Ok(result);
        }
    }

    public class PickleCheckRequest
    {
        public string Item { get; set; } = string.Empty;
    }
}