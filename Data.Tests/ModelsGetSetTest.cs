using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Xunit;
using System.Linq.Expressions;
using System.Reflection;
using Data.Models;
using Data.Repositories.ReadOnly;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Data.Tests
{
    public class ModelsGetSetTest
    {

        [ClassData(typeof(ModelTestDataGenerator))]
        [Theory]
        public void GettersGetWithoutError<T>(T model)
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (var i = 0; i < properties.Length; i++)
            {
                var prop = properties[i];
                prop.GetValue(model);
            }
        }

        [ClassData(typeof(ModelTestDataGenerator))]
        [Theory]
        public void SettersSetWithoutError<T>(T model)
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (var i = 0; i < properties.Length; i++)
            {
                var prop = properties[i];
                //var curValue = prop.GetValue(model);
                if (prop.GetSetMethod(true) != null)
                    prop.SetValue(model, null);
            }
        }

        public class ModelTestDataGenerator : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new UserRole() },
                new object[] { new User() },
                new object[] { new Role() },
                new object[] { new V_MyView() },
                new object[] { new DataContext(null, null, true, "mockdb") },
                new object[] { new AllDataContext(true, "mockdb2"), },
                new object[] { new ReadOnlyDataContext("mock", true, "mockdb3")}
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
