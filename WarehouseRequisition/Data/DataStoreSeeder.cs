using WarehouseRequisition.Enums;
using WarehouseRequisition.Models;

namespace WarehouseRequisition.Data;

/// <summary>
/// Seeds realistic Spanish manufacturing sample data for the prototype demo.
/// </summary>
public class DataStoreSeeder : IDataStoreSeeder
{
    private readonly InMemoryDataStore _store;
    private readonly IDataStorePersistence _persistence;

    public DataStoreSeeder(InMemoryDataStore store, IDataStorePersistence persistence)
    {
        _store = store;
        _persistence = persistence;
    }

    public void SeedIfEmpty()
    {
        lock (_store.SyncRoot)
        {
            if (_store.Users.Count == 0 || _store.Parts.Count == 0)
            {
                SeedCore();
                _persistence.Save(_store);
            }
            else
            {
                // Keep previously saved prototype data (JSON snapshot already loaded).
            }
        }
    }

    public void ResetToSeedData()
    {
        lock (_store.SyncRoot)
        {
            SeedCore();
            _persistence.Save(_store);
        }
    }

    private void SeedCore()
    {
        _store.Users.Clear();
        _store.Plants.Clear();
        _store.Areas.Clear();
        _store.Machines.Clear();
        _store.Parts.Clear();
        _store.PartLocations.Clear();
        _store.ShortageReasons.Clear();
        _store.Requisitions.Clear();

        SeedUsers();
        SeedPlants();
        SeedAreas();
        SeedMachines();
        SeedParts();
        SeedShortageReasons();
        SeedRequisitions();
    }

    private void SeedUsers()
    {
        var users = new List<User>
        {
            new() { Id = 1, EmployeeNumber = "10021", Name = "Laura Méndez Ríos", Email = "laura.mendez@planta.mx", Role = UserRole.Production, IsActive = true },
            new() { Id = 2, EmployeeNumber = "10088", Name = "Jorge Ramírez Castillo", Email = "jorge.ramirez@planta.mx", Role = UserRole.Production, IsActive = true },
            new() { Id = 3, EmployeeNumber = "10145", Name = "Ana Sofía Herrera", Email = "ana.herrera@planta.mx", Role = UserRole.Production, IsActive = true },
            new() { Id = 4, EmployeeNumber = "10233", Name = "Miguel Ángel Torres", Email = "miguel.torres@planta.mx", Role = UserRole.Production, IsActive = true },
            new() { Id = 5, EmployeeNumber = "20114", Name = "María Fernanda López", Email = "maria.lopez@planta.mx", Role = UserRole.Warehouse, IsActive = true },
            new() { Id = 6, EmployeeNumber = "20177", Name = "Carlos Sánchez Vega", Email = "carlos.sanchez@planta.mx", Role = UserRole.Warehouse, IsActive = true },
            new() { Id = 7, EmployeeNumber = "30102", Name = "Alejandra Navarro Pineda", Email = "alejandra.navarro@planta.mx", Role = UserRole.Supervisor, IsActive = true },
            new() { Id = 8, EmployeeNumber = "30341", Name = "Roberto Díaz Fuentes", Email = "roberto.diaz@planta.mx", Role = UserRole.Supervisor, IsActive = true },
            new() { Id = 9, EmployeeNumber = "40018", Name = "Diana Cruz Molina", Email = "diana.cruz@planta.mx", Role = UserRole.Administrator, IsActive = true },
            new() { Id = 10, EmployeeNumber = "40077", Name = "Ernesto Alonso Reyes", Email = "ernesto.reyes@planta.mx", Role = UserRole.Administrator, IsActive = false }
        };

        _store.Users.AddRange(users);
    }

    private void SeedPlants()
    {
        _store.Plants.AddRange(
            new Plant { Id = 1, Code = "PT1", Name = "Planta Monterrey" },
            new Plant { Id = 2, Code = "PT2", Name = "Planta Querétaro" },
            new Plant { Id = 3, Code = "PT3", Name = "Planta Tijuana" });
    }

