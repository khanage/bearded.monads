Bearded.Monads
============

[![Publish Bearded.Monads](https://github.com/khanage/bearded.monads/actions/workflows/dotnet.yml/badge.svg)](https://github.com/khanage/bearded.monads/actions/workflows/dotnet.yml)

Monads for use in C#. These include implementations of `SelectMany` (aka. `bind`) so you can use C#s fluent linq syntax.

Currently provides Option and Either, as they are useful for error checking, as well as task.

Also provides applicative instances for Task and Maybe.

### Installation

Bearded.Monads is available from NuGet:

`Install-Package Bearded.Monads`

Then, just add `using Bearded.Monads;` to the top of your C# source file. There is also a `Bearded.Monads.Syntax` module that you can reference using `using static Bearded.Monads.Syntax;` with your other using statements. 

## Option

An obvious implementation which provides various helper methods. This allows you to avoid passing back `null` from methods and perfoming some special logic. By using `Option`, you enable the compiler to check that you've handled the missing case.

### Usage

As an academic example, consider a trivial method that might have looked thusly without Option:

```c#
public bool TryParseInt(string input, out int output)
{
    if(int.TryParse(input, out int i))
    {
        output = i;
        return true;
    }
    output = default
    return false;
}
```

Can now be transformed into something a little more sane:

```c#
public Option<int> TryParseInt(string input)
{
    if(int.TryParse(input, out int i))
    {
        // Note the implicit operator that converts the `A` to an `Option<A>`
        return i;
    }
    return Option<int>.None;
}
```

As a more realistic example, imagine loading something from a database:

```c#
public MyEntity LoadEntityBy(int id)
{
    // Assume Dapper is being used...
    using(var connection = new IDbConnection(_connectionString))
    {
        string sql = "SELECT * FROM Entity WHERE Id = @Id";
        var result = connection.Query<DbEntity>(sql, new { Id = id });
        
        if(result.Any())
        {
            return _mapper.From(result.First()).To(new MyEntity());
        }
        return null;
    }
}
```

Becomes:

```c#
public Option<MyEntity> LoadEntityBy(int id)
{
    // Assume Dapper is being used...
    using(var connection = new IDbConnection(_connectionString))
    {
        string sql = "SELECT * FROM Entity WHERE Id = @Id";
        var result = connection.Query<DbEntity>(sql, new { Id = id });
        
        if(result.Any())
        {
            return _mapper.From(result.First()).To(new MyEntity());
        }
        return Option<MyEntity>.None;
    }
}
```

This doesn't seem to add much, but if you compose them:

```c#
public void LoadEntity(string fromPossibleId)
{
    var maybeEntity = from id in TryParseInt(fromPossibleId)
                        from entity in LoadEntityBy(id)
                        select entity;
    // This will shortcircuit if none of these work.
}
```

## Either

This has been renamed from `EitherSuccessOrFailure`, and takes it's "success" value as the first type parameter.

This was done as Either "short circuits" on a `Left` in haskell, but this seems a little unnatural from C#. Please raise an issue if you don't believe this is the case.

### Usage

This is useful to return some error condition from a long chain of Either results (or even via the `AsEither` when dealing with Option results).

For example, if I have the following chain of optional results:

```c#
public Option<ResultFromTertiaryService> LoadFromAMultitudeOfServices(string value)
{
    return from id in TryParseInt(value)
            from first in ExpensiveCallToAWebServiceThatMightFail(id)
            from second in TryAndLoadARecordFromTheDatabase(id, first.ClientData.SomeField)
            from third in TryAndFindDataInTertiaryService(id, second.AnotherField, first.Some.Other.Context)
            select third;
}
```

This might fail at any point, so it's helpful to *tag* the `None` with some helpful context by using `AsEither` to convert from `Option<Success>` to `Either<Success, string>` e.g.

```c#
public Either<ResultFromTertiaryService,string> LoadFromAMultitudeOfServices(string value)
{
    return from id in TryParseInt(value).AsEither("Failed to parse ({0}) into an id", value)
            from first in ExpensiveCallToAWebServiceThatMightFail(id).AsEither("Didn't find a value")
            from second in TryAndLoadARecordFromTheDatabase(id, first.ClientData.SomeField).AsEither("Couldn't find {0} in the database", id)
            from third in TryAndFindDataInTertiaryService(id, second.AnotherField, first.Some.Other.Context).AsEither("Failed to load from tertiary source")
            select third;
}
```

## Try

This is similar to Either, but uses an exception in the error case. This is useful for things like the `SafeCallback` extension method over all objects, which provides an ergonomic version of wrapping everything in a try-catch block.

## Task Applicative (aka Asynquence)

This is probable the most interesting use of `Task`. This allows one to chain together a sequence of tasks and provide a callback at the end to produce a final result.

It's recommended to use the below to bring the class into scope directly.
 
```
using static Bearded.Monads.Syntax;
```

Then usage is as follows:

```
            var result = await Asynquence(Task.FromResult(10))
                .And(Task.FromResult(10))
                .And(Task.FromResult(10))
                .And(Task.FromResult(10))
                .Select((a, b, c, d) => a + b + c + d);
            var expected = 40;

            Assert.Equal(expected, result);
 ```
