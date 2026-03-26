# Contributing to The Lost Yeti Kitchen & Bar

Thank you for your interest in contributing! This document provides guidelines and instructions for contributing to the project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Commit Guidelines](#commit-guidelines)
- [Pull Request Process](#pull-request-process)
- [Testing](#testing)
- [Documentation](#documentation)

## Code of Conduct

This project adheres to a [Code of Conduct](./CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- Angular CLI 17+
- Git
- Code editor (VS Code recommended)

### Setup Development Environment

```bash
# Fork and clone the repository
git clone https://github.com/yourusername/lost-yeti-angular.git
cd lost-yeti-angular

# Install dependencies
npm install

# Start development server
npm start
```

### Project Structure

```
src/
├── app/
│   ├── components/     # Reusable components
│   ├── sections/       # Page sections
│   ├── app.component.ts
│   └── app.routes.ts
├── assets/            # Static files
└── styles.css         # Global styles
```

## Development Workflow

### 1. Create a Branch

```bash
# Create a feature branch
git checkout -b feature/your-feature-name

# Or a bugfix branch
git checkout -b fix/bug-description
```

### Branch Naming Convention

- `feature/` - New features
- `fix/` - Bug fixes
- `docs/` - Documentation changes
- `style/` - Code style changes
- `refactor/` - Code refactoring
- `test/` - Test additions or changes
- `chore/` - Maintenance tasks

### 2. Make Changes

- Write clean, readable code
- Follow the coding standards
- Add comments for complex logic
- Update documentation as needed

### 3. Test Your Changes

```bash
# Run linting
npm run lint

# Run tests
npm test

# Build for production
npm run build:prod
```

### 4. Commit Your Changes

Follow the commit message guidelines below.

### 5. Push and Create Pull Request

```bash
git push origin feature/your-feature-name
```

Then create a Pull Request on GitHub.

## Coding Standards

### TypeScript

- Use TypeScript strict mode
- Define types for all variables and functions
- Avoid `any` type unless absolutely necessary
- Use interfaces for object shapes
- Use enums for fixed sets of values

```typescript
// Good
interface MenuItem {
  name: string;
  price: string;
  description: string;
}

// Bad
const item: any = { name: 'Food' };
```

### Angular Components

- Use standalone components
- Keep components focused and single-purpose
- Use OnPush change detection when possible
- Implement lifecycle hooks properly
- Unsubscribe from observables

```typescript
@Component({
  selector: 'app-example',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExampleComponent implements OnInit, OnDestroy {
  // Component logic
}
```

### HTML Templates

- Use semantic HTML
- Add ARIA labels for accessibility
- Keep templates clean and readable
- Use Angular directives appropriately

```html
<!-- Good -->
<button 
  type="button" 
  aria-label="Close menu"
  (click)="closeMenu()">
  Close
</button>

<!-- Bad -->
<div (click)="closeMenu()">Close</div>
```

### CSS/Tailwind

- Use Tailwind utility classes
- Follow mobile-first approach
- Use custom CSS only when necessary
- Maintain consistent spacing

```html
<!-- Good -->
<div class="flex flex-col md:flex-row gap-4">
  <!-- Content -->
</div>
```

### File Naming

- Use kebab-case for files: `my-component.component.ts`
- Use PascalCase for classes: `MyComponent`
- Use camelCase for variables: `myVariable`
- Use UPPER_CASE for constants: `MAX_ITEMS`

## Commit Guidelines

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Test additions or changes
- `chore`: Maintenance tasks
- `perf`: Performance improvements

### Examples

```
feat(reservation): add custom calendar component

Implemented a custom calendar component for date selection
in the reservation form with disabled past dates.

Closes #123
```

```
fix(header): resolve mobile menu toggle issue

Fixed the mobile menu not closing when clicking on links.
Added click handlers to menu items.
```

## Pull Request Process

### Before Submitting

1. ✅ Update documentation
2. ✅ Add/update tests
3. ✅ Run linting and tests
4. ✅ Build successfully
5. ✅ Update CHANGELOG.md

### PR Title Format

Use the same format as commit messages:
```
feat(component): add new feature
```

### PR Description Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests pass
- [ ] Manual testing completed
- [ ] No console errors

## Screenshots
(if applicable)

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex code
- [ ] Documentation updated
- [ ] No new warnings generated
```

### Review Process

1. At least one approval required
2. All CI checks must pass
3. No merge conflicts
4. Documentation updated
5. Tests passing

## Testing

### Unit Tests

```bash
# Run all tests
npm test

# Run tests in watch mode
npm run test:watch

# Generate coverage report
npm run test:coverage
```

### E2E Tests

```bash
# Run e2e tests
npm run e2e
```

### Manual Testing Checklist

- [ ] Desktop browsers (Chrome, Firefox, Safari, Edge)
- [ ] Mobile browsers (iOS Safari, Chrome)
- [ ] Responsive breakpoints
- [ ] Keyboard navigation
- [ ] Screen reader compatibility
- [ ] Performance (Lighthouse score)

## Documentation

### Code Comments

```typescript
/**
 * Handles date selection from the calendar
 * @param date - Selected date object
 */
onDateSelected(date: Date): void {
  this.reservation.date = date;
}
```

### Component Documentation

Each component should have:
- Purpose description
- Input/Output documentation
- Usage examples
- Dependencies listed

### README Updates

Update README.md when:
- Adding new features
- Changing setup process
- Modifying dependencies
- Updating configuration

## Questions?

- Open an issue for bugs
- Start a discussion for questions
- Email: hello@thelostyeti.com

## Recognition

Contributors will be added to [CONTRIBUTORS.md](./CONTRIBUTORS.md).

Thank you for contributing! 🎉
