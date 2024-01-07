using GdMUT;

public class SampleTest
{
    [CSTestFunction]
    public static Result Sample()
    {
        bool testConditionGoesHere = true;

        return new Result(testConditionGoesHere, "ayyy");
    }
}
