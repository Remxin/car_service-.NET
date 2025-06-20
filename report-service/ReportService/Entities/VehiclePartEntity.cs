﻿namespace ReportService.Entities;

public class VehiclePartEntity {
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string PartNumber { get; set; } = default!;
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? AvailableQuantity { get; set; }
}