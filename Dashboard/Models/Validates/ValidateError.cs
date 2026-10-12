using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Models.Validates
{
    public class ValidateError
    {
        public string Id { get; set; }

        public string Name => Id + "Error";

        public string Description { get; set; }
    }
}