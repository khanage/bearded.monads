using System.Linq;
using Xunit;

namespace Bearded.Monads.Tests
{
    public class EnumerableExtensionTests
    {
        [Fact]
        public void FlattenIEnumerableOfIEnumerable_OK()
        {
            var input = Enumerable.Range(1, 3).Select(i => Enumerable.Range(i * 10, 2));
            var result = input.Flatten();

            Assert.Equal(new []{10,11,20,21,30,31}, result.ToArray());
        }
    }
}
