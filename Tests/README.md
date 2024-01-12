# Abandoned tests notice

This directory depended on GdMut plugin, which caused loading problems on different machines and required manual actions to get it to work. Apparently, most C# plugins are a hassle, so it will not be used any more for unit tests.

Unit Tests without automation are not real tests and it's easy to miss something, because tests had to be triggered manually inside a scene through a separate tab, so it was a bit problematic.

**Note to contributors**: the old tests will remain in this directory just for reference purposes, since
the test cases already covered some functionality and were able to find faulty behavior, so their logic _could_ be used in future tests to guard from unintentional changes. They will be stored until NUnit or xUnit testing frameworks are implemented.

> [!note]
> It was intentional to leave the entire test code commented, because GdMut doesn't exist anymore and they will just trigger build errors.
