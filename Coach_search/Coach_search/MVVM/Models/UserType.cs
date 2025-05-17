using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coach_search.Models
{

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public bool IsBlocked { get; set; }
    }
    public enum UserType
    {
        Client,
        Tutor,
        Admin
    }


}
