using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SportsStore.Domain.Concrete;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Models
{
    public class RoleEditModel
    {
        public EfRole Role { get; set; }
        public IEnumerable<User> Members { get; set; }
        public IEnumerable<User> NonMembers { get; set; }
    }
}