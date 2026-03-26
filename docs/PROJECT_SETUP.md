# Project Setup Guide

Complete guide to setting up The Lost Yeti Kitchen & Bar development environment.

## Prerequisites

### Required Software

- **Node.js**: 18.x or higher
- **npm**: 9.x or higher
- **Git**: Latest version
- **Angular CLI**: 17.x

### Recommended Tools

- **VS Code**: With Angular extensions
- **Chrome**: For debugging
- **Docker**: For containerization (optional)

## Installation Steps

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/lost-yeti-angular.git
cd lost-yeti-angular
```

### 2. Install Dependencies

```bash
npm install
```

This will install all required packages listed in `package.json`.

### 3. Verify Installation

```bash
# Check Node version
node --version  # Should be 18.x or higher

# Check npm version
npm --version   # Should be 9.x or higher

# Check Angular CLI
ng version      # Should be 17.x
```

### 4. Start Development Server

```bash
npm start
```

The application will be available at `http://localhost:4200`

## Development Environment Setup

### VS Code Extensions

Install these recommended extensions:

1. **Angular Language Service** - Angular IntelliSense
2. **ESLint** - Code linting
3. **Prettier** - Code formatting
4. **Angular Snippets** - Code snippets
5. **GitLens** - Git integration
6. **Path Intellisense** - Path autocomplete
7. **Auto Rename Tag** - HTML tag renaming
8. **Bracket Pair Colorizer** - Bracket matching

### VS Code Settings

Create `.vscode/settings.json`:

```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "editor.codeActionsOnSave": {
    "source.fixAll.eslint": true
  },
  "typescript.tsdk": "node_modules/typescript/lib",
  "angular.enable-strict-mode-prompt": false
}
```

## Project Configuration

### Environment Variables

Create environment files (future):

```bash
# Development
src/environments/environment.ts

# Production
src/environments/environment.prod.ts
```

### Angular Configuration

The project uses `angular.json` for configuration:

- **Build options**: Output path, assets, styles, scripts
- **Serve options**: Port, host, proxy
- **Test options**: Karma configuration

## Available Scripts

### Development

```bash
# Start dev server
npm start

# Start with custom port
ng serve --port 4300

# Start with host binding
ng serve --host 0.0.0.0
```

### Building

```bash
# Development build
npm run build

# Production build
npm run build:prod

# Watch mode
npm run watch
```

### Testing

```bash
# Run unit tests
npm test

# Run tests with coverage
npm run test:coverage

# Run e2e tests
npm run e2e
```

### Linting

```bash
# Run ESLint
npm run lint

# Fix linting issues
npm run lint:fix
```

### Code Analysis

```bash
# Bundle analysis
npm run analyze
```

## Troubleshooting

### Common Issues

#### Port Already in Use

```bash
# Kill process on port 4200
# Windows
netstat -ano | findstr :4200
taskkill /PID <PID> /F

# Mac/Linux
lsof -ti:4200 | xargs kill -9
```

#### Node Modules Issues

```bash
# Clear cache and reinstall
rm -rf node_modules package-lock.json
npm cache clean --force
npm install
```

#### Angular CLI Issues

```bash
# Reinstall Angular CLI
npm uninstall -g @angular/cli
npm install -g @angular/cli@17
```

#### Build Errors

```bash
# Clear Angular cache
rm -rf .angular
npm run build
```

### Getting Help

- Check [GitHub Issues](#)
- Read [Documentation](./README.md)
- Contact: hello@thelostyeti.com

## Next Steps

After setup:

1. Read [STRUCTURE.md](./STRUCTURE.md) - Understand project architecture
2. Read [CONTRIBUTING.md](./CONTRIBUTING.md) - Learn contribution guidelines
3. Read [FEATURES.md](./FEATURES.md) - Explore features
4. Start coding!

## Docker Setup (Optional)

### Build Docker Image

```bash
docker build -t lost-yeti .
```

### Run Container

```bash
docker run -p 80:80 lost-yeti
```

### Docker Compose

Create `docker-compose.yml`:

```yaml
version: '3.8'
services:
  web:
    build: .
    ports:
      - "80:80"
    restart: unless-stopped
```

Run with:

```bash
docker-compose up -d
```

## CI/CD Setup

### GitHub Actions

Workflows are in `.github/workflows/`:

- `ci.yml` - Continuous Integration
- `deploy.yml` - Deployment
- `security.yml` - Security scanning

### Required Secrets

Add these to GitHub repository secrets:

- `DOCKER_USERNAME`
- `DOCKER_PASSWORD`
- `VERCEL_TOKEN`
- `NETLIFY_AUTH_TOKEN`
- `AWS_ACCESS_KEY_ID`
- `AWS_SECRET_ACCESS_KEY`
- `SNYK_TOKEN`

## Production Deployment

See [DEPLOYMENT.md](./DEPLOYMENT.md) for detailed deployment instructions.

---

**Need help?** Open an issue or contact the team!
