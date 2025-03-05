using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using NLog;

namespace HelloGreetingApp.Controllers
{
    /// <summary>
    /// Class Providing API for HelloGreetingApp
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HelloGreetingAppController : ControllerBase
    {
        private static readonly NLog.ILogger logger = LogManager.GetCurrentClassLogger();


        private readonly IGreetingBL _greetingBL;
        public HelloGreetingAppController(IGreetingBL greetingBL)
        {
            _greetingBL = greetingBL;
        }


        /// <summary>
        /// Get method to get the greeting message
        /// </summary>
        /// <returns> "Hello World" </returns>
        [HttpGet]
        public IActionResult Get()
        {
            logger.Info("Get method called");

            ResponseModel<string> responseModel = new ResponseModel<string>
            {
                Success = true,
                Message = "Hello to creating the API endpoints",
                Data = "Hello World"
            };

            return Ok(responseModel);
        }

        /// <summary>
        /// Post method to receive a greeting name and return a personalized message
        /// </summary>
        /// <returns>A personalized greeting message</returns>
        [HttpPost]
        public IActionResult Post(RequestModel requestModel)
        {
            if (requestModel == null || !ModelState.IsValid)
            {
                logger.Warn("Invalid POST request received");
                return BadRequest("Invalid request data");
            }

            logger.Info("Post method called with Key: {0}, Value: {1}", requestModel.Key, requestModel.Value);

            ResponseModel<string> responseModel = new ResponseModel<string>
            {
                Success = true,
                Message = "Request received successfully",
                Data = $"Key: {requestModel.Key}, Value: {requestModel.Value}"
            };

            return CreatedAtAction(nameof(Get), responseModel);
        }

        /// <summary>
        /// Put method to update a greeting message
        /// </summary>
        /// <returns>Confirmation of update</returns>
        [HttpPut]
        public IActionResult Put([FromBody] RequestModel requestModel)
        {
            if (requestModel == null || !ModelState.IsValid)
            {
                logger.Warn("Invalid PUT request received");
                return BadRequest("Invalid request data");
            }

            logger.Info("Put method called with Key: {0}, Value: {1}", requestModel.Key, requestModel.Value);

            ResponseModel<string> responseModel = new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting updated successfully",
                Data = $"Updated Key: {requestModel.Key}, Updated Value: {requestModel.Value}"
            };

            return Ok(responseModel);
        }

        /// <summary>
        /// Patch method to partially update a greeting message
        /// </summary>
        /// <returns>Confirmation of partial update</returns>
        [HttpPatch]
        public IActionResult Patch([FromBody] RequestModel requestModel)
        {
            if (requestModel == null || !ModelState.IsValid)
            {
                logger.Warn("Invalid PATCH request received");
                return BadRequest("Invalid request data");
            }

            logger.Info("Patch method called with Key: {0}, Value: {1}", requestModel.Key, requestModel.Value);

            ResponseModel<string> responseModel = new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting partially updated successfully",
                Data = $"Partially Updated Key: {requestModel.Key}, Partially Updated Value: {requestModel.Value}"
            };

            return Ok(responseModel);
        }

        /// <summary>
        /// Delete method to remove a greeting message
        /// </summary>
        /// <returns>Confirmation of deletion</returns>
        [HttpDelete("{key}")]
        public IActionResult Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                logger.Warn("Invalid DELETE request with empty key");
                return BadRequest("Key cannot be null or empty");
            }

            logger.Info("Delete method called with Key: {0}", key);

            ResponseModel<string> responseModel = new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting deleted successfully",
                Data = $"Deleted Key: {key}"
            };

            return Ok(responseModel);
        }


        //UC2
        [HttpGet("Greeting")]
        public IActionResult GetGreeting()
        {
            return Ok(_greetingBL.GetGreetingBL());
        }

        //UC3
        [HttpGet("hello")]
        public IActionResult GetGreeting([FromQuery] string? firstName, [FromQuery] string? lastName)
        {

            string greetingMessage = _greetingBL.GetGreeting(firstName, lastName);
            return Ok(new { Message = greetingMessage });
        }

        //UC4

        [HttpPost]
        [Route("save")]

        public IActionResult SaveGreeting([FromBody] GreetingModel greetingModel)
        {
            var result = _greetingBL.SaveGreetingBL(greetingModel);

            var response = new ResponseModel<object>
            {
                Success = true,
                Message = "Greeting Created",
                Data = result

            };
            return Created("Greeting Created", response);

        }

        //UC5

        [HttpGet("GetGreetingById/{id}")]
        public IActionResult GetGreetingById(int id)
        {
            var response = new ResponseModel<GreetingModel>();
            try
            {
                var result = _greetingBL.GetGreetingByIdBL(id);
                if (result != null)
                {
                    response.Success = true;
                    response.Message = "Greeting Message Found";
                    response.Data = result;
                    return Ok(response);
                }
                response.Success = false;
                response.Message = "Greeting Message Not Found";
                return NotFound(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, response);
            }
        }

        //U6

        [HttpGet("GetAllGreetings")]
        public IActionResult GetAllGreetings()
        {
            ResponseModel<List<GreetingModel>> response = new ResponseModel<List<GreetingModel>>();
            try
            {
                var result = _greetingBL.GetAllGreetingsBL();
                if (result != null && result.Count > 0)
                {
                    response.Success = true;
                    response.Message = "Greeting Messages Found";
                    response.Data = result;
                    return Ok(response);
                }
                response.Success = false;
                response.Message = "No Greeting Messages Found";
                return NotFound(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, response);
            }
        }



    }
}
