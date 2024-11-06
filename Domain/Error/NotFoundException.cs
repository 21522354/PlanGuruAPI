﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Error
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("Can't found this entity")
        {
            
        }
    }
}
