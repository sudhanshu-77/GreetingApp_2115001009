using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        UserEntity RegistrationBL(RegisterModel registerModel);
        (UserEntity user, string token) LoginnUserBL(LoginModel loginModel);

        public bool ValidateEmail(string email);

        public bool UpdateUserPassword(string email, string newPassword);

        public UserEntity GetByEmail(string email);
    }
}