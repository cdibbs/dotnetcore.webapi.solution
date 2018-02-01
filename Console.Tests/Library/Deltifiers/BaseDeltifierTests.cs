using System;
using System.Collections.Generic;
using System.Text;
using Console.Library.Deltifiers;
using Xunit;

namespace Console.Tests.Library.Deltifiers
{
    public class BaseDeltifierTests
    {
        [Fact]
        public void ProcessDeltas_SplitsAccordingly()
        {
            var source = new Dictionary<string, string>()
            {
                { "onlyinsource", "onlyinsource" },
                { "inbothsame", "inbothsame" },
                { "inbothdiff", "variantOne" }
            };
            var target = new Dictionary<string, string>()
            {
                { "onlyintarget", "onlyintarget" },
                { "inbothsame", "inbothsame" },
                { "inbothdiff", "variantTwo" }
            };
            var d = new TestDeltifier();
            d.ProcessDeltas(source, target);

            Assert.Equal("variantOne", d.wead.First.Value);
            Assert.Equal("inbothsame", d.weas.First.Value);
            Assert.Equal("onlyinsource", d.woeis.First.Value);
            Assert.Equal("onlyintarget", d.woeit.First.Value);
        }

        public class TestDeltifier : BaseSetDeltifier<string, string, string, string>
        {
            public LinkedList<string> woeis = new LinkedList<string>();
            public LinkedList<string> woeit = new LinkedList<string>();
            public LinkedList<string> weas = new LinkedList<string>();
            public LinkedList<string> wead = new LinkedList<string>();
            public override void WhenOnlyExistsInSource(string source)
            {
                woeis.AddLast(source);
            }

            public override void WhenExistsAndDifferent(string source, string target)
            {
                wead.AddLast(source);
            }

            public override void WhenExistsAndSame(string source, string target)
            {
                weas.AddLast(source);
            }

            public override void WhenOnlyExistsInTarget(string target)
            {
                woeit.AddLast(target);
            }
        }
    }
}
