namespace Scribbly.Eventually.Tests.RoutingTests;

public class AddBottleTest : BoxTestBase
{
    [Fact]
    public void Add_bottle_to_empty_box_succeeds()
    {
        Given(
            Box_created_with_capacity(24));

        When(
            Add_killer_belgian_beer());

        Expect(
            Killer_belgian_beer_added());
    }

    [Fact]
    public void Add_bottle_to_full_box_fails()
    {
        Given(
            Box_created_with_capacity(1),
            Killer_belgian_beer_added());

        When(
            Add_killer_belgian_beer());

        Expect(
            Beer_failed_to_add_because_box_was_full());
    }
}