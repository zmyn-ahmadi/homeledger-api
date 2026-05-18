using Castle.Core.Logging;
using HomeLedger.API.DTOs;
using HomeLedger.API.Mappers;
using HomeLedger.API.Models;
using HomeLedger.API.Services;
using Moq;
using NSubstitute;
using Shouldly;
using Xunit;

namespace HomeLedger.API.UnitTests.BillTests;

public class BillServiceTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly IBillRepository _bills = Substitute.For<IBillRepository>();
    private readonly BillMapper _mapper = new();
    private readonly BillService _ser;
    private readonly ILogger<BillService> _logger = Substitute.For<ILogger<BillService>>();

    public BillServiceTests()
    {
        _uow.Bills.Returns(_bills);
        _ser = new BillService(_uow, _mapper, _logger);
    }
}
