using System;
using System.Collections.Generic;
using ModelLayer.Entity;
using ModelLayer.Model;

namespace BusinessLayer.Interface
{
    /// <summary>
    /// Interface defining business logic operations for greeting functionalities.
    /// </summary>
    public interface IGreetingBL
    {
        /// <summary>
        /// Returns a simple "Hello World" message.
        /// </summary>
        /// <returns>A string containing "Hello World".</returns>
        string GetGreetingBL();

        /// <summary>
        /// Generates a greeting message based on the provided first and last name.
        /// </summary>
        /// <param name="firstName">First name of the user (optional).</param>
        /// <param name="lastName">Last name of the user (optional).</param>
        /// <returns>Formatted greeting message.</returns>
        string GetGreeting(string? firstName, string? lastName);

        /// <summary>
        /// Saves a greeting message to the database.
        /// </summary>
        /// <param name="greetingModel">Greeting model containing message details.</param>
        /// <returns>Saved greeting entity.</returns>
        GreetEntity SaveGreetingBL(GreetingModel greetingModel);

        /// <summary>
        /// Retrieves a greeting message by its unique ID.
        /// </summary>
        /// <param name="Id">Unique identifier of the greeting.</param>
        /// <returns>Greeting model containing message details.</returns>
        GreetingModel GetGreetingByIdBL(int Id);

        /// <summary>
        /// Retrieves all greeting messages.
        /// </summary>
        /// <returns>List of greeting models.</returns>
        List<GreetingModel> GetAllGreetingsBL();

        /// <summary>
        /// Updates an existing greeting message.
        /// </summary>
        /// <param name="id">ID of the greeting to be updated.</param>
        /// <param name="greetingModel">Updated greeting model.</param>
        /// <returns>Updated greeting model.</returns>
        GreetingModel EditGreetingBL(int id, GreetingModel greetingModel);

        /// <summary>
        /// Deletes a greeting message by its ID.
        /// </summary>
        /// <param name="id">ID of the greeting to be deleted.</param>
        /// <returns>Boolean indicating whether deletion was successful.</returns>
        bool DeleteGreetingBL(int id);
    }
}
