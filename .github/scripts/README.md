# Documentation Automation Scripts

This directory contains Node.js scripts that automatically update project documentation when changes are made to the codebase.

## Scripts Overview

### 1. update-structure.js
**Purpose**: Updates `docs/STRUCTURE.md` with current project structure and file statistics.

**Triggers**:
- Any changes to `src/**` files
- Changes to `package.json`, `angular.json`, or `tsconfig.json`

**Updates**:
- File tree structure
- Component count
- TypeScript file count
- HTML template count
- CSS stylesheet count
- Last updated timestamp

**Usage**:
```bash
node .github/scripts/update-structure.js
```

### 2. update-features.js
**Purpose**: Updates `docs/FEATURES.md` with component inventory and API information.

**Triggers**:
- Changes to component files
- New components added

**Updates**:
- Component list with selectors
- Input/Output properties
- Component locations
- Total component count

**Usage**:
```bash
node .github/scripts/update-features.js [componentCount] [sectionCount]
```

### 3. update-changelog.js
**Purpose**: Updates `docs/CHANGELOG.md` with recent commits and changes.

**Triggers**:
- Push to main branch
- Pull request merges

**Updates**:
- Unreleased section with categorized commits
- Modified files list
- Timestamp

**Commit Categories**:
- `feat:` → Added
- `fix:` → Fixed
- `docs:` → Documentation
- `style:` → Changed
- `refactor:` → Changed
- `perf:` → Performance
- `test:` → Testing
- `security` → Security

**Usage**:
```bash
COMMITS="..." CHANGED_FILES="..." node .github/scripts/update-changelog.js
```

### 4. update-technologies.js
**Purpose**: Updates `docs/TECHNOLOGIES.md` with dependency versions and tech stack.

**Triggers**:
- Changes to `package.json`
- Dependency updates

**Updates**:
- Angular version
- All dependencies with versions
- Dev dependencies
- Build configuration
- Browser support
- Performance features

**Usage**:
```bash
node .github/scripts/update-technologies.js
```

### 5. update-readme.js
**Purpose**: Updates `README.md` with project statistics and badges.

**Triggers**:
- Any code changes
- Component additions

**Updates**:
- Component count badge
- File count badges
- Last updated timestamp

**Usage**:
```bash
TOTAL_FILES=X TS_FILES=Y ... node .github/scripts/update-readme.js
```

### 6. update-sitemap.js
**Purpose**: Updates `src/sitemap.xml` with current routes and sections.

**Triggers**:
- Changes to `app.routes.ts`
- New sections added

**Updates**:
- URL list
- Last modified dates
- Priority levels
- Change frequency

**Usage**:
```bash
node .github/scripts/update-sitemap.js
```

## Workflow Integration

These scripts are integrated into GitHub Actions workflows:

### docs-update.yml
Main documentation update workflow that runs all scripts:
- `update-structure.js` - On any src changes
- `update-features.js` - On component changes
- `update-changelog.js` - On main branch pushes
- `update-technologies.js` - On package.json changes
- `update-readme.js` - On any changes
- API documentation generation with Compodoc

### sitemap-update.yml
Dedicated sitemap update workflow:
- `update-sitemap.js` - On route changes
- XML validation
- Automatic commit

## How It Works

1. **Trigger**: Code changes pushed to repository
2. **Detection**: GitHub Actions detects changed files
3. **Execution**: Relevant scripts run automatically
4. **Update**: Documentation files are updated
5. **Commit**: Changes are committed back to repository with `[skip ci]` flag

## Manual Execution

You can run any script manually:

```bash
# Update structure documentation
npm run docs:structure

# Update all documentation
npm run docs:update

# Update sitemap
npm run docs:sitemap
```

Add these scripts to `package.json`:
```json
{
  "scripts": {
    "docs:structure": "node .github/scripts/update-structure.js",
    "docs:features": "node .github/scripts/update-features.js",
    "docs:changelog": "node .github/scripts/update-changelog.js",
    "docs:technologies": "node .github/scripts/update-technologies.js",
    "docs:readme": "node .github/scripts/update-readme.js",
    "docs:sitemap": "node .github/scripts/update-sitemap.js",
    "docs:update": "npm run docs:structure && npm run docs:features && npm run docs:technologies && npm run docs:readme && npm run docs:sitemap"
  }
}
```

## Requirements

- Node.js 18+ (20+ recommended)
- npm 9+
- Git repository
- GitHub Actions enabled

## Configuration

### Skip CI
All automated commits include `[skip ci]` to prevent infinite loops.

### Permissions
Workflows need write permissions to commit changes:
```yaml
permissions:
  contents: write
```

### Secrets
Uses `GITHUB_TOKEN` for authentication (automatically provided by GitHub Actions).

## Error Handling

All scripts include:
- Try-catch blocks
- Descriptive error messages
- Non-zero exit codes on failure
- Console logging for debugging

## Best Practices

1. **Review Changes**: Always review automated commits
2. **Test Locally**: Run scripts locally before pushing
3. **Keep Updated**: Update scripts when project structure changes
4. **Monitor Workflows**: Check GitHub Actions for failures
5. **Version Control**: Commit script changes with descriptive messages

## Troubleshooting

### Script Fails
- Check Node.js version
- Verify file paths exist
- Review error messages in Actions logs

### No Updates
- Verify trigger paths in workflow
- Check if `[skip ci]` is preventing runs
- Ensure permissions are correct

### Merge Conflicts
- Pull latest changes before pushing
- Resolve conflicts in documentation files
- Re-run workflows if needed

## Contributing

When adding new scripts:
1. Follow existing naming convention
2. Add error handling
3. Include console logging
4. Update this README
5. Add to relevant workflow
6. Test thoroughly

## License

Same as main project license.
