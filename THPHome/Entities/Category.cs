﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using THPCore.Infrastructures;

namespace THPHome.Entities;

public class Category : BaseEntity<int>
{
    public int? ParentId { get; set; }
    [StringLength(100)]
    public string Name { get; set; } = default!;
    [StringLength(100)]
    public string? NormalizeName { get; set; }
    [StringLength(300)]
    public string? Description { get; set; }
    [StringLength(300)]
    public string? Thumbnail { get; set; }
    public CategoryStatus? Status { get; set; }
    public int Index { get; set; }
    public bool IsDisplayOnHome { get; set; }
    public string? Icon { get; set; }
    [StringLength(10)]
    public string Locale { get; set; } = "vi-VN";
    [ForeignKey(nameof(Department))]
    public int? DepartmentId { get; set; }

    public Department? Department { get; set; }
    public ICollection<Post>? Posts { get; set; }
}

public enum CategoryStatus
{
    Draft = 0,
    Active = 1
}
