using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Bearded.Monads.Syntax;

namespace Bearded.Monads.Tests
{
    public class EitherApplicativeTests
    {
        private Either<string,string> NewEither(string value) =>
            Either<string,string>.CreateSuccess(value);

        [Fact]
        public void OneArg()
        {
            var result = EitherArg(NewEither("A"))
                            .Then(a => a + "!");
            var expected = "A!";

            Assert.True(result.IsSuccess);
            Assert.False(result.IsError);

            Assert.Equal(expected, result.AsSuccess.Value);
        }

        [Fact]
        public void TwoArgs()
        {
            var result = EitherArg(NewEither("A"))
                            .And(NewEither("B"))
                            .Then((a, b) => a + b + "!");
            var expected = "AB!";

            Assert.True(result.IsSuccess);
            Assert.False(result.IsError);

            Assert.Equal(expected, result.AsSuccess.Value);
        }
        
        [Fact]
        public void ThreeArgs()
        {
            var result = EitherArg(NewEither("A"))
                            .And(NewEither("B"))
                            .And(NewEither("C"))
                            .Then((a, b, c) => a + b + c + "!");
            var expected = "ABC!";

            Assert.True(result.IsSuccess);
            Assert.False(result.IsError);

            Assert.Equal(expected, result.AsSuccess.Value);
        }
    
        [Fact]
        public void FourArgs()
        {
            var result = EitherArg(NewEither("A"))
                            .And(NewEither("B"))
                            .And(NewEither("C"))
                            .And(NewEither("D"))
                            .Then((a, b, c, d) => a + b + c + d + "!");
            var expected = "ABCD!";

            Assert.True(result.IsSuccess);
            Assert.False(result.IsError);

            Assert.Equal(expected, result.AsSuccess.Value);
        }

        [Fact]
        public void FiveArgs()
        {
            var result = EitherArg(NewEither("A"))
                            .And(NewEither("B"))
                            .And(NewEither("C"))
                            .And(NewEither("D"))
                            .And(NewEither("E"))
                            .Then((a, b, c, d, e) => a + b + c + d + e + "!");
            var expected = "ABCDE!";

            Assert.True(result.IsSuccess);
            Assert.False(result.IsError);

            Assert.Equal(expected, result.AsSuccess.Value);
        }

        [Fact]
        public void SixArgs()
        {
            var result = EitherArg(NewEither("A"))
                            .And(NewEither("B"))
                            .And(NewEither("C"))
                            .And(NewEither("D"))
                            .And(NewEither("E"))
                            .And(NewEither("F"))
                            .Then((a, b, c, d, e, f) => a + b + c + d + e + f + "!");
            var expected = "ABCDEF!";

            Assert.True(result.IsSuccess);
            Assert.False(result.IsError);

            Assert.Equal(expected, result.AsSuccess.Value);
        }

        [Fact]
        public void SevenArgs()
        {
            var result = EitherArg(NewEither("A"))
                            .And(NewEither("B"))
                            .And(NewEither("C"))
                            .And(NewEither("D"))
                            .And(NewEither("E"))
                            .And(NewEither("F"))
                            .And(NewEither("G"))
                            .Then((a, b, c, d, e, f, g) => a + b + c + d + e + f + g + "!");
            var expected = "ABCDEFG!";

            Assert.True(result.IsSuccess);
            Assert.False(result.IsError);

            Assert.Equal(expected, result.AsSuccess.Value);
        }

        
        [Fact]
        public void OneErrorArg()
        {
            var result = EitherArg(Either<string,string>.CreateError("Error"))
                            .Then(a => a + "!");
            var expected = "Error";

            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);

            Assert.Equal(expected, result.AsError.Value);
        }
        

        [Fact]
        public void SevenArgsOneError()
        {
            var result = EitherArg(NewEither("A"))
                            .And(NewEither("B"))
                            .And(NewEither("C"))
                            .And(Either<string,string>.CreateError("D IS ERROR"))
                            .And(NewEither("E"))
                            .And(NewEither("F"))
                            .And(NewEither("G"))
                            .Then((a, b, c, d, e, f, g) => a + b + c + d + e + f + g + "!");
            var expected = "D IS ERROR";

            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);

            Assert.Equal(expected, result.AsError.Value);
        }

        [Fact]
        public void SevenArgsTwoErrors()
        {
            var result = EitherArg(NewEither("A"))
                            .And(NewEither("B"))
                            .And(NewEither("C"))
                            .And(Either<string,string>.CreateError("D IS ERROR"))
                            .And(NewEither("E"))
                            .And(NewEither("F"))
                            .And(Either<string,string>.CreateError("G IS ERROR"))
                            .Then((a, b, c, d, e, f, g) => a + b + c + d + e + f + g + "!");
            var expected = "D IS ERROR";

            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);

            Assert.Equal(expected, result.AsError.Value);
        }
    }
}
