using IMS.Application.Common.Interfaces;
using Moq;

namespace IMS.UnitTests.Application.Common;

public abstract class TestBase
{
    protected readonly Mock<IUnitOfWork> UnitOfWorkMock;
    protected readonly Mock<IItemRepository> ItemRepositoryMock;

    protected TestBase()
    {
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        ItemRepositoryMock = new Mock<IItemRepository>();
    }
}
