using Employee_Management.Interface;
using Employee_Management.Models;
using System.Collections.Generic;
using System.Linq;

namespace Employee_Management.Repository
{
    public class DesignationRepository : IDesignation
    {
        private readonly AppDBContext _context;

        public DesignationRepository(AppDBContext context)
        {
            _context = context;
        }

        public IEnumerable<Designation> GetAllDesignations()
        {
            return _context.Designations.ToList();
        }

        public Designation GetDesignationById(int id)
        {
            return _context.Designations.Find(id);
        }

        public void AddDesignation(Designation designation)
        {
            _context.Designations.Add(designation);
            _context.SaveChanges();
        }

        public bool UpdateDesignation(Designation designation)
        {
            var existingDesignation = _context.Designations.Find(designation.DesignationId);
            if (existingDesignation == null)
            {
                return false; // Designation not found
            }

            _context.Entry(existingDesignation).CurrentValues.SetValues(designation);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteDesignation(int id)
        {
            var designation = _context.Designations.Find(id);
            if (designation == null)
            {
                return false; // Designation not found
            }

            _context.Designations.Remove(designation);
            _context.SaveChanges();
            return true;
        }
    }
}
