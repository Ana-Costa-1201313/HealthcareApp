using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Domain.OperationTypes
{
    [NotMapped]
    public class RequiredStaffDto
    {
        public string Specialization { get; set; }
        public int Total { get; set; }
  
    }
}