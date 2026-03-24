using FluentAssertions;
using UrGuide.Core;

namespace UrGuide.UnitTests.Core;

public class PagedListTests
{
    [Fact]
    public void PagedList_default_PageNumber_is_1()
    {
        var pagedList = new PagedList<string>();
        pagedList.PageNumber.Should().Be(1);
    }

    [Fact]
    public void Can_set_properties_on_PagedList()
    {
        var items = new List<string> { "a", "b", "c" };
        var pagedList = PagedList.Of(items, 1);
        pagedList.PageNumber.Should().Be(1);
        pagedList.ItemsCount.Should().Be(3);
        pagedList.Items.Should().HaveCount(3);
    }

    [Fact]
    public void Of_with_mapping_transforms_items()
    {
        var items = new List<int> { 1, 2, 3 };
        var pagedList = PagedList.Of<int, string>(items, 1, x => x.ToString());
        pagedList.Items.Should().ContainInOrder("1", "2", "3");
        pagedList.ItemsCount.Should().Be(3);
    }

    [Fact]
    public void Of_paginates_correctly()
    {
        var items = Enumerable.Range(1, 25).ToList();
        var page1 = PagedList.Of(items, 1);
        page1.Items.Should().HaveCount(10);
        page1.ItemsCount.Should().Be(25);

        var page2 = PagedList.Of(items, 2);
        page2.Items.Should().HaveCount(10);

        var page3 = PagedList.Of(items, 3);
        page3.Items.Should().HaveCount(5);
    }
}
