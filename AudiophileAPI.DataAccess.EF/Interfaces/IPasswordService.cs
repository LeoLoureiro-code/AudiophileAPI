using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiophileAPI.DataAccess.EF.Interfaces
{
    public interface IPasswordService
    {
        public string HashPassword(string password);

        public bool VerifyPassword(string hashedPassword, string providedPassword);
    }

}
