using System;
using JetBrains.Annotations;
using MenuFloaters.Configuration;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace MenuFloaters.Managers;

[UsedImplicitly]
internal class LevitatingNoteManager : IInitializable, IDisposable
{
    private static PluginConfig Config => PluginConfig.Instance;
    
    private static Transform? _menuEnvironment;
    private static GameObject? _levitatingNotePrefab;
    
    private readonly GameObject _container = new ("LevitatingNoteManager");
    
    private static readonly int FogHeightOffsetID = Shader.PropertyToID("_FogHeightOffset");
    
    private static Vector2 PointOnCircle(float radius, float angle, Vector2 origin)
    {
        float x = (radius * Mathf.Cos(angle * Mathf.PI / 180f)) + origin.x;
        float y = (radius * Mathf.Sin(angle * Mathf.PI / 180f)) + origin.y;

        return new Vector2(x, y);
    }

    private static Vector3 GetRandomNotePosition()
    {
        float positionOffset = Config.SpawnRadius / 2;
        float maxY = (Config.SpawnScale.y + positionOffset) * Config.MaxSpawnHeightPercentage;
        float minY = Config.IsFullSphere ? -maxY : 0;

        Vector2 randomPoint = PointOnCircle(1f, Random.Range(-180f, 180f), Vector2.zero);
        Vector3 position = new Vector3()
        {
            x = (randomPoint.x * Config.SpawnScale.x) + Random.Range(-positionOffset, positionOffset),
            y = Random.Range(minY, maxY),
            z = (randomPoint.y * Config.SpawnScale.z) + Random.Range(-positionOffset, positionOffset)
        };

        if (!Mathf.Approximately(Config.CoalesceAmount, 0f))
        {
            position = Vector3.Lerp(position,
                new Vector3(0, position.y < 0 ? -Config.SpawnScale.y : Config.SpawnScale.y, 0),
                Mathf.Abs(position.y) / maxY * Config.CoalesceAmount);
        }

        return position;
    }

    public void Initialize()
    {
        if (_menuEnvironment == null)
        {
            _menuEnvironment = GameObject.Find("DefaultMenuEnvironment").transform;
            _container.transform.SetParent(_menuEnvironment);
        }
        if (_levitatingNotePrefab == null)
        {
            _levitatingNotePrefab = _menuEnvironment.Find("Notes").Find("LevitatingNote").gameObject;
        }
        
        UpdateDefaultPiles();
        SpawnNotes();
    }

    private void ClearNotes()
    {
        while (_container.transform.childCount != 0)
        {
            Object.DestroyImmediate(_container.transform.GetChild(0).gameObject);
        }
    }

    internal void SpawnNotes()
    {
        ClearNotes();
        
        for (int i = 0; i < Config.NoteCount; i++)
        {
            GameObject note = Object.Instantiate(_levitatingNotePrefab, _container.transform)!;
            Transform? noteObject = note.transform.GetChild(0);
            Object.DestroyImmediate(note.transform.Find("Shadow").gameObject);
            Object.DestroyImmediate(noteObject.GetComponent<BoxCollider>());
            
            note.transform.position = GetRandomNotePosition();
            note.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), Random.Range(-15f, 15f));

            if (Random.value < Config.ArrowChance)
            {
                noteObject.Find("NoteArrow").gameObject.SetActive(true);
                noteObject.Find("NoteArrowGlow").gameObject.SetActive(true);
            }

            if (noteObject.TryGetComponent(out Renderer renderer))
            {
                if (Config.ForceNoteColor)
                {
                    Color color = Config.NoteColorRandom ? Color.HSVToRGB(Random.value, 1f, 1f) : Config.NoteColor;
                    renderer.material.color = color;

                    if (noteObject.Find("NoteArrowGlow").TryGetComponent(out SpriteRenderer arrowGlow))
                    {
                        arrowGlow.color = color with { a = 0.5f };
                    }
                }
                
                if (Config.IsFullSphere)
                {
                    renderer.material.SetFloat(FogHeightOffsetID, 99999f);
                }
            }

            if (note.TryGetComponent(out Animation animation))
            {
                foreach (AnimationState state in animation)
                {
                    state.speed = Random.Range(0.9f, 1.1f);
                    state.time = Random.Range(0f, 30f);
                }
            }
        }
    }

    public void Dispose()
    {
        Object.DestroyImmediate(_container);
        _menuEnvironment?.Find("Notes").gameObject.SetActive(true);
    }
    
    public void Enable()
    {
        _container.SetActive(true);
        UpdateDefaultPiles();
    }
    public void Disable()
    {
        _container.SetActive(false);
        _menuEnvironment?.Find("Notes").gameObject.SetActive(true);
    }

    public void UpdateDefaultPiles()
    {
        _menuEnvironment?.Find("Notes").gameObject.SetActive(!Config.AlsoDisableDefaultPiles);
        _menuEnvironment?.Find("PileOfNotes").gameObject.SetActive(!Config.AlsoDisableDefaultPiles);
    }
}
