using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XNuvem.Data;
using XNuvem.Security;

namespace XNuvem.Tests.Data
{
    [TestFixture]
    class MappingsTests
    {
        [Test]
        public void GetAllMappingsClass() {
            var mappedClass = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("XNuvem.")) //Aumenta a performance
                .SelectMany(a => a.GetTypes())
                .Where(t => IsMappingOf<IEntityMap>(t))
                .ToList();

            
            var entities = mappedClass.Select(t => t.BaseType.GenericTypeArguments.First()).ToList();
            
            Assert.That(mappedClass.Contains(typeof(UserMap)));
            Assert.That(entities.Contains(typeof(User)));
        }

        private bool IsMappingOf<T>(Type type) {
            return !type.IsInterface && !type.IsGenericType && typeof(T).IsAssignableFrom(type);
        }
    }
}
