using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Interfaces
{
    public interface INodes
    {
        IEnumerable<INode> Collection { get; }

        void Add(INode node);
    }
}
