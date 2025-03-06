using System;
using System.ComponentModel;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using Zenject;
using MenuFloaters.Configuration;
using JetBrains.Annotations;
using MenuFloaters.Managers;
using UnityEngine;

namespace MenuFloaters.UI;

[UsedImplicitly]
internal class SettingsMenuManager : IInitializable, IDisposable, INotifyPropertyChanged
{
    private static PluginConfig Config => PluginConfig.Instance;
    private static LevitatingNoteManager? _levitatingNoteManager;
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private const string MenuName = nameof(MenuFloaters);
    private const string ResourcePath = nameof(MenuFloaters) + ".UI.BSML.Settings.bsml";

    [UsedImplicitly] public string VectorXFormatter(float x) => $"X: {x}";
    [UsedImplicitly] public string VectorYFormatter(float y) => $"Y: {y}";
    [UsedImplicitly] public string VectorZFormatter(float z) => $"Z: {z}";
    [UsedImplicitly] public string PercentageFormatter(float x) => x.ToString("0%");
    
    // ReSharper disable FieldCanBeMadeReadOnly.Local
    [UIComponent("SpawnAreaSliderX")]
    private SliderSetting _spawnAreaSliderX = null!;
    [UIComponent("SpawnAreaSliderY")]
    private SliderSetting _spawnAreaSliderY = null!;
    [UIComponent("SpawnAreaSliderZ")]
    private SliderSetting _spawnAreaSliderZ = null!;
    // ReSharper restore FieldCanBeMadeReadOnly.Local

    public SettingsMenuManager(LevitatingNoteManager levitatingNoteManager)
    {
        _levitatingNoteManager = levitatingNoteManager;
    }

    public void Initialize()
    {
        BeatSaberMarkupLanguage.Settings.BSMLSettings.Instance?.AddSettingsMenu(MenuName, ResourcePath, this);
    }

    public void Dispose()
    {
        BeatSaberMarkupLanguage.Settings.BSMLSettings.Instance?.RemoveSettingsMenu(this);
    }

    [UIAction("#post-parse")]
    [UsedImplicitly]
    private void PostParse()
    {
        if (_spawnAreaSliderX != null)
        {
            CurvedTextMeshPro tmpComponent = _spawnAreaSliderX.Slider.GetComponentInChildren<CurvedTextMeshPro>();
            if (tmpComponent != null)
            {
                tmpComponent.color = new Color(1f, 0.67f, 0.67f);
            }
        }
        
        if (_spawnAreaSliderY != null)
        {
            CurvedTextMeshPro tmpComponent = _spawnAreaSliderY.Slider.GetComponentInChildren<CurvedTextMeshPro>();
            if (tmpComponent != null)
            {
                tmpComponent.color = new Color(0.67f, 1f, 0.67f);
            }
        }
        
        if (_spawnAreaSliderZ != null)
        {
            CurvedTextMeshPro tmpComponent = _spawnAreaSliderZ.Slider.GetComponentInChildren<CurvedTextMeshPro>();
            if (tmpComponent != null)
            {
                tmpComponent.color = new Color(0.67f, 0.67f, 1f);
            }
        }
    }

    protected bool Enabled
    {
        get => Config.Enabled;
        set
        {
            Config.Enabled = value;
            
            if (value)
            {
                _levitatingNoteManager?.Enable();
            }
            else
            {
                _levitatingNoteManager?.Disable();
            }
        }
    }

    protected int NoteCount
    {
        get => Config.NoteCount;
        set => Config.NoteCount = Math.Max(value, 0);
    }
    protected bool AlsoDisableDefaultPiles
    {
        get => Config.AlsoDisableDefaultPiles;
        set
        {
            Config.AlsoDisableDefaultPiles = value;
            _levitatingNoteManager?.UpdateDefaultPiles();
        }
    }

    protected bool IsFullSphere
    {
        get => Config.IsFullSphere;
        set => Config.IsFullSphere = value;
    }
    protected float SpawnScaleX
    {
        get => Config.SpawnScale.x;
        set => Config.SpawnScale = Config.SpawnScale with { x = Mathf.Max(value, 0f) };
    }
    protected float SpawnScaleY
    {
        get => Config.SpawnScale.y;
        set => Config.SpawnScale = Config.SpawnScale with { y = Mathf.Max(value, 0f) };
    }
    protected float SpawnScaleZ
    {
        get => Config.SpawnScale.z;
        set => Config.SpawnScale = Config.SpawnScale with { z = Mathf.Max(value, 0f) };
    }
    
    protected float SpawnRadius
    {
        get => Config.SpawnRadius;
        set => Config.SpawnRadius = Mathf.Max(value, 0f);
    }
    
    protected float MinSpawnHeightPercentage
    {
        get => Config.MinSpawnHeightPercentage;
        set => Config.MinSpawnHeightPercentage = Mathf.Clamp(value, 0f, 1f);
    }
    protected float MaxSpawnHeightPercentage
    {
        get => Config.MaxSpawnHeightPercentage;
        set => Config.MaxSpawnHeightPercentage = Mathf.Clamp(value, 0f, 1f);
    }

    protected float CoalesceAmount
    {
        get => Config.CoalesceAmount;
        set => Config.CoalesceAmount = Mathf.Clamp(value, 0f, 1f);
    }

    protected float ArrowChance
    {
        get => Config.ArrowChance;
        set => Config.ArrowChance = Mathf.Clamp(value, 0f, 1f);
    }

    [UIValue("ForceNoteColor")]
    protected bool ForceNoteColor
    {
        get => Config.ForceNoteColor;
        set
        {
            Config.ForceNoteColor = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForceNoteColor)));
        }
    }
    protected Color NoteColor
    {
        get => Config.NoteColor;
        set => Config.NoteColor = value;
    }
    protected bool NoteColorRandom
    {
        get => Config.NoteColorRandom;
        set => Config.NoteColorRandom = value;
    }

    [UIAction("SpawnNotes")]
    [UsedImplicitly]
    private void SpawnNotes()
    {
        _levitatingNoteManager?.SpawnNotes();
    }
}