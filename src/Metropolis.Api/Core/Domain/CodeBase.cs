﻿using System.Collections.Generic;
using System.Linq;
using Metropolis.Common.Models;

namespace Metropolis.Api.Core.Domain
{
    public class CodeBase
    {
        public string Name { get; set; }
        public string SourceBaseDirectory { get; set; }
        public RepositorySourceType SourceType { get; set; }
        public CodeGraph Graph { get; }
        public List<Instance> AllInstances => Graph.AllInstances;

        public CodeBase(CodeGraph graph) : this(string.Empty, graph)
        {
        }

        public CodeBase(string name, CodeGraph graph, RepositorySourceType sourceType = RepositorySourceType.CSharp)
        {
            Name = name;
            Graph = graph;
            SourceType = sourceType;
        }

        public void Enrich(CodeGraph enricher)
        {
            Enrich(enricher.AllInstances);
        }

        public void Enrich(IEnumerable<Instance> instances)
        {
            foreach (var c in instances)
            {
                Graph.Apply(c);
            }
        }

        public int NumberOfTypes => Graph.Count;
        public int LinesOfCode => AllInstances.Sum(c => c.LinesOfCode);

        public double AverageToxicity()
        {
            var sum = AllInstances.Sum(c => c.Toxicity);
            return sum / NumberOfTypes;
        }

        public Dictionary<string, IEnumerable<Instance>> ByNamespace()
        {
            return Graph.AllNamespaces.ToDictionary(ns => ns, ns => AllInstances.Where(x => x.NameSpace == ns));
        }

        public static CodeBase Empty()
        {
            return new CodeBase(new CodeGraph(new Instance[0]));
        }
    }
}