using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Models.HtmlHelpers
{
    public class ControlSharedViewModelBase
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public int Length { get; set; }
        public int Col { get; set; }
        public int Row { get; set; }
    }
}