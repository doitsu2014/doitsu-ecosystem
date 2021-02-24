﻿using System.ComponentModel;
using Shared.EntityFrameworkCore;

namespace FileConversion.Abstraction.Model
{
    public class OutputMapping : Entity<InputType>
    {
        public string XmlConfiguration { get; set; }
        [DefaultValue(0)]
        public int NumberOfHeader { get; set; }
        [DefaultValue(0)]
        public int NumberOfFooter { get; set; }
    }
}
