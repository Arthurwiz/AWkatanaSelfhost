using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.ComponentModel.DataAnnotations;

namespace AWKatanaSelfHost.Controllers
{
    [Authorize]
    public class FruitsController : ApiController
    {
        public IEnumerable<Fruit> Get()
        {
            var list = new List<Fruit>();

            list.Add(new Fruit { Id = 1, Name = "Apple" });
            list.Add(new Fruit { Id = 2, Name = "Banana" });
            list.Add(new Fruit { Id = 3, Name = "Grape" });
            list.Add(new Fruit { Id = 4, Name = "Strawberry" });
            list.Add(new Fruit { Id = 5, Name = "Orange" });
            list.Add(new Fruit { Id = 6, Name = "Watermelon" });

            return list;
        }

    }

    public class Fruit
    {
        // Add Key Attribute: Add Reference to the project: System.ComponentModel.DataAnnotations
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