    private void SeedAreas()
    {
        _store.Areas.AddRange(
            new Area { Id = 1, Code = "MECA", Name = "Mecanizado", PlantId = 1 },
            new Area { Id = 2, Code = "ENS1", Name = "Ensamble Automotriz", PlantId = 1 },
            new Area { Id = 3, Code = "PINT", Name = "Pintura", PlantId = 1 },
            new Area { Id = 4, Code = "MANT", Name = "Mantenimiento", PlantId = 1 },
            new Area { Id = 5, Code = "INYE", Name = "Inyección de Plásticos", PlantId = 2 },
            new Area { Id = 6, Code = "ENS2", Name = "Ensamble Electrónico", PlantId = 2 },
            new Area { Id = 7, Code = "CALI", Name = "Calidad", PlantId = 2 },
            new Area { Id = 8, Code = "EMBA", Name = "Empaque", PlantId = 3 },
            new Area { Id = 9, Code = "HERR", Name = "Herramentales", PlantId = 3 },
            new Area { Id = 10, Code = "ALMA", Name = "Almacén", PlantId = 3 });
    }

    private void SeedMachines()
    {
        var machines = new List<Machine>
        {
            new() { Id = 1, Code = "CNC-01", Name = "Centro de mecanizado CNC 1", AreaId = 1 },
            new() { Id = 2, Code = "CNC-02", Name = "Centro de mecanizado CNC 2", AreaId = 1 },
            new() { Id = 3, Code = "TOR-01", Name = "Torno CNC horizontal", AreaId = 1 },
            new() { Id = 4, Code = "FRE-01", Name = "Fresadora vertical", AreaId = 1 },
            new() { Id = 5, Code = "LIN-ENS-1", Name = "Línea de ensamble 1", AreaId = 2 },
            new() { Id = 6, Code = "LIN-ENS-2", Name = "Línea de ensamble 2", AreaId = 2 },
            new() { Id = 7, Code = "PRE-200T", Name = "Prensa hidráulica 200 T", AreaId = 2 },
            new() { Id = 8, Code = "SOL-MIG-1", Name = "Celda de soldadura MIG 1", AreaId = 2 },
            new() { Id = 9, Code = "PIN-CAB-1", Name = "Cabina de pintura 1", AreaId = 3 },
            new() { Id = 10, Code = "HOR-CUR-1", Name = "Horno de curado", AreaId = 3 },
            new() { Id = 11, Code = "MANT-TAL-1", Name = "Taller de mantenimiento", AreaId = 4 },
            new() { Id = 12, Code = "INY-03", Name = "Inyectora 350 toneladas", AreaId = 5 },
            new() { Id = 13, Code = "INY-04", Name = "Inyectora 120 toneladas", AreaId = 5 },
            new() { Id = 14, Code = "SMT-LIN-1", Name = "Línea SMT electrónica", AreaId = 6 },
            new() { Id = 15, Code = "BAN-PRU-1", Name = "Banco de pruebas eléctricas", AreaId = 6 },
            new() { Id = 16, Code = "MET-MIC-1", Name = "Laboratorio metrología", AreaId = 7 },
            new() { Id = 17, Code = "EMP-AUT-1", Name = "Empacadora automática", AreaId = 8 },
            new() { Id = 18, Code = "AJE-MAN-2", Name = "Estación de ajuste manual", AreaId = 9 },
            new() { Id = 19, Code = "EDM-HIL-1", Name = "Alimentador de hilo EDM", AreaId = 9 },
            new() { Id = 20, Code = "MON-PAT-1", Name = "Montacargas de patios", AreaId = 10 }
        };

        _store.Machines.AddRange(machines);
    }

