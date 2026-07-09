using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Content.Server.Database;

public static class DenModel
{
    [Table("denu_settings")]
    public sealed class DenuSettings
    {
        [Key]
        [Column("player_user_id")]
        public Guid PlayerUserId { get; set; }

        [Column("settings", TypeName = "jsonb")]
        public string SettingsJson { get; set; } = "{}";

        public Player Player { get; set; } = null!;
    }

    public class LoadoutProfile
    {
        public int Id { get; set; }

        public int LoadoutCategoryId {  get; set; }

        public LoadoutCategory Category { get; set; } = null!;

        public Guid LoadoutUniqueId { get; set; }

        [MaxLength(256)]
        public string LoadoutName { get; set; } = string.Empty;

        public int Priority { get; set; }

        public List<string> LoadoutItems { get; set; } = new();
    }

    public class JobLoadout
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; } = null!;

        [MaxLength(256)]
        public string JobName { get; set; } = string.Empty;

        public List<Guid> LoadoutProfiles { get; set; } = new();
    }

    public class LoadoutCategory
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; } = null!;

        public Guid CategoryUniqueId { get; set; }

        public int Priority { get; set; }

        [MaxLength(256)]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(10)]
        public string CategoryColor { get; set; } = string.Empty;

        public List<LoadoutProfile> Members { get; set; } = new();
    }
}
