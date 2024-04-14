﻿namespace Models;

public class FileData
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required byte[] Data { get; set; }
}