    private void SeedParts()
    {
        string[][] catalog =
        [
            ["Tornillo hexagonal M8 x 40 mm", "PZA", "A-01-02"],
            ["Sensor fotoeléctrico difuso 24 VDC", "PZA", "B-04-01"],
            ["Cable eléctrico flexible 2 m calibre 14", "M", "C-02-04"],
            ["Rodamiento de bolas 6204 ZZ", "PZA", "A-05-01"],
            ["Banda transportadora PU 3 mm", "M", "D-01-03"],
            ["Fusible tipo NH 32 A", "PZA", "B-02-02"],
            ["Válvula neumática 5/2 con solenoide", "PZA", "B-06-03"],
            ["Motor eléctrico 2 HP 3 fases", "PZA", "D-04-01"],
            ["Sello hidráulico 35x47x7", "PZA", "A-08-04"],
            ["Aceite hidráulico ISO 68", "L", "E-01-01"],
            ["Guantes de nitrilo talla L", "CJA", "F-03-02"],
            ["Disco de corte 4 1/2 pulgadas", "PZA", "G-02-01"],
            ["Broca helicoidal para metal 10 mm", "PZA", "G-01-05"],
            ["Contactor tripolar 40 A", "PZA", "B-01-04"],
            ["Manguera hidráulica R2 1/2 pulgada", "M", "C-05-02"],
            ["Tuerca hexagonal M10 zincada", "PZA", "A-01-04"],
            ["Rondana plana 3/8 acero inoxidable", "PZA", "A-01-05"],
            ["Termopar tipo J 150 mm", "PZA", "B-07-02"],
            ["Variador de frecuencia 5 HP", "PZA", "D-06-01"],
            ["Lubricante multiusos en aerosol", "PZA", "E-02-03"],
            ["Trapo industrial absorbente", "KG", "E-03-01"],
            ["Cinta aislante vinílica 19 mm", "RLL", "B-03-01"],
            ["Premoldado para banda transportadora", "PZA", "D-01-05"],
            ["Balero de agujas HK2020", "PZA", "A-05-03"],
            ["Filtro de aire para cabina de pintura", "PZA", "H-02-02"],
            ["Electrodo E6013 1/8 pulgada", "KG", "G-03-02"],
            ["Solución desengrasante industrial", "L", "E-02-05"],
            ["Cadena de rodillos No. 40", "M", "A-06-02"],
            ["Piñón para cadena No. 40 z=18", "PZA", "A-06-03"],
            ["Interruptor de límite IP67", "PZA", "B-05-01"],
            ["Fuente de alimentación 24 VDC 10 A", "PZA", "D-05-02"],
            ["Malla abrasiva para orbital", "CJA", "G-04-01"],
            ["Sello mecánico para bomba 1 pulgada", "PZA", "A-08-02"],
            ["Grasa EP litio grado 2", "KG", "E-01-03"],
            ["Tornillo prisionero M6 x 20", "PZA", "A-02-01"],
            ["Relevador térmico 9-13 A", "PZA", "B-01-06"],
            ["Cinta adhesiva de empaque 48 mm", "RLL", "F-01-02"],
            ["Caja de cartón corrugado 40x30x25", "PZA", "F-01-01"],
            ["Etiqueta adhesiva de embarque", "RLL", "F-01-04"],
            ["Película estirable 500 mm", "RLL", "F-02-01"],
            ["Manómetro glycerinado 0-160 psi", "PZA", "B-08-01"],
            ["Manguera neumática 8 mm", "M", "C-03-03"],
            ["Acople rápido neumático 1/4", "PZA", "B-06-05"],
            ["Vidrio de visión para horno", "PZA", "H-01-03"],
            ["Resistencia eléctrica tubular 2 kW", "PZA", "D-07-02"],
            ["Rodillo de acero para transportador", "PZA", "D-02-02"],
            ["Pistola de aire para limpieza", "PZA", "G-05-01"],
            ["Llave allen juego métrico 9 pzas", "CJA", "G-05-03"],
            ["Taladro portátil 1/2 pulgada", "PZA", "G-06-01"],
            ["Aceite para cadena de sierra", "L", "E-01-05"]
        ];

        for (var i = 0; i < catalog.Length; i++)
        {
            var partNumber = $"MAT-{10001 + i}";
            var (description, unit, location) = (catalog[i][0], catalog[i][1], catalog[i][2]);
            _store.Parts.Add(new Part
            {
                Id = i + 1,
                PartNumber = partNumber,
                Description = description,
                UnitOfMeasure = unit,
                DefaultLocation = location
            });

            if (i % 5 == 0)
            {
                var alternateRow = location[0] == 'A' ? 'B' : 'A';
                _store.PartLocations.Add(new PartLocation
                {
                    Id = _store.PartLocations.Count + 1,
                    PartId = i + 1,
                    LocationCode = $"{alternateRow}-{location[2..]}",
                    Description = "Ubicación alternativa"
                });
            }
        }
    }

