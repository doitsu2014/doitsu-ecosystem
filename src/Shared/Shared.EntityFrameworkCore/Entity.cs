using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Shared.EntityFrameworkCore
{
    public abstract class Entity<T>
    {
        public T Id { get; set; }
    } 
}