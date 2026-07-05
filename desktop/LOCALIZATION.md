# Localization & Bilingual Support Documentation

## Overview

The Naar-Noor Desktop Application provides comprehensive bilingual support with English (en) and Arabic (ar) localization. The implementation includes runtime culture switching without application restart, RTL/LTR layout support, and persistent language preferences.

**Requirements Implemented:**
- REQ-121: Bilingual support (English/Arabic)
- REQ-122: Runtime culture switching and RTL/LTR support

## Architecture

### Core Components

#### 1. LocalizationService
Located in: `src/NaarNoor.Desktop.Common/Services/Implementations/LocalizationService.cs`

**Responsibilities:**
- Load localization resources from JSON files (Resources/en.json, Resources/ar.json)
- Manage current culture state
- Provide string lookup via `GetString(key)` and `GetString(key, ...args)`
- Support runtime culture switching via `SetCulture(cultureName)`
- Fire `CultureChanged` observable for UI updates
- Persist culture preference to app configuration

**Key Methods:**
```csharp
// Get localized string
string GetString(string key)
string GetString(string key, params object[] args)

// Switch culture at runtime (updates Thread.CurrentThread.CurrentUICulture)
void SetCulture(string cultureName)

// Load resources from JSON files
Task LoadResourcesAsync()

// Check RTL status
bool IsRightToLeft { get; }

// Get text alignment for current culture
TextAlignment GetTextAlignment()

// Observable for culture changes
IObservable<string> CultureChanged { get; }
```

#### 2. ILocalizationService Interface
Located in: `src/NaarNoor.Desktop.Common/Services/Interfaces/ILocalizationService.cs`

Defines the contract for localization services with support for:
- Dot-notation resource keys (e.g., "Login.Username")
- Format argument substitution
- RTL/LTR detection
- Culture change notifications via Reactive Extensions

#### 3. LocalizationHelper Utility
Located in: `src/NaarNoor.Desktop.WinForms/Utilities/LocalizationHelper.cs`

Provides utilities for WinForms controls:
- `ApplyLayoutDirection(control, localizationService)`: Applies RTL/LTR to all controls recursively
- `GetLocalizedText(resourceKey, localizationService)`: Gets translated text
- `UpdateLocalizedStrings(control, localizationService)`: Updates all UI text after culture change
- `ConfigureFlowPanel(panel, localizationService)`: Configures FlowLayoutPanel for bidirectional flow

### Resource Files

Located in: `src/NaarNoor.Desktop.WinForms/Resources/`

#### en.json (English Resources)
Contains 200+ English strings organized hierarchically:
- `AppTitle`: Application title
- `Login.*`: Login form strings
- `Dashboard.*`: Dashboard view strings
- `Reservations.*`: Reservation management strings
- `Menu.*`: Menu management strings
- `Staff.*`: Staff management strings
- `Reports.*`: Reports and analytics strings
- `Common.*`: Common UI elements
- `Validation.*`: Validation messages
- `TimeFormat.*`: Date/time formatting
- `Currency.*`: Currency formatting
- `ErrorMessages.*`: Error message templates
- `ConfirmationMessages.*`: Confirmation dialogs
- `SuccessMessages.*`: Success notifications

#### ar.json (Arabic Resources)
Complete Arabic translations of all resource keys with proper RTL text.

### Integration Points

#### 1. Application Startup (Program.cs)
```csharp
// Load localization resources during startup
var localizationService = serviceProvider.GetRequiredService<ILocalizationService>();
localizationService.LoadResourcesAsync().GetAwaiter().GetResult();
```

#### 2. DashboardForm Integration
- Subscribes to `CultureChanged` observable
- Applies layout direction on initialization and culture change
- Updates all UI strings when culture changes

#### 3. LoginForm Integration
- Applies RTL/LTR layout based on current culture
- Updates form strings when culture changes
- Provides language selector

#### 4. Dependency Injection
Registered in `ServiceConfiguration.cs`:
```csharp
services.AddSingleton<ILocalizationService, LocalizationService>();
```

## Runtime Culture Switching

### Flow

1. **User selects language** in UI
2. **SetCulture() called** with new culture code ("en" or "ar")
3. **Validation**: Ensure culture is supported
4. **Thread.CurrentThread.CurrentUICulture updated** for date/number formatting
5. **CultureChanged event fired** to notify all subscribers
6. **Configuration persisted** to app-config.json via ConfigurationService
7. **All UI elements updated** via CultureChanged subscription

### Example
```csharp
// Switch to Arabic
_localizationService.SetCulture("ar");

// This triggers:
// 1. Thread culture update
// 2. CultureChanged.OnNext("ar")
// 3. Config persistence
// 4. All forms update their UI strings and layout
```

## RTL/LTR Support

### Implementation

#### Windows Forms (WinForms) Approach
- Uses `Control.RightToLeft` property
- Recursively applies to all child controls
- Adjusts text alignment for each control type:
  - Labels: TopLeft (LTR) / TopRight (RTL)
  - Buttons: MiddleLeft (LTR) / MiddleRight (RTL)
  - TextBoxes: Left (LTR) / Right (RTL)
- FlowLayoutPanel: LeftToRight (LTR) / RightToLeft (RTL)

