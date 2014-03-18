Beard.Monads
============

Neckbeard monads for use in C#. These include implementations of `SelectMany` (aka. `bind`) so you can use C#s fluent linq syntax.

Currently only provides Option and Either, as they are usefull for error checking.

Note: `Select` (aka `fmap`) has been renamed to `Map` to avoid confusion for C# developers. If you don't think this is a good idea, please raise an issue.

## Option

An obvious implementation which provides various helper methods. This allows you to avoid passing back `null` from methods and perfoming some special logic. By using `Option`, you enable the compiler to check that you've handled the missing case.

### Usage

As an academic example, consider a trivial method that might have looked thusly without Option:

    public bool TryParseInt(string input, out int output)
    {
        output = default(int);
        int i;
        if(int.TryParse(string, out i))
        {
            output = i;
            return true;
        }

        return false;
    }

Can now be transformed into something a little more sane:

    public Option<int> TryParseInt(string input
    {
        var i = default(int);
        if(!int.TryParse(input, out i))
            return Option<int>.None;
        // Note the implicit operator that converts the `A` to an `Option<A>`
        return i;
    }

As a more realistic example, imagine loading something from a database:

    public MyEntity LoadEntityBy(int id)
    {
        // Please excuse this code, it's been awhile since I've used these types.

        using(var connection = _connectionPool.Open())
        using(var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM Entity WHERE id = {id}";
            command.Parameters.Add("id", id);

            var result = command.ExecuteReader();

            if(result.Results == 0)
                return null;

            return _mapper.From(reader).To(new MyEntity());
        }
    }

Becomes:

    public Option<MyEntity> LoadEntityBy(int id)
    {
        // Please excuse this code, it's been awhile since I've used these types.

        using(var connection = _connectionPool.Open())
        using(var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM Entity WHERE id = {id}";
            command.Parameters.Add("id", id);

            var result = command.ExecuteReader();

            if(result.Results == 0)
                return Option<MyEntity>.None;

            return _mapper.From(reader).To(new MyEntity());
        }
    }

This doesn't seem to add much, but if you compose them:

    public void LoadEntity(string fromPossibleId)
    {
        var maybeEntity = from id in TryParseInt(fromPossibleId)
                          from entity in LoadEntityBy(id)
                          select entity;
        // This will shortcircuit if none of these work.
    }

## Either

This has been renamed to `EitherSuccessOrFailure`, and takes it's "success" value as the first type parameter.

This was done as Either "short circuits" on a `Left` in haskell, but this seems a little unnatural from C#. Please raise an issue if you don't believe this is the case.

### Usage

This is useful to return some error condition from a long chain of Either results (or even via the `AsEither` when dealing with Option results).

For example, if I have the following chain of optional results:

    public Option<ResultFromTertiaryService> LoadFromAMultitudeOfServices(string value)
    {
        return from id in TryParseInt(value)
               from first in ExpensiveCallToAWebServiceThatMightFail(id)
               from second in TryAndLoadARecordFromTheDatabase(id, first.ClientData.SomeField)
               from third in TryAndFindDataInTertiaryService(id, second.AnotherField, first.Some.Other.Context)
               select third;
    }

This might fail at any point, so it's helpful to *tag* the `None` with some helpful context, e.g.

    public EitherSuccessOrFailure<ResultFromTertiaryService,string> LoadFromAMultitudeOfServices(string value)
    {
        return from id in TryParseInt(value).AsEither("Failed to parse ({0}) into an id", value)
               from first in ExpensiveCallToAWebServiceThatMightFail(id).AsEither("Didn't find a value")
               from second in TryAndLoadARecordFromTheDatabase(id, first.ClientData.SomeField).AsEither("Couldn't find {0} in the database", id)
               from third in TryAndFindDataInTertiaryService(id, second.AnotherField, first.Some.Other.Context).AsEither("Failed to load from tertiary source")
               select third;
    }