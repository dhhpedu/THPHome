﻿using ApplicationCore.Entities;
using THPHome.Enums;

namespace THPHome.Models.Args.Posts;

public class AddPostArgs
{
    public string Title { get; set; } = default!;
    public string? Content { get; set; }
    public string? Description { get; set; }
    public PostType Type { get; set; }
    public string? Thumbnail { get; set; }
    public DateTime IssuedDate { get; set; }
    public int[] Categories { get; set; } = [];
    public int? CategoryId { get; set; }
    public List<Attachment>? Attachments { get; set; }
}

public class UpdatePostArgs : AddPostArgs
{
    public long Id { get; set; }
}