#### Usage
```csharp
// In form initialization
LocalizationHelper.ApplyLayoutDirection(this, _localizationService);

// Subscribe to culture changes
_cultureChangeSubscription = _localizationService.CultureChanged.Subscribe(_ =>
{
    LocalizationHelper.ApplyLayoutDirection(this, _localizationService);
    UpdateLocalizedText();
});
```

### Migration Path to WPF
For future WPF migration, use:
- `FlowDirection` property: `FlowDirection.LeftToRight` (LTR) / `FlowDirection.RightToLeft` (RTL)
- `TextAlignment` property in XAML
- `Culture` and `Language` attributes in XAML

## Resource Loading

### File Discovery
1. First tries: `%APPDATA%\NaarNoor\Resources\{culture}.json`
2. Fallback: `{AppContext.BaseDirectory}\Resources\{culture}.json`

### Resource Format
```json
{
  "Section": {
    "Key": "Value",
    "Key2": "Value with {0} placeholder"
  }
}
```

### Dot-Notation Resolution
Resources are flattened to support dot notation:
- `Section.Key` → `GetString("Section.Key")`
- Supports nested structures
- JsonElement types handled for various value types

## Testing

### Test Coverage
Located in: `src/NaarNoor.Desktop.Tests/Services/LocalizationServiceTests.cs`

**26 comprehensive tests covering:**
- Constructor initialization and error handling
- String retrieval with various input types
- Culture switching and validation
- CultureChanged event firing
- RTL/LTR property verification
- Thread culture updates
- Configuration persistence
- Format argument substitution
- Concurrent culture switching (thread safety)
- Graceful handling of missing resources

**All tests passing:**
```
Passed!  - Failed: 0, Passed: 26, Skipped: 0, Total: 26
```

## Configuration

### app-config.json
```json
{
  "Culture": "en",
  "CacheTtlMinutes": 30,
  "EnableOfflineMode": true
}
```

The selected culture is persisted after runtime switching.

## Best Practices

### 1. Resource Key Naming
- Use hierarchical dot notation: `Feature.Component.Element`
- Examples: `Login.UsernameLabel`, `Dashboard.RevenueTitle`
- Keep keys lowercase for consistency

### 2. Format Strings
- Use standard .NET format placeholders: `{0}`, `{1}`
- Example: `"Welcome, {0}!"` → `GetString("Dashboard.Welcome", userName)`

### 3. Bidirectional Text Handling
- Arabic text containing English words is properly supported
- Never hardcode mixed EN/AR text
- Always use resource keys

### 4. Form Updates
- Always subscribe to `CultureChanged` in forms that show dynamic content
- Update both text and layout direction on culture change
- Use `LocalizationHelper` for consistent behavior

### 5. Performance
- Resources are loaded once at startup
- Culture switching is O(1) - just updates a reference
- No reload of UI required, just observable notification

## Future Enhancements

### Planned Features
1. **Additional Languages**: Add support for more languages (fr, de, es)
2. **Pluralization**: Implement proper pluralization rules per language
3. **Date/Time Formatting**: Locale-specific calendar and time formats
4. **Currency Formatting**: Locale-specific currency symbols and decimal separators
5. **Right-to-Left Checkbox**: User preference UI for RTL/LTR override
6. **Resource Hot Reload**: Support updating resources without restart
7. **WPF Migration**: Transition from WinForms to WPF with improved binding support

### WPF Migration Notes
- WPF's `FlowDirection` binding will simplify RTL implementation
- XAML `Language` attribute supports culture-specific rendering
- StringFormat binding converter can replace `GetString()` calls
- Proper resource dictionary support in XAML

## Troubleshooting

### Culture Not Persisting
**Issue**: Culture reverts after application restart

**Solution**: 
- Verify `ConfigurationService.SaveConfigurationAsync()` is called
- Check write permissions to `%APPDATA%\NaarNoor\`
- Ensure `app-config.json` file is created

### RTL Layout Incorrect
**Issue**: Some controls not aligned properly for Arabic

**Solution**:
- Call `LocalizationHelper.ApplyLayoutDirection()` for all custom controls
- Check that `RightToLeft` property is properly set
- Verify `TextAlign` is set to `TopRight` for RTL

### Resource Keys Not Found
**Issue**: UI shows resource key instead of translated text

**Solution**:
- Verify JSON files are in `Resources/` directory
- Check key name matches exactly (case-sensitive dot notation)
- Call `LoadResourcesAsync()` during startup
- Check JSON syntax is valid

## Files Modified/Created

### New Files
- `LocalizationHelper.cs` - WinForms layout and text utilities
- `LocalizationServiceTests.cs` - Comprehensive test suite (26 tests)

### Modified Files
- `LocalizationService.cs` - Updated to load from JSON resources
- `ILocalizationService.cs` - Added `LoadResourcesAsync()` method
- `Program.cs` - Added resource loading call
- `DashboardForm.cs` - Added RTL/LTR and culture change support
- `LoginForm.cs` - Added RTL/LTR and culture change support

### Resource Files
- `Resources/en.json` - 200+ English strings
- `Resources/ar.json` - 200+ Arabic translations

## References

- .NET Globalization: https://docs.microsoft.com/en-us/dotnet/api/system.globalization
- Reactive Extensions (Rx.NET): https://reactivex.io/
- WinForms RTL Support: https://docs.microsoft.com/en-us/dotnet/desktop/winforms/right-to-left-support
- JSON Serialization: https://docs.microsoft.com/en-us/dotnet/api/system.text.json

