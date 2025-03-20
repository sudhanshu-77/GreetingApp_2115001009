using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Entity;
using ModelLayer.Model;

namespace RepositoryLayer.Interface
{
    public interface IGreetingRL
    {
        /// <summary>
        /// Saves a new greeting to the repository.
        /// </summary>
        /// <param name="greetingModel">The model containing the greeting details to be saved.</param>
        /// <returns>Returns the saved GreetEntity object.</returns>
        GreetEntity SaveGreetingRL(GreetingModel greetingModel);

        /// <summary>
        /// Retrieves a greeting by its ID.
        /// </summary>
        /// <param name="Id">The ID of the greeting to retrieve.</param>
        /// <returns>Returns the GreetingModel associated with the specified ID.</returns>
        GreetingModel GetGreetingByIdRL(int Id);

        /// <summary>
        /// Retrieves all greetings from the repository.
        /// </summary>
        /// <returns>Returns a list of all GreetEntity objects.</returns>
        List<GreetEntity> GetAllGreetingsRL();

        /// <summary>
        /// Edits an existing greeting in the repository.
        /// </summary>
        /// <param name="id">The ID of the greeting to edit.</param>
        /// <param name="greetingModel">The model containing the updated greeting details.</param>
        /// <returns>Returns the updated GreetEntity object.</returns>
        GreetEntity EditGreetingRL(int id, GreetingModel greetingModel);

        /// <summary>
        /// Deletes a greeting from the repository by its ID.
        /// </summary>
        /// <param name="id">The ID of the greeting to delete.</param>
        /// <returns>Returns true if the deletion was successful; otherwise, false.</returns>
        bool DeleteGreetingRL(int id);
    }
}
