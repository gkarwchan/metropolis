﻿using System.Collections.Generic;
using System.Linq;
using Metropolis.Api.Domain;
using Metropolis.Api.Extensions;
using Metropolis.Api.Parsers.XmlReaders.CheckStyles.Readers;

namespace Metropolis.Api.Parsers.XmlReaders.CheckStyles
{
    public interface ICheckStylesClassBuilder
    {
        Instance Build(string key, List<CheckStylesItem> cls);
    }

    public abstract class BaseCheckStylesClassBuilder : ICheckStylesClassBuilder
    {
        protected readonly IEnumerable<ICheckStylesClassParser> ClassParsers;
        protected readonly IEnumerable<ICheckStylesMemberParser> MemberParsers;

        protected BaseCheckStylesClassBuilder(IEnumerable<ICheckStylesClassParser> classParsers, IEnumerable<ICheckStylesMemberParser> memberParsers)
        {
            ClassParsers = classParsers;
            MemberParsers = memberParsers;
        }

        public Instance Build(string key, List<CheckStylesItem> items)
        {
            var grouped = (from item in items
                           group item by new {item.Line, item.Column } into grp
                           select new { grp.Key, Metrics = grp }).ToList();

            var members = grouped.Select(each => {
                var member = new Member("unknown", 0, 0, 0);

                (from p in MemberParsers
                 join item in each.Metrics
                   on p.Source equals item.Source
                 select new { Parser = p, Item = item }
                ).ForEach(e => e.Parser.Parse(member, e.Item));

                return member;
            }).ToList();

            var type = ParseClass(key, members);

             //add class level metrics
            grouped.ForEach(each => {

                (from p in ClassParsers
                 join item in each.Metrics
                   on p.Source equals item.Source
                 select new { Parser = p, Item = item }
                ).ForEach(e => e.Parser.Parse(type, e.Item));
            });

            return type;
        }

        private static Instance ParseClass(string key, IEnumerable<Member> members)
        {
            var parts = key.Split('\\').ToList();
            var name = parts.Last();
            parts.RemoveRange(parts.Count - 1, 1);
            return new Instance(string.Join("\\", parts), name, members);
        }
    }
}