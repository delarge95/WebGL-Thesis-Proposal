---
tipo: sistema
estado: mantenido
---
#sistema #configuracion_tecnica
# Contributing to WebGL Drone Visualization

Thank you for your interest in contributing to this project!

## 🏗️ Project Setup

1. Clone the repository
2. Open `desarrollo/unity_project` in Unity 6.0 LTS or higher
3. Ensure Universal Render Pipeline (URP) is configured

## 📁 Code Organization

### Folder Structure

```
Assets/Scripts/
├── Core/
│   ├── Content/       # Components attached to scene objects
│   ├── Data/          # ScriptableObjects and data classes
│   ├── Events/        # Event definitions and EventBus
│   ├── Managers/      # Singleton managers
│   └── Utils/         # Utility classes and helpers
├── UI/                # UI Toolkit components
└── Shaders/           # HLSL shader files (in Assets/Shaders/)
```

### Naming Conventions

| Type | Convention | Example |
|------|------------|---------|
| Classes | PascalCase | `SelectionManager` |
| Methods | PascalCase | `HandleClick()` |
| Private fields | camelCase with underscore or no prefix | `_currentSelection` or `currentSelection` |
| Public properties | PascalCase | `CurrentState` |
| Constants | UPPER_SNAKE_CASE | `MAX_POLYGONS` |
| Events | PascalCase + Event suffix | `PartSelectedEvent` |

### Code Style

1. **All comments must be in English**
2. **Use XML documentation for public APIs**:
   ```csharp
   /// <summary>
   /// Selects the specified part and fires selection event.
   /// </summary>
   /// <param name="part">The part to select.</param>
   public void SelectPart(Transform part)
   ```

3. **Use regions sparingly**, prefer smaller classes
4. **Follow Unity's component pattern**
5. **Use `[SerializeField]` instead of `public` for Inspector fields**

## 🎯 Architecture Patterns

### Singleton Pattern
Use `Singleton<T>` or `PersistentSingleton<T>` for managers:

```csharp
public class MyManager : Singleton<MyManager>
{
    // Access via MyManager.Instance
}
```

### Event Bus Pattern
Use EventBus for decoupled communication:

```csharp
// Subscribe
EventBus.Subscribe<PartSelectedEvent>(OnPartSelected);

// Publish
EventBus.Publish(new PartSelectedEvent(data));

// Unsubscribe (important!)
EventBus.Unsubscribe<PartSelectedEvent>(OnPartSelected);
```

### ScriptableObjects
Use for configuration and data:

```csharp
[CreateAssetMenu(fileName = "NewPart", menuName = "WebGL/Part Data")]
public class DronePartData : ScriptableObject
{
    // Data fields
}
```

## 🧪 Testing

- Test in Unity Editor first
- Build WebGL and test in Chrome/Firefox
- Check browser console for errors
- Use `F3` to show performance metrics

## ✅ Pull Request Checklist

- [ ] All comments in English
- [ ] XML documentation for public methods
- [ ] No compiler warnings
- [ ] Tested in Unity Editor
- [ ] Tested WebGL build (if applicable)
- [ ] Follows naming conventions

## 📝 Commit Messages

Use conventional commits:

```
feat: Add new measurement tool
fix: Correct rotation calculation
docs: Update README with new features
refactor: Simplify event handling
```

## 📄 License

This is an academic project. Contact the author for licensing inquiries.


