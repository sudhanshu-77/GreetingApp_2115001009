using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using NLog;
using System;
using System.Collections.Generic;

namespace HelloGreetingApp.Controllers
{
    //UC1

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
        /// Get method to get the greeting message.
        /// </summary>
        /// <returns>Returns "Hello World".</returns>
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
        /// Post method to receive a greeting name and return a personalized message.
        /// </summary>
        /// <returns>Returns a personalized greeting message.</returns>
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
        /// Put method to update a greeting message.
        /// </summary>
        /// <returns>Returns confirmation of update.</returns>
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
        /// Patch method to partially update a greeting message.
        /// </summary>
        /// <returns>Returns confirmation of partial update.</returns>
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
        /// Delete method to remove a greeting message.
        /// </summary>
        /// <returns>Returns confirmation of deletion.</returns>
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

        /// <summary>
        /// Get method to retrieve a generic greeting message.
        /// </summary>
        /// <returns>Returns a generic greeting message.</returns>
        [HttpGet("Greeting")]
        public IActionResult GetGreeting()
        {
            logger.Info("GetGreeting method called");
            return Ok(_greetingBL.GetGreetingBL());
        }

        //UC3

        /// <summary>
        /// Get method to retrieve a personalized greeting message.
        /// </summary>
        /// <param name="firstName">The first name of the person to greet.</param>
        /// <param name="lastName">The last name of the person to greet.</param>
        /// <returns>Returns a personalized greeting message.</returns>
        [HttpGet("hello")]
        public IActionResult GetGreeting([FromQuery] string? firstName, [FromQuery] string? lastName)
        {
            logger.Info("GetGreeting method called with firstName: {0}, lastName: {1}", firstName, lastName);
            string greetingMessage = _greetingBL.GetGreeting(firstName, lastName);
            return Ok(new { Message = greetingMessage });
        }

        //UC4

        /// <summary>
        /// Post method to save a new greeting message.
        /// </summary>
        /// <param name="greetingModel">The greeting model containing the message details.</param>
        /// <returns>Returns a confirmation of the created greeting.</returns>
        [HttpPost]
        [Route("save")]
        public IActionResult SaveGreeting([FromBody] GreetingModel greetingModel)
        {
            logger.Info("SaveGreeting method called with GreetingModel: {0}", greetingModel);
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

        /// <summary>
        /// Get method to retrieve a greeting message by its ID.
        /// </summary>
        /// <param name="id">The ID of the greeting message.</param>
        /// <returns>Returns the greeting message if found, otherwise a not found response.</returns>
        [HttpGet("GetGreetingById/{id}")]
        public IActionResult GetGreetingById(int id)
        {
            logger.Info("GetGreetingById method called with id: {0}", id);
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
                logger.Error(ex, "An error occurred in GetGreetingById");
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, response);
            }
        }

        //UC6

        /// <summary>
        /// Get method to retrieve all greeting messages.
        /// </summary>
        /// <returns>Returns a list of all greeting messages.</returns>
        [HttpGet("GetAllGreetings")]
        public IActionResult GetAllGreetings()
        {
            logger.Info("GetAllGreetings method called");
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
                logger.Error(ex, "An error occurred in GetAllGreetings");
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, response);
            }
        }

        //UC7

        /// <summary>
        /// Put method to update an existing greeting message.
        /// </summary>
        /// <param name="id">The ID of the greeting message to update.</param>
        /// <param name="greetModel">The updated greeting model.</param>
        /// <returns>Returns a confirmation of the updated greeting message.</returns>
        [HttpPut("EditGreeting/{id}")]
        public IActionResult EditGreeting(int id, GreetingModel greetModel)
        {
            logger.Info("EditGreeting method called with id: {0}, GreetingModel: {1}", id, greetModel);
            ResponseModel<GreetingModel> response = new ResponseModel<GreetingModel>();
            try
            {
                var result = _greetingBL.EditGreetingBL(id, greetModel);
                if (result != null)
                {
                    response.Success = true;
                    response.Message = "Greeting Message Updated Successfully";
                    response.Data = result;
                    return Ok(response);
                }
                response.Success = false;
                response.Message = "Greeting Message Not Found";
                return NotFound(response);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred in EditGreeting");
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, response);
            }
        }

        //UC8

        /// <summary>
        /// Delete method to remove a greeting message by its ID.
        /// </summary>
        /// <param name="id">The ID of the greeting message to delete.</param>
        /// <returns>Returns a confirmation of the deleted greeting message.</returns>
        [HttpDelete("DeleteGreeting/{id}")]
        public IActionResult DeleteGreeting(int id)
        {
            logger.Info("DeleteGreeting method called with id: {0}", id);
            ResponseModel<string> response = new ResponseModel<string>();
            try
            {
                bool result = _greetingBL.DeleteGreetingBL(id);
                if (result)
                {
                    response.Success = true;
                    response.Message = "Greeting Message Deleted Successfully";
                    return Ok(response);
                }
                response.Success = false;
                response.Message = "Greeting Message Not Found";
                return NotFound(response);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred in DeleteGreeting");
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, response);
            }
        }
    }
}
