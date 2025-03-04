using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Entity;
using ModelLayer.Model;

namespace RepositoryLayer.Interface
{
    public interface IGreetingRL
    {
        GreetEntity SaveGreetingRL(GreetingModel greetingModel);
    }
}
