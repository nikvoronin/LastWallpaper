﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LastWallpaper.Models;

/// <summary>
/// Description of common image of the day.
/// </summary>
public class Imago
{
    public required string Filename { get; init; }
    public required DateTime Created { get; init; }
    public string? Copyright { get; init; }
    public string? Title { get; init; }
}