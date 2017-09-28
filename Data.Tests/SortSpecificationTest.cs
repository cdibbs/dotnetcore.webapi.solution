using System;
using System.Collections.Generic;
using System.Text;
using Data.Utilities;
using Xunit;

namespace Data.Tests
{
    public class SortSpecificationTest
    {
        [Fact]
        public void ToString_FormatsInformatively()
        {
            var spec = new SortSpecification("SomeProp", SortDirection.Ascending);
            var formatted = spec.ToString();
            Assert.Equal("SomeProp:Ascending", formatted);
        }

        [Fact]
        public void ToString_NullParamsNoError()
        {
            var spec = new SortSpecification(null, SortDirection.Descending);
            var formatted = spec.ToString();
            Assert.Equal("null:Descending", formatted);
        }
    }
}
