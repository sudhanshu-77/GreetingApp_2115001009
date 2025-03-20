using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Entity;
using ModelLayer.Model;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        UserEntity RegistrationBL(RegisterModel registerDTO);
        (UserEntity user, string token) LoginnUserBL(LoginModel loginDTO);

        public bool ValidateEmail(string email);

        public bool UpdateUserPassword(string email, string newPassword);

        public UserEntity GetByEmail(string email);
    }
}