using Northrend.Alodi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Classes
{
    public class NodesMap : INodes
    {
        readonly List<INode> mCollection = [];
        public IEnumerable<INode> Collection => mCollection;

        public void Add(INode node)
            => mCollection.Add(node);
    }
}
