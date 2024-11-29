using Bearded.Monads;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bearded.Monads.Tests
{
    public class TryAsyncTests
    {
        [Fact]
        public async void QuerySyntax_Happy()
        {
            Task<Try<ComplexObject>> ReturnString()
            {
                return Task.FromResult(new ComplexObject("test1").AsTry());
            }

            Task<Try<ComplexObject>> ReturnString2(string entry)
            {
                return Task.FromResult(new ComplexObject($"test2-{entry}").AsTry());
            }

            Try<string> ReturnString3(string entry)
            {
                return $"test3-{entry}";
            }

            Try<string> result = await (from s1 in ReturnString()
                                        let fakeValue = s1
                                        from s2 in ReturnString2(s1.Name)
                                        from s3 in ReturnString3(s2.Name).AsTask()
                                        select s3);

            Assert.True(result.IsSuccess);
        }

        public class ComplexObject
        {
            public ComplexObject(string name)
            {
                Name = name;
            }
            public string Name { get; set; }
        }
    }
}
