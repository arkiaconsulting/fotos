﻿namespace Fotos.Client.Api.Photos;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed record ExifMetadata(
    DateTime? DateTaken = default,
    string? CameraBrand = default,
    string? CameraModel = default,
    int? Iso = default,
    int? Width = default,
    int? Height = default,
    int? Aperture = default,
    int? FocalLength = default,
    bool IsFlash = false,
    string? ExposureTime = default,
    string? Lens = default,
    string? Quality = default);
