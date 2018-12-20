using System;
using Xunit;

namespace Bearded.Monads.Tests
{
    public class AlternativeTests
    {
        [Fact]
        public void AlternativeReturnsFirst()
        {
            var expected = "first".AsEither<String, Exception>();

            var actual = "first".AsEither<String,Exception>()
                .Alternatively("second".AsEither<String, Exception>());
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AlternativeFailureYieldsSecond()
        {
            var expected = "second".AsEither<String, Exception>();
            
            var actual = Either<String, Exception>.CreateError(new Exception("boo"))
                .Alternatively("second".AsEither<String, Exception>());
            
            Assert.Equal(expected, actual);
        }
    }
}