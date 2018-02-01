using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Console.Tests
{
    public class OptionsTests
    {
        [Fact]
        public void OptionsValid()
        {
            // Does the object construct without error
            new Options();
        }
    }
}