    private void SeedShortageReasons()
    {
        _store.ShortageReasons.AddRange(
            new ShortageReason { Id = 1, Code = "OUT_OF_STOCK", Description = "Sin existencia" },
            new ShortageReason { Id = 2, Code = "INSUFFICIENT_INVENTORY", Description = "Inventario insuficiente" },
            new ShortageReason { Id = 3, Code = "DAMAGED_MATERIAL", Description = "Material dañado" },
            new ShortageReason { Id = 4, Code = "MATERIAL_NOT_FOUND", Description = "Material no encontrado" },
            new ShortageReason { Id = 5, Code = "DIFFERENT_LOCATION", Description = "Material en otra ubicación" },
            new ShortageReason { Id = 6, Code = "RESERVED_MATERIAL", Description = "Material reservado" },
            new ShortageReason { Id = 7, Code = "QUALITY_ISSUE", Description = "Problema de calidad" },
            new ShortageReason { Id = 8, Code = "OTHER", Description = "Otro", RequiresComment = true });
    }

    private void SeedRequisitions()
    {
        var today = DateTime.Today;
        var warehouseUser = _store.Users.First(u => u.Id == 5).Name;
        var warehouseUser2 = _store.Users.First(u => u.Id == 6).Name;

        Requisition Build(int id, string number, DateTime created, int requesterId, string requesterName,
            string employeeNumber, int plantId, int areaId, int machineId, RequisitionStatus status,
            IEnumerable<(int partId, decimal requested, decimal fulfilled, int? reasonId, string? comment)> items,
            DateTime? startedAt = null, DateTime? closedAt = null, string? closedBy = null, bool reviewedAll = false)
        {
            var requisition = new Requisition
            {
                Id = id,
                RequisitionNumber = number,
                CreatedAt = created.AddHours(8).AddMinutes(id * 7),
                RequestedDate = created,
                RequesterId = requesterId,
                RequesterName = requesterName,
                EmployeeNumber = employeeNumber,
                PlantId = plantId,
                AreaId = areaId,
                MachineId = machineId,
                Status = status,
                CreatedBy = requesterName,
                StartedAt = startedAt ?? (status is RequisitionStatus.Open ? null : created.AddHours(9)),
                ClosedAt = closedAt,
                ClosedBy = closedBy
            };

            foreach (var (partId, requested, fulfilled, reasonId, comment) in items)
            {
                var part = _store.Parts.First(p => p.Id == partId);
                var reason = reasonId.HasValue ? _store.ShortageReasons.First(r => r.Id == reasonId.Value) : null;
                var itemReviewed = reviewedAll || status is not (RequisitionStatus.Open);

                requisition.Items.Add(new RequisitionItem
                {
                    Id = requisition.Items.Count + 1,
                    RequisitionId = id,
                    PartId = part.Id,
                    PartNumber = part.PartNumber,
                    Description = part.Description,
                    RequestedQuantity = requested,
                    QuantityDescription = null,
                    UnitOfMeasure = part.UnitOfMeasure,
                    Location = part.DefaultLocation,
                    FulfilledQuantity = fulfilled,
                    FulfillmentStatus = ResolveStatus(requested, fulfilled),
                    ShortageReasonId = reason?.Id,
                    ShortageReasonDescription = reason?.Description,
                    ShortageComment = comment,
                    Reviewed = itemReviewed,
                    ReviewedAt = itemReviewed ? startedAt?.AddMinutes(30) : null
                });
            }

            return requisition;
        }

        static FulfillmentStatus ResolveStatus(decimal requested, decimal fulfilled) =>
            fulfilled switch
            {
                var q when q >= requested => FulfillmentStatus.Complete,
                > 0 => FulfillmentStatus.Partial,
                _ => FulfillmentStatus.NotFulfilled
            };

        _store.Requisitions.Add(Build(1, $"REQ-{today:yyyyMMdd}-0001", today.AddDays(-1), 1, "Laura Méndez Ríos", "10021", 1, 1, 1,
            RequisitionStatus.Open,
            [(1, 10, 0, null, null), (2, 2, 0, null, null), (3, 8, 0, null, null), (11, 2, 0, null, null)]));

        _store.Requisitions.Add(Build(2, $"REQ-{today:yyyyMMdd}-0002", today, 2, "Jorge Ramírez Castillo", "10088", 1, 2, 7,
            RequisitionStatus.Open,
            [(8, 1, 0, null, null), (15, 4, 0, null, null), (33, 1, 0, null, null)]));

        _store.Requisitions.Add(Build(3, $"REQ-{today.AddDays(-1):yyyyMMdd}-0001", today.AddDays(-2), 3, "Ana Sofía Herrera", "10145", 2, 5, 12,
            RequisitionStatus.InProgress,
            [(42, 12, 12, null, null), (43, 6, 4, 1, null), (27, 5, 0, null, null), (34, 2, 0, null, null)],
            startedAt: today.AddDays(-1)));

        _store.Requisitions.Add(Build(4, $"REQ-{today.AddDays(-1):yyyyMMdd}-0002", today.AddDays(-3), 4, "Miguel Ángel Torres", "10233", 1, 1, 2,
            RequisitionStatus.InProgress,
            [(13, 5, 5, null, null), (26, 3, 3, null, null), (28, 2, 2, null, null), (49, 1, 0, null, null), (21, 4, 0, null, null)],
            startedAt: today.AddDays(-1)));

        _store.Requisitions.Add(Build(5, $"REQ-{today.AddDays(-2):yyyyMMdd}-0001", today.AddDays(-2), 1, "Laura Méndez Ríos", "10021", 1, 1, 3,
            RequisitionStatus.Fulfilled,
            [(4, 6, 6, null, null), (29, 2, 2, null, null), (31, 1, 1, null, null), (16, 20, 20, null, null)],
            closedAt: today.AddDays(-2).AddHours(17), closedBy: warehouseUser, reviewedAll: true));

        _store.Requisitions.Add(Build(6, $"REQ-{today.AddDays(-3):yyyyMMdd}-0001", today.AddDays(-3), 2, "Jorge Ramírez Castillo", "10088", 1, 2, 8,
            RequisitionStatus.Fulfilled,
            [(25, 4, 4, null, null), (12, 10, 10, null, null), (22, 3, 3, null, null)],
            closedAt: today.AddDays(-3).AddHours(15), closedBy: warehouseUser2, reviewedAll: true));

        _store.Requisitions.Add(Build(7, $"REQ-{today.AddDays(-1):yyyyMMdd}-0003", today.AddDays(-1), 3, "Ana Sofía Herrera", "10145", 2, 6, 14,
            RequisitionStatus.PartiallyFulfilled,
            [(5, 8, 8, null, null), (23, 4, 2, 2, null), (46, 6, 0, 3, "Se encontraron con óxido en el rack B-04"), (38, 50, 50, null, null), (40, 2, 2, null, null)],
            closedAt: today.AddHours(11), closedBy: warehouseUser, reviewedAll: true));

        _store.Requisitions.Add(Build(8, $"REQ-{today.AddDays(-4):yyyyMMdd}-0001", today.AddDays(-4), 4, "Miguel Ángel Torres", "10233", 3, 8, 17,
            RequisitionStatus.PartiallyFulfilled,
            [(37, 6, 6, null, null), (39, 3, 1, 4, null), (41, 2, 0, 8, "El proveedor retrasó la entrega, llega el viernes")],
            closedAt: today.AddDays(-4).AddHours(16), closedBy: warehouseUser2, reviewedAll: true));

        _store.Requisitions.Add(Build(9, $"REQ-{today.AddDays(-7):yyyyMMdd}-0001", today.AddDays(-7), 1, "Laura Méndez Ríos", "10021", 1, 4, 11,
            RequisitionStatus.Closed,
            [(47, 2, 2, null, null), (44, 1, 1, null, null), (45, 3, 3, null, null)],
            closedAt: today.AddDays(-6).AddHours(10), closedBy: warehouseUser, reviewedAll: true));

        _store.Requisitions.Add(Build(10, $"REQ-{today.AddDays(-6):yyyyMMdd}-0001", today.AddDays(-6), 2, "Jorge Ramírez Castillo", "10088", 2, 7, 16,
            RequisitionStatus.Cancelled,
            [(30, 2, 0, null, null), (36, 4, 0, null, null)],
            closedAt: today.AddDays(-6).AddHours(13), closedBy: warehouseUser));
    }
}
