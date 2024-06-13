using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Interfaces
{
    public interface INodesMap
    {
        IEnumerable<INode> Collection { get; }

        void Add(INode node);

        INode? GetNodeByName(string name);
    }
}
