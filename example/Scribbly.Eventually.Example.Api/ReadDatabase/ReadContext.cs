using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scribbly.Eventually.Example.Api.ReadDatabase;

public class ReadContext : DbContext
{
    public ReadContext(DbContextOptions<ReadContext> options) : base(options)
    { }

    public DbSet<BoxStatus> BoxStatuses 
        => Set<BoxStatus>();
    public DbSet<ProjectionCheckpoint> ProjectionCheckpoints 
        => Set<ProjectionCheckpoint>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CheckpointMapping());
    }
}

internal class CheckpointMapping : IEntityTypeConfiguration<ProjectionCheckpoint>
{
    public void Configure(EntityTypeBuilder<ProjectionCheckpoint> builder)
    {
        builder.HasKey(e => e.ProjectionName);
        
        builder.Property(e => e.ProjectionName)
            .HasMaxLength(256)
            .HasColumnType("varchar");
    }
}