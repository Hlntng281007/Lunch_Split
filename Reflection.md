# Reflection: Assignment 1 (Start / Stop / Continue)

**Name:** Kim Quynh Huong Tuong

## Start

- **Compiling before the end, not just at the end.** I wrote the models first, then the rest of the implementation and tests, and only ran my first build once everything was written. That meant a stray semicolon, an empty method body, a wrong namespace, and a typo in a `using` all surfaced at the same time, in one error list, instead of one at a time as I wrote each piece. Building after each class — not just after the whole thing — would have shown me each mistake right where I made it.
- **Checking package versions when an API "doesn't exist."** `Assert.ThrowsException` wasn't available because my project used MSTest v4, which replaced it with `Assert.ThrowsExactly`. I learned to check the framework version before assuming I'd made a mistake.
- **Injecting dependencies that make tests flaky.** Passing the clock into the receipt formatter let me assert an exact timestamp instead of racing `DateTime.Now`.
- **Reading an assertion failure as a question about my design, not just my test.** A rounding test failed because my penny-reconciliation logic ran for every rounding mode, silently overriding an explicit "round down" choice. Reconciliation only makes sense when rounding to nearest (where independent per-share rounding can leave an ambiguous total); directional modes like up/down are already deterministic and shouldn't be redistributed afterward.
- **Checking that overload I called actually does what I think.** `Math.Round(amount, MidpointRounding.ToEven)` compiles fine, but without the decimal-places argument it rounds to the nearest whole dollar, not the nearest cent — a bug that wouldn't have shown up without a test checking an exact cent value.

## Stop

- **Trusting that my project matched the spec without checking.** My project targeted .NET 10 at more than one point while the assignment requires .NET 8. I should diff my setup against the spec at the start of each session, not assume a previous fix stuck.
- **Writing large batches of code before running any tests.** Small red-green cycles would have caught problems sooner and made the commit history more meaningful.
- **Putting behavior on data models.** My first draft put tip calculation directly on the bill record. Keeping models as plain data and behavior in engine classes made the design cleaner and easier to test.
- **Letting a test's name describe something its arrange step doesn't actually set up.** One test named an excluded attendee, but every attendee in the arrange block had `Included: true`, so nobody was actually excluded. The assertions and the method name were both correct — the arrange step just didn't match them.
- **Leaving unused imports and scaffold files in place.** A stray `using` for a package I never referenced, and the default test-scaffold file MSTest generates, both sat in the project longer than they should have.

## Continue

- **Writing tests before behavior.** For classes that decide or calculate something, writing the failing test first kept me focused on required behavior instead of guessing at an interface.
- **Strict AAA layout and descriptive test names.** Explicit Arrange/Act/Assert sections made every test quick to read.
- **Keeping rounding logic separate from splitting logic.** The penny reconciliation pass was easy to test in isolation because it wasn't tangled up with tip and weight math.
- **Frequent, meaningful commits.** Small commits tied to a specific test or feature make the history useful, not just a checkbox.

## What I learned

The hardest part was penny reconciliation: rounding each share independently can leave the total a cent off, so the remainder has to be redistributed deliberately. It taught me that "correct" for money means the pieces must add up exactly, not that each piece looks reasonable on its own.

It also taught me that a passing test for one scenario doesn't mean the logic is right in general. My reconciliation code passed the test it was written for, but was quietly wrong for other rounding modes until a different test caught it — and a missing function argument elsewhere compiled cleanly while still producing the wrong answer. Both bugs were only caught because each rounding mode had its own test with an exact expected value, not just a general "it rounds the number" check.
