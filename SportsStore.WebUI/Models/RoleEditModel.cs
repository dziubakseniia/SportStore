﻿using System.Collections.Generic;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Identity.Concrete;

namespace SportsStore.WebUI.Models
{
    /// <summary>
    /// Manages Role editing.
    /// </summary>
    public class RoleEditModel
    {
        public EfRole Role { get; set; }
        public IEnumerable<User> Members { get; set; }
        public IEnumerable<User> NonMembers { get; set; }
    }
}