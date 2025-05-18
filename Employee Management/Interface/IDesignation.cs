using Employee_Management.Models;
using System.Collections.Generic;

namespace Employee_Management.Interface
{
    public interface IDesignation
    {
        IEnumerable<Designation> GetAllDesignations();
        Designation GetDesignationById(int id);
        void AddDesignation(Designation designation);
        bool UpdateDesignation(Designation designation);
        bool DeleteDesignation(int id);
    }
}
