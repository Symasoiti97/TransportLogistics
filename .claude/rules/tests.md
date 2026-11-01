---
paths:
  - "src/**/Tests/**"
  - "src/**/*Tests*/**"
  - "src/**/*.Tests/**"
---

# Tests: xunit and FluentAssertions

- Stack: xunit + FluentAssertions. There is no separate linter; style is set by `src/.editorconfig`.
- Assertions use `FluentAssertions` (`value.Should().Be(...)`, `BeEquivalentTo`), not `Assert.*`.
- Parameterized cases use `[Theory]` with `[MemberData(nameof(...), MemberType = typeof(...))]`, see `ComparatorTests`. Use `[Fact]` for single cases.
- Test projects: `Tariff/Tests/Tariffs.UnitTests` (unit tests), `Socials/Identity/Startups/Tests` (Business unit tests; `Aggregates/` folder).
- Run one test: `dotnet test TL.sln --filter "FullyQualifiedName~<Name>"` from `src/`.
- Business tests do not need infrastructure (Neo4j, Postgres, Redis).
