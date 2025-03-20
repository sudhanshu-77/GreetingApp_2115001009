
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Middleware.GlobalExceptionHandler;
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

        // UC2

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

        [HttpPost]
        [Route("save")]

        public IActionResult SaveGreeting([FromBody] GreetingModel greetingModel)
        {
            var result = _greetingBL.SaveGreetingBL(greetingModel);
            if (result == null)
            {
                var response1 = new ResponseModel<object>
                {
                    Success = false,
                    Message = "Unable to save Greeting. Please verify that the user exists. !",
                    Data = result

                };
                return BadRequest(response1);

            }
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
            try
            {
                var entity = _greetingBL.GetGreetingByIdBL(id);
                if (entity != null)
                {
                    var model = new GreetingModel
                    {
                        Id = entity.Id,
                        Message = entity.Message,
                        Uid = entity.Uid
                    };
                    return Ok(new ResponseModel<GreetingModel>
                    {
                        Success = true,
                        Message = "Greeting Message Found",
                        Data = model
                    });
                }
                return NotFound(new ResponseModel<GreetingModel>
                {
                    Success = false,
                    Message = "Greeting Message Not Found"
                });
            }
            catch (Exception ex)
            {
                var errorResponse = ExceptionHandler.CreateErrorResponse(ex);
                return StatusCode(500, errorResponse);
            }
        }



        //UC6


        [HttpGet("GetAllGreetings")]
        public IActionResult GetAllGreetings()
        {
            try
            {
                var entities = _greetingBL.GetAllGreetingsBL();
                if (entities != null && entities.Count > 0)
                {
                    var models = entities.Select(entity => new GreetingModel
                    {
                        Id = entity.Id,
                        Message = entity.Message,
                        Uid = entity.Uid
                    }).ToList();

                    return Ok(new ResponseModel<List<GreetingModel>>
                    {
                        Success = true,
                        Message = "Greeting Messages Found",
                        Data = models
                    });
                }
                return NotFound(new ResponseModel<List<GreetingModel>>
                {
                    Success = false,
                    Message = "No Greeting Messages Found"
                });
            }
            catch (Exception ex)
            {
                var errorResponse = ExceptionHandler.CreateErrorResponse(ex);
                return StatusCode(500, errorResponse);
            }
        }


        //UC7

        [HttpPut("EditGreeting/{id}")]
        public IActionResult EditGreeting(int id, [FromBody] GreetingModel greetModel)
        {
            try
            {
                var entity = _greetingBL.EditGreetingBL(id, greetModel);
                if (entity == null)
                {
                    return BadRequest(new ResponseModel<object>
                    {
                        Success = false,
                        Message = $"No Greeting found with ID {id} to update!"
                    });
                }

                var updatedModel = new GreetingModel
                {
                    Id = entity.Id,
                    Message = entity.Message,
                    Uid = entity.Uid
                };

                return Ok(new ResponseModel<GreetingModel>
                {
                    Success = true,
                    Message = "Greeting Message Updated Successfully",
                    Data = updatedModel
                });
            }
            catch (Exception ex)
            {
                var errorResponse = ExceptionHandler.CreateErrorResponse(ex);
                return StatusCode(500, errorResponse);
            }
        }


        //UC8

        [HttpDelete("DeleteGreeting/{id}")]
        public IActionResult DeleteGreeting(int id)
        {
            try
            {
                bool result = _greetingBL.DeleteGreetingBL(id);
                if (result)
                {
                    return Ok(new ResponseModel<string>
                    {
                        Success = true,
                        Message = "Greeting Message Deleted Successfully"
                    });
                }
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Greeting Message Not Found"
                });
            }
            catch (Exception ex)
            {
                var errorResponse = ExceptionHandler.CreateErrorResponse(ex);
                return StatusCode(500, errorResponse);
            }
        }
    }
}
