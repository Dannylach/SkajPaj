using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF.Objects
{
    public class SimpleResponse
    {
        
    }

    public class CreateAccountsRequest
    {
        public string message;
        public bool create_account;
    }

    public class SignInRequest
    {
        public string message;
        public bool is_sign_in;
    }
}
