using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;

namespace Data.Tests
{
    public class AllDataContextTest
    {
        [Fact]
        public void Save_CallsUnderlyingSaveChanges()
        {
            var dcm = new Mock<AllDataContext>(true, nameof(Save_CallsUnderlyingSaveChanges));
            dcm.Setup(d => d.SaveChanges());
            dcm.Object.Save();
            dcm.Verify(d => d.SaveChanges(), Times.Once);
        }
    }
}
