using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using JetBrains.Annotations;
using UnityEngine;
// ReSharper disable RedundantDefaultMemberInitializer

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace MenuFloaters.Configuration;

[UsedImplicitly]
internal class PluginConfig
{
    public static PluginConfig Instance { get; set; } = null!;

    public virtual bool Enabled { get; set; } = true;
    public virtual int NoteCount { get; set; } = 100;
    public virtual bool AlsoDisableDefaultPiles { get; set; } = false;
    public virtual bool IsFullSphere { get; set; } = false;
    public virtual Vector3 SpawnScale { get; set; } = new (11f, 3.5f, 11f);
    public virtual float SpawnRadius { get; set; } = 9f;
    public virtual float MinSpawnHeightPercentage { get; set; } = 0f;
    public virtual float MaxSpawnHeightPercentage { get; set; } = 1f;
    public virtual float CoalesceAmount { get; set; } = 0.67f;
    public virtual float ArrowChance { get; set; } = 0.1f;
    public virtual bool ForceNoteColor { get; set; } = false;
    public virtual Color NoteColor { get; set; } = Color.white;
    public virtual bool NoteColorRandom { get; set; } = false;
}