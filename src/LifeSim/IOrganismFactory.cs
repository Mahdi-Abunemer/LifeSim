using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeSim
{
    public interface IOrganismFactory
    {
        Organism Create(World world, Point2 position);
    }
}
