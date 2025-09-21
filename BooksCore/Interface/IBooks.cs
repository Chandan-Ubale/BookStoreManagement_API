using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books_Core.Interface
{
    public interface IBooks
    {
        string? Id { get; set; }
        string? Title { get; set; }
        string? Author { get; set; }
        decimal Price { get; set; }
    }
}
