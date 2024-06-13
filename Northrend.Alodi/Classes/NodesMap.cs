using Northrend.Alodi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Classes
{
    public class NodesMap : INodesMap
    {
        readonly List<INode> mCollection = [];
        public IEnumerable<INode> Collection => mCollection;

        public void Add(INode node)
            => mCollection.Add(node);

        public INode? GetNodeByName(string name)
            => Collection.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
