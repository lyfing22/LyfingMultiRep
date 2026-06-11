using DataMigrate.Models;

namespace DataMigrate.Core;

public record SourceDependencies(
    string ConnectionString,
    IdentityConfig Identity,
    Dictionary<string, DeviceDptInfo> ModalityDepartment
);
