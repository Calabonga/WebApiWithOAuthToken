using System.Collections.Generic;
using System.Security.Claims;

namespace WebApplication1.Models {
    /// <summary>
    /// User validated because founed in the user list
    /// </summary>
    public class ValidatedUser {

        public string UserName { get; set; }

        public IEnumerable<Claim> Claims { get; set; }
    }
}