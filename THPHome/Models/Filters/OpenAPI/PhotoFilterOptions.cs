﻿using THPHome.Models.Filters.OpenAPI;

namespace WebUI.Models.Filters.OpenAPI;

public class PhotoFilterOptions : OpenApiFilterOptions
{
    public long? PostId { get; set; }
}
