using SalesWebMvc.Data;
using SalesWebMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.Services
{
    public class SellerService
    {
        private readonly SalesWebMvcContext _context;

        public SellerService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public List<Seller> FindAll()
        {
            return _context.Seller.ToList(); //aplicação de forma sincrona
        }

        public void Insert(Seller obj)
        {
            obj.Department = _context.Department.First(); //solução provisória, inserindo o primeiro departamento associado ao vendedor.
            _context.Add(obj);
            _context.SaveChanges();
        }
    }
}
