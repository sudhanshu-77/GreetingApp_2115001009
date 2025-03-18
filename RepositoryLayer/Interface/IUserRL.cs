using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        UserEntity Registration(RegisterModel registerModel);
        UserEntity LoginnUserRL(LoginModel loginModel);
        public bool ValidateEmail(string email);
        public UserEntity FindByEmail(string email);
        public bool Update(UserEntity user);
    }
}