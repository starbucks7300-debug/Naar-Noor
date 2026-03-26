# Project Structure

Comprehensive guide to the architecture and organization of The Lost Yeti Kitchen & Bar project.

## Table of Contents

- [Directory Structure](#directory-structure)
- [Component Architecture](#component-architecture)
- [File Organization](#file-organization)
- [Naming Conventions](#naming-conventions)
- [Module System](#module-system)

## Directory Structure

```
lost-yeti-angular/
├── .angular/                    # Angular cache
├── .github/                     # GitHub configuration
│   └── workflows/              # CI/CD workflows
│       ├── ci.yml
│       ├── deploy.yml
│       └── security.yml
├── docs/                        # Documentation
│   ├── CHANGELOG.md
│   ├── CODE_OF_CONDUCT.md
│   ├── CONTRIBUTING.md
│   ├── DEPLOYMENT.md
│   ├── ERD.md
│   ├── FEATURES.md
│   ├── PROJECT_SETUP.md
│   ├── SECURITY.md
│   ├── STRUCTURE.md
│   ├── STYLES.md
│   ├── TECHNOLOGIES.md
│   ├── USE_CASES.md
│   └── CONTRIBUTORS.md
├── node_modules/                # Dependencies
├── src/                         # Source code
│   ├── app/                    # Application code
│   │   ├── components/         # Reusable components
│   │   │   ├── animated-background/
│   │   │   │   ├── animated-background.component.ts
│   │   │   │   ├── animated-background.component.html
│   │   │   │   └── animated-background.component.css
│   │   │   ├── custom-calendar/
│   │   │   │   ├── custom-calendar.component.ts
│   │   │   │   ├── custom-calendar.component.html
│   │   │   │   └── custom-calendar.component.css
│   │   │   ├── custom-dropdown/
│   │   │   │   ├── custom-dropdown.component.ts
│   │   │   │   ├── custom-dropdown.component.html
│   │   │   │   └── custom-dropdown.component.css
│   │   │   ├── footer/
│   │   │   │   ├── footer.component.ts
│   │   │   │   ├── footer.component.html
│   │   │   │   └── footer.component.css
│   │   │   └── header/
│   │   │       ├── header.component.ts
│   │   │       ├── header.component.html
│   │   │       └── header.component.css
│   │   ├── sections/           # Page sections
│   │   │   ├── about/
│   │   │   ├── blog/
│   │   │   ├── category/
│   │   │   ├── chefs/
│   │   │   ├── cinematic-banner/
│   │   │   ├── hero/
│   │   │   ├── locations/
│   │   │   ├── menu/
│   │   │   ├── reservation/
│   │   │   └── reviews/
│   │   ├── pages/              # Full pages (future)
│   │   ├── services/           # Services (future)
│   │   ├── models/             # TypeScript interfaces (future)
│   │   ├── guards/             # Route guards (future)
│   │   ├── interceptors/       # HTTP interceptors (future)
│   │   ├── app.component.ts    # Root component
│   │   └── app.routes.ts       # Routing configuration
│   ├── assets/                 # Static assets
│   │   ├── images/            # Images
│   │   ├── videos/            # Videos
│   │   └── fonts/             # Custom fonts (if any)
│   ├── environments/           # Environment configs (future)
│   ├── .htaccess              # Apache configuration
│   ├── favicon.ico            # Favicon
│   ├── favicon.svg            # SVG favicon
│   ├── index.html             # Main HTML file
│   ├── main.ts                # Application entry point
│   ├── robots.txt             # SEO robots file
│   ├── sitemap.xml            # SEO sitemap
│   └── styles.css             # Global styles
├── .editorconfig               # Editor configuration
├── .gitignore                  # Git ignore rules
├── angular.json                # Angular CLI configuration
├── package.json                # Dependencies and scripts
├── package-lock.json           # Locked dependencies
├── README.md                   # Project README
├── tsconfig.json               # TypeScript configuration
├── tsconfig.app.json           # App TypeScript config
└── tsconfig.spec.json          # Test TypeScript config
```

## Component Architecture

### Standalone Components

The project uses Angular 17's standalone component architecture:

```typescript
@Component({
  selector: 'app-example',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './example.component.html',
  styleUrls: ['./example.component.css']
})
export class ExampleComponent {
  // Component logic
}
```

### Component Types

#### 1. Layout Components
**Location**: `src/app/components/`

**Purpose**: Structural components used across the application

**Examples**:
- `HeaderComponent` - Navigation header
- `FooterComponent` - Site footer
- `AnimatedBackgroundComponent` - Global background

#### 2. Section Components
**Location**: `src/app/sections/`

**Purpose**: Page sections with specific content

**Examples**:
- `HeroComponent` - Hero section
- `ReservationComponent` - Booking form
- `MenuComponent` - Menu display

#### 3. Reusable Components
**Location**: `src/app/components/`

**Purpose**: Shared UI components

**Examples**:
- `CustomCalendarComponent` - Date picker
- `CustomDropdownComponent` - Select dropdown

### Component Communication

#### Parent to Child (Input)
```typescript
// Parent
<app-child [data]="parentData"></app-child>

// Child
@Input() data: string;
```

#### Child to Parent (Output)
```typescript
// Child
@Output() valueSelected = new EventEmitter<string>();

onSelect(value: string) {
  this.valueSelected.emit(value);
}

// Parent
<app-child (valueSelected)="handleValue($event)"></app-child>
```

#### Two-Way Binding
```typescript
// Component
[(ngModel)]="value"
```

## File Organization

### Component Files

Each component consists of three files:

```
component-name/
├── component-name.component.ts      # Logic
├── component-name.component.html    # Template
└── component-name.component.css     # Styles
```

### Component Structure

```typescript
// Imports
import { Component } from '@angular/core';

// Decorator
@Component({
  selector: 'app-component-name',
  standalone: true,
  imports: [],
  templateUrl: './component-name.component.html',
  styleUrls: ['./component-name.component.css']
})

// Class
export class ComponentNameComponent {
  // Properties
  property: string = 'value';
  
  // Constructor
  constructor() {}
  
  // Lifecycle hooks
  ngOnInit() {}
  
  // Methods
  method() {}
}
```

## Naming Conventions

### Files
- **Components**: `component-name.component.ts`
- **Services**: `service-name.service.ts`
- **Models**: `model-name.model.ts`
- **Guards**: `guard-name.guard.ts`
- **Pipes**: `pipe-name.pipe.ts`

### Classes
- **Components**: `ComponentNameComponent`
- **Services**: `ServiceNameService`
- **Interfaces**: `InterfaceName`
- **Enums**: `EnumName`

### Variables
- **Properties**: `camelCase`
- **Constants**: `UPPER_SNAKE_CASE`
- **Private**: `_privateProperty`

### CSS Classes
- **Utility**: Tailwind classes
- **Custom**: `kebab-case`
- **BEM**: `block__element--modifier` (if needed)

## Module System

### Standalone Architecture

No NgModules required. Each component imports its dependencies:

```typescript
@Component({
  standalone: true,
  imports: [
    CommonModule,      // *ngIf, *ngFor, etc.
    FormsModule,       // ngModel
    RouterModule,      // routing
    CustomComponent    // other components
  ]
})
```

### Routing

**File**: `src/app/app.routes.ts`

```typescript
export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'about', component: AboutComponent },
  // Lazy loading
  {
    path: 'admin',
    loadComponent: () => import('./admin/admin.component')
      .then(m => m.AdminComponent)
  }
];
```

### Bootstrap

**File**: `src/main.ts`

```typescript
bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    // other providers
  ]
});
```

## Data Flow

### Application State

```
User Interaction
      ↓
  Component
      ↓
   Service (future)
      ↓
  API Call (future)
      ↓
   Response
      ↓
  Component
      ↓
   Template
      ↓
    View
```

### Form Data Flow

```
User Input
    ↓
[(ngModel)]
    ↓
Component Property
    ↓
Form Submit
    ↓
Validation
    ↓
Service/API (future)
```

## Scalability Considerations

### Future Structure

As the application grows, consider:

```
src/app/
├── core/                    # Core functionality
│   ├── services/           # Singleton services
│   ├── guards/             # Route guards
│   ├── interceptors/       # HTTP interceptors
│   └── models/             # Shared models
├── shared/                  # Shared module
│   ├── components/         # Shared components
│   ├── directives/         # Shared directives
│   └── pipes/              # Shared pipes
├── features/                # Feature modules
│   ├── auth/              # Authentication
│   ├── admin/             # Admin panel
│   └── ordering/          # Online ordering
└── layout/                  # Layout components
```

### State Management

For complex state, consider:
- NgRx (Redux pattern)
- Akita
- RxJS BehaviorSubject

### Code Splitting

Implement lazy loading:
```typescript
{
  path: 'feature',
  loadChildren: () => import('./feature/feature.routes')
}
```

## Best Practices

### Component Design
- Single responsibility
- Small, focused components
- Reusable where possible
- Clear naming

### File Organization
- Group related files
- Consistent structure
- Logical hierarchy
- Easy navigation

### Code Organization
- Imports at top
- Decorator next
- Class last
- Lifecycle hooks in order

### Performance
- OnPush change detection
- TrackBy for *ngFor
- Lazy loading
- Unsubscribe from observables

## Architecture Patterns

### Smart vs Presentational

**Smart Components** (Container):
- Handle business logic
- Manage state
- API calls
- Route parameters

**Presentational Components**:
- Display data
- Emit events
- No business logic
- Reusable

### Example

```typescript
// Smart Component
@Component({
  selector: 'app-reservation-container',
  template: `
    <app-reservation-form
      [timeSlots]="timeSlots"
      (submit)="onSubmit($event)">
    </app-reservation-form>
  `
})
export class ReservationContainerComponent {
  timeSlots = ['17:00', '18:00', '19:00'];
  
  onSubmit(data: Reservation) {
    // Handle submission
  }
}

// Presentational Component
@Component({
  selector: 'app-reservation-form',
  template: `<!-- Form UI -->`
})
export class ReservationFormComponent {
  @Input() timeSlots: string[];
  @Output() submit = new EventEmitter<Reservation>();
}
```

## Testing Structure

```
component-name/
├── component-name.component.ts
├── component-name.component.html
├── component-name.component.css
└── component-name.component.spec.ts    # Unit tests
```

## Documentation Standards

Each component should include:

```typescript
/**
 * Component description
 * 
 * @example
 * <app-component [input]="value"></app-component>
 */
@Component({...})
export class ComponentName {
  /**
   * Property description
   */
  @Input() property: string;
  
  /**
   * Method description
   * @param param - Parameter description
   * @returns Return value description
   */
  method(param: string): void {
    // Implementation
  }
}
```

---

For more details, see:
- [FEATURES.md](./FEATURES.md) - Feature documentation
- [TECHNOLOGIES.md](./TECHNOLOGIES.md) - Technology stack
- [STYLES.md](./STYLES.md) - Styling guidelines
