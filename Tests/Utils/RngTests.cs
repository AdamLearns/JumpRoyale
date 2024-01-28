using Godot;

namespace Utils;

[TestFixture]
public class RngTests
{
    /// <summary>
    /// This test makes sure we can generate all numbers from the given range and there is no off-by-one error in the
    /// implementation.
    /// </summary>
    [Test]
    public void CanGenerateInclusiveRange()
    {
        List<int> numbers = [];

        // After running this 1 billion times, the maximum steps reached 180~, with an average of 40 tries to get all 10
        // numbers on the list, so should be fine here
        for (int i = 0; i < 250; i++)
        {
            int number = Rng.IntRange(1, 10);

            if (numbers.Contains(number))
            {
                continue;
            }

            numbers.Add(number);

            if (numbers.Count == 10)
            {
                break;
            }
        }

        Assert.Multiple(() =>
        {
            Assert.That(numbers.Sum(), Is.EqualTo(55));
            Assert.That(numbers, Has.Count.EqualTo(10));
        });
    }

    /// <summary>
    /// This test only exists in case there were any changes in the implemented methods, otherwise this should never
    /// fail for default [0, maxUInt) / maxUInt.
    /// </summary>
    [Test]
    public void CanGenerateZeroOneFloat()
    {
        bool outOfRange = false;

        for (int i = 0; i < 10000000; i++)
        {
            float result = Rng.RandomFloat();

            if (result < 0 || result > 1)
            {
                outOfRange = true;
                break;
            }
        }

        Assert.That(outOfRange, Is.False);
    }

    [Test]
    public void CanGenerateRandomColor()
    {
        Assert.DoesNotThrow(() =>
        {
            try
            {
                Color color = Color.FromHtml(Rng.RandomHex());
                Assert.That(Color.HtmlIsValid(color.ToHtml()));
            }
            catch (Exception)
            {
                throw;
            }
        });
    }
}
