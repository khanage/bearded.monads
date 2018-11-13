using System;
using Xunit;
using Bearded.Monads;

namespace Bearded.Monads.Tests
{
    public class MapErrorTests
    {
        [Fact]
        public void MapErrorWhenSuccess()
        {
            var expected = "New Error";

            var initial = Either<string, string>.CreateSuccess("Success");

            var result = initial.MapError(_ => expected);

            Assert.True(result.IsSuccess);
            Assert.False(result.IsError);

            Assert.Equal("Success", result.AsSuccess.Value);
        }

        [Fact]
        public void MapError()
        {
            var expected = DateTime.Now;

            var initial = Either<int, string>.CreateError("Error");

            var result = initial.MapError(_ => expected);

            Assert.True(result.IsError);
            Assert.False(result.IsSuccess);

            Assert.Equal(expected, result.AsError.Value);
        }

        [Fact]
        public void MapErrorSameInitialType()
        {
            var expected = DateTime.Now;

            var initial = Either<string, string>.CreateError("Error");

            var result = initial.MapError(_ => expected);

            Assert.True(result.IsError);
            Assert.False(result.IsSuccess);

            Assert.Equal(expected, result.AsError.Value);
        }

        [Fact]
        public void MapErrorSameFinalType()
        {
            var expected = DateTime.Now;

            var initial = Either<DateTime, string>.CreateError("Error");

            var result = initial.MapError(_ => expected);

            Assert.True(result.IsError);
            Assert.False(result.IsSuccess);

            Assert.Equal(expected, result.AsError.Value);
        }

        [Fact]
        public void MapErrorSameTypes()
        {
            var expected = "NewError";

            var initial = Either<string, string>.CreateError("Error");

            var result = initial.MapError(_ => expected);

            Assert.True(result.IsError);
            Assert.False(result.IsSuccess);

            Assert.Equal(expected, result.AsError.Value);
        }
    }
}
