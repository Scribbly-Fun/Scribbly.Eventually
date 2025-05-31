namespace Scribbly.Eventually.UnitTests.RoutingTests;

public class CreateBoxTest : BoxTestBase
{
    [Theory]
    [InlineData(-1, 6)]
    [InlineData(0, 6)]
    [InlineData(5, 6)]
    [InlineData(6, 6)]
    [InlineData(7, 12)]
    [InlineData(11, 12)]
    [InlineData(12, 12)]
    [InlineData(13, 24)]
    [InlineData(23, 24)]
    [InlineData(24, 24)]
    [InlineData(25, 24)]
    public void Creates_box_with_correct_capacity(int desired_capacity, int resulting_capacity)
    {
        Given();

        When(Create_box_with_desired_capacity(desired_capacity));

        Expect(Box_created_with_capacity(resulting_capacity));
    }
}