﻿using System;
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

        GreetingModel GetGreetingByIdRL(int Id);


        List<GreetEntity> GetAllGreetingsRL();

        GreetEntity EditGreetingRL(int id, GreetingModel greetingModel);


      bool DeleteGreetingRL(int id);


    }
}
