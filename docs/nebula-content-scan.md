=== Scanning agents/ for Nebula-specific content ===

agents/AGENT-STATUS.md:5:This document tracks the completion status of all builder agent roles for the Nebula insurance CRM project.
agents/AGENT-STATUS.md:52:    - architecture-examples.md ⭐ *expanded from 135 to 770 lines with complete Broker, Submission, Account 360, and ADR examples*
agents/AGENT-STATUS.md:315:    - persona-examples.md: Expanded from 19 lines → 350 lines with 3 complete personas (Sarah, Marcus, Jennifer)
agents/AGENT-STATUS.md:316:    - screen-spec-examples.md: Expanded from 16 lines → 702 lines with 3 detailed screen specs (Broker List, Broker 360, Create Broker)
agents/architect/README.md:74:│   └── architecture-examples.md          # Real examples from Nebula
agents/architect/README.md:147:| `architecture-examples.md` | Real Nebula examples | Reference implementations |
agents/architect/README.md:342:- Workflows specified: Target = Submission + Renewal
agents/architect/references/api-design-guide.md:3:Comprehensive guide for designing RESTful APIs using .NET 10 Minimal APIs for the Nebula insurance CRM. This guide covers REST principles, request/response patterns, pagination, filtering, versioning, security, and OpenAPI documentation.
agents/architect/references/api-design-guide.md:13:- ❌ Bad: `GET /api/getBrokers`, `POST /api/createSubmission`
agents/architect/references/api-design-guide.md:135:**Recommendation for Nebula MVP:** Keep HATEOAS minimal (use for pagination links only). Full HATEOAS adds complexity without clear benefit for internal API.
agents/architect/references/api-design-guide.md:146:public record CreateBrokerRequest
agents/architect/references/api-design-guide.md:169:    CreateBrokerRequest request,
agents/architect/references/api-design-guide.md:170:    IBrokerService brokerService,
agents/architect/references/api-design-guide.md:171:    IValidator<CreateBrokerRequest> validator) =>
agents/architect/references/api-design-guide.md:180:.WithName("CreateBroker")
agents/architect/references/api-design-guide.md:181:.WithTags("Brokers")
agents/architect/references/api-design-guide.md:182:.Produces<BrokerResponse>(201)
agents/architect/references/api-design-guide.md:200:public record BrokerResponse
agents/architect/references/api-design-guide.md:220:var broker = await context.Brokers.FindAsync(id);
agents/architect/references/api-design-guide.md:221:var response = new BrokerResponse
agents/architect/references/api-design-guide.md:292:  "message": "User lacks CreateBroker permission",
agents/architect/references/api-design-guide.md:317:    { "id": "...", "name": "Best Brokers" }
agents/architect/references/api-design-guide.md:347:    { "id": "...", "name": "Broker 21" },
agents/architect/references/api-design-guide.md:348:    { "id": "...", "name": "Broker 22" }
agents/architect/references/api-design-guide.md:366:    var query = context.Brokers.AsQueryable();
agents/architect/references/api-design-guide.md:374:        .Select(b => new BrokerListItem { Id = b.Id, Name = b.Name })
agents/architect/references/api-design-guide.md:419:var brokers = await context.Brokers
agents/architect/references/api-design-guide.md:486:    var query = context.Brokers.AsQueryable();
agents/architect/references/api-design-guide.md:553:var brokers = await context.Brokers
agents/architect/references/api-design-guide.md:563:### 5.1 URI Versioning (Recommended for Nebula)
agents/architect/references/api-design-guide.md:577:v1.MapGet("/brokers", GetBrokersV1);
agents/architect/references/api-design-guide.md:580:v2.MapGet("/brokers", GetBrokersV2);
agents/architect/references/api-design-guide.md:737:        Title = "Nebula Insurance CRM API",
agents/architect/references/api-design-guide.md:742:            Name = "Nebula Support",
agents/architect/references/api-design-guide.md:784:    BrokerResponse:
agents/architect/references/api-design-guide.md:816:app.MapPost("/api/brokers", async (CreateBrokerRequest request) => { ... })
agents/architect/references/api-design-guide.md:820:        operation.Description = "Creates a new insurance broker or brokerage firm. Requires CreateBroker permission.";
agents/architect/references/api-design-guide.md:823:    .Produces<BrokerResponse>(201)
agents/architect/references/architecture-best-practices.md:3:Comprehensive guide for designing robust, maintainable architecture for Nebula and similar enterprise applications.
agents/architect/references/architecture-best-practices.md:60:- Entities (Broker, Submission, Renewal)
agents/architect/references/architecture-best-practices.md:62:- Domain Events (BrokerCreated, SubmissionBound)
agents/architect/references/architecture-best-practices.md:74:public class Broker
agents/architect/references/architecture-best-practices.md:79:    public BrokerStatus Status { get; private set; }
agents/architect/references/architecture-best-practices.md:84:        if (Status == BrokerStatus.Suspended)
agents/architect/references/architecture-best-practices.md:87:        Status = BrokerStatus.Active;
agents/architect/references/architecture-best-practices.md:88:        AddDomainEvent(new BrokerActivated(Id));
agents/architect/references/architecture-best-practices.md:92:    public static Broker Create(string name, string licenseNumber)
agents/architect/references/architecture-best-practices.md:98:        var broker = new Broker
agents/architect/references/architecture-best-practices.md:103:            Status = BrokerStatus.Active
agents/architect/references/architecture-best-practices.md:106:        broker.AddDomainEvent(new BrokerCreated(broker.Id, broker.Name));
agents/architect/references/architecture-best-practices.md:115:- Use Cases / Application Services (CreateBrokerUseCase, GetBrokerQuery)
agents/architect/references/architecture-best-practices.md:129:public class CreateBrokerUseCase
agents/architect/references/architecture-best-practices.md:131:    private readonly IBrokerRepository _repository;
agents/architect/references/architecture-best-practices.md:135:    public async Task<BrokerDto> Execute(CreateBrokerCommand command, User user)
agents/architect/references/architecture-best-practices.md:138:        await _authz.CheckPermission(user, "CreateBroker");
agents/architect/references/architecture-best-practices.md:141:        var broker = Broker.Create(
agents/architect/references/architecture-best-practices.md:151:        await _timeline.LogEvent(new BrokerCreatedEvent(broker.Id, user.Id));
agents/architect/references/architecture-best-practices.md:170:public class BrokerRepository : IBrokerRepository
agents/architect/references/architecture-best-practices.md:172:    private readonly NebulaDbContext _context;
agents/architect/references/architecture-best-practices.md:174:    public async Task<Broker> GetById(Guid id)
agents/architect/references/architecture-best-practices.md:176:        return await _context.Brokers
agents/architect/references/architecture-best-practices.md:181:    public async Task Add(Broker broker)
agents/architect/references/architecture-best-practices.md:183:        await _context.Brokers.AddAsync(broker);
agents/architect/references/architecture-best-practices.md:207:public class BrokersController : ControllerBase
agents/architect/references/architecture-best-practices.md:209:    private readonly CreateBrokerUseCase _createBroker;
agents/architect/references/architecture-best-practices.md:212:    [ProducesResponseType(typeof(BrokerResponse), 201)]
agents/architect/references/architecture-best-practices.md:214:    public async Task<IActionResult> Create([FromBody] CreateBrokerRequest request)
agents/architect/references/architecture-best-practices.md:217:        var result = await _createBroker.Execute(command, User);
agents/architect/references/architecture-best-practices.md:238:public class BrokerService
agents/architect/references/architecture-best-practices.md:240:    public void CreateBroker() { /* creates broker */ }
agents/architect/references/architecture-best-practices.md:250:public class BrokerService
agents/architect/references/architecture-best-practices.md:252:    private readonly IBrokerRepository _repository;
agents/architect/references/architecture-best-practices.md:255:    private readonly IValidator<Broker> _validator;
agents/architect/references/architecture-best-practices.md:257:    public void CreateBroker()
agents/architect/references/architecture-best-practices.md:272:    decimal Calculate(Submission submission);
agents/architect/references/architecture-best-practices.md:277:    public decimal Calculate(Submission submission) { /* restaurant logic */ }
agents/architect/references/architecture-best-practices.md:282:    public decimal Calculate(Submission submission) { /* retail logic */ }
agents/architect/references/architecture-best-practices.md:326:public interface IBrokerRepository
agents/architect/references/architecture-best-practices.md:328:    Task<Broker> GetById(Guid id);
agents/architect/references/architecture-best-practices.md:329:    Task Add(Broker broker);
agents/architect/references/architecture-best-practices.md:330:    Task Update(Broker broker);
agents/architect/references/architecture-best-practices.md:332:    Task<List<Broker>> Search(string term);
agents/architect/references/architecture-best-practices.md:333:    Task<BrokerStatistics> GetStatistics(Guid id);
agents/architect/references/architecture-best-practices.md:334:    Task<List<Submission>> GetSubmissions(Guid id);
agents/architect/references/architecture-best-practices.md:341:public interface IBrokerRepository
agents/architect/references/architecture-best-practices.md:343:    Task<Broker> GetById(Guid id);
agents/architect/references/architecture-best-practices.md:344:    Task Add(Broker broker);
agents/architect/references/architecture-best-practices.md:348:public interface IBrokerQueryService
agents/architect/references/architecture-best-practices.md:350:    Task<List<Broker>> Search(string term);
agents/architect/references/architecture-best-practices.md:351:    Task<BrokerStatistics> GetStatistics(Guid id);
agents/architect/references/architecture-best-practices.md:354:public interface IBrokerSubmissionsService
agents/architect/references/architecture-best-practices.md:356:    Task<List<Submission>> GetSubmissions(Guid id);
agents/architect/references/architecture-best-practices.md:366:public class BrokerService
agents/architect/references/architecture-best-practices.md:368:    private SqlBrokerRepository _repository; // Concrete dependency!
agents/architect/references/architecture-best-practices.md:370:    public BrokerService()
agents/architect/references/architecture-best-practices.md:372:        _repository = new SqlBrokerRepository(); // Tightly coupled!
agents/architect/references/architecture-best-practices.md:379:public class BrokerService
agents/architect/references/architecture-best-practices.md:381:    private readonly IBrokerRepository _repository; // Abstract dependency
agents/architect/references/architecture-best-practices.md:383:    public BrokerService(IBrokerRepository repository) // Injected
agents/architect/references/architecture-best-practices.md:390:services.AddScoped<IBrokerRepository, SqlBrokerRepository>();

=== Summary ===
Total matches: 55
